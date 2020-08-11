using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Integration.OutboundTransactions;
using Database;
using Entities.Domain.Accounts;
using Entities.Domain.Payment;
using Entities.Domain.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Endpoints.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly SandBankDbContext _db;
        private readonly EventPublisher<Transaction> _transactionEventPublisher;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentController> _logger;
        private readonly IAmazonCloudWatch _cloudWatch;
        
        public PaymentController(SandBankDbContext db, 
            EventPublisher<Transaction> transactionEventPublisher, 
            IConfiguration config,
            ILogger<PaymentController> logger,
            IAmazonCloudWatch cloudWatch)
        { 
            _db = db;
            _transactionEventPublisher = transactionEventPublisher;
            _config = config;
            _logger = logger;
            _cloudWatch = cloudWatch;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> PostPayment([FromBody] PostPaymentRequest postPaymentRequest)
        {
            _logger.LogInformation("incoming post payment request", postPaymentRequest);
            if (IsValid(postPaymentRequest))
            {
                var (debit, credit) = CreateTransactions(postPaymentRequest);
                
                var fromAccount = await GetAccount(postPaymentRequest.FromAccount);

                if (fromAccount == null)
                {
                    return NotFound();
                }
                
                fromAccount.PostTransaction(debit);

                var interIntra = "";
                
                if (IsIntrabank(postPaymentRequest))
                {
                    _logger.LogInformation("intrabank payment request", postPaymentRequest);
                    var toAccount = await GetAccount(postPaymentRequest.ToAccount, false);
                    
                    if (toAccount == null)
                    {
                        return NotFound();
                    }
                    
                    toAccount.PostTransaction(credit);
                    interIntra = "intra";
                }
                else
                {
                    _logger.LogInformation("outbound payment request", postPaymentRequest);
                    await AddToSettlementBatch(credit);
                    interIntra = "inter";
                }

                await _db.SaveChangesAsync();
                
                _logger.LogInformation($"Posted {interIntra}-bank transaction of ${credit.Amount} from {postPaymentRequest.FromAccount} to {postPaymentRequest.ToAccount}");
                
                await _cloudWatch.PutMetricDataAsync(new PutMetricDataRequest
                {
                    Namespace = "Payments",
                    MetricData = new List<MetricDatum>
                    {
                        new MetricDatum
                        {
                            MetricName = "TransferEvent",
                            Unit = StandardUnit.Count, 
                            Value = 1, 
                            TimestampUtc = DateTime.UtcNow,
                            Dimensions = new List<Dimension>
                            {
                                new Dimension { Name = "InterIntra", Value = interIntra }
                            }
                        },
                        new MetricDatum
                        {
                            MetricName = "TransferValue",
                            Unit = StandardUnit.None, 
                            Value = (double) credit.Amount, 
                            TimestampUtc = DateTime.UtcNow,
                            Dimensions = new List<Dimension>
                            {
                                new Dimension { Name = "InterIntra", Value = interIntra }
                            }
                        }
                    }
                });
                
                return Ok();
            }

            return UnprocessableEntity();
        }

        private static (Transaction, Transaction) CreateTransactions(PostPaymentRequest postPaymentRequest)
        {
            var utcNow = DateTime.UtcNow;
            
            var debitTransaction = new Transaction
            {
                Amount = postPaymentRequest.Amount * -1,
                TransactionTimeUtc = utcNow,
                Description = postPaymentRequest.Description,
                MerchantName = postPaymentRequest.MerchantName
            };
            
            var creditTransaction = new Transaction
            {
                //this needs tweaking... doesn't work for outbound transactions because it doesn't capture the account number to send it to
                Amount = postPaymentRequest.Amount,
                TransactionTimeUtc = utcNow,
                Description = postPaymentRequest.Description,
                MerchantName = postPaymentRequest.MerchantName
            };
            
            return (debitTransaction, creditTransaction);
        }

        //refactor pull out to service / specification
        private async Task<Account> GetAccount(string accountNumber, bool isOwnAccount = true)
        {
            if (isOwnAccount)
            { 
                return await _db.Accounts
                   .Include(acc => acc.AccountTransactions)
                   .FirstOrDefaultAsync(acc => acc.AccountNumber == accountNumber);
            }
            
            return await _db.Accounts.IgnoreQueryFilters()
                .Include(acc => acc.AccountTransactions)
                .FirstOrDefaultAsync(acc => acc.AccountNumber == accountNumber);
        }

        private bool IsIntrabank(PostPaymentRequest postPaymentRequest)
        {
            return postPaymentRequest.ToAccount.StartsWith(_config["BankPrefix"]);
        }

        private bool IsValid(PostPaymentRequest postPaymentRequest)
        {
            return postPaymentRequest.Amount > 0M && IsExistingAccount(postPaymentRequest.FromAccount);
        }

        private bool IsExistingAccount(string accountNumber)
        {
            return _db.Accounts.SingleOrDefault(acc => acc.AccountNumber == accountNumber) != null;
        }

        private async Task AddToSettlementBatch(Transaction outgoingTransaction)
        {
            try
            {
                _logger.LogInformation("publish payment request", outgoingTransaction);
                var response = await _transactionEventPublisher.Publish(outgoingTransaction);
                _logger.LogInformation($"payment request finished with messageid={response.MessageId}", response.MessageId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"payment request failed with message={e.Message}");
            }
        }
    }
}
