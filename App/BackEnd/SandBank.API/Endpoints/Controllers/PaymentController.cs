using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Account;
using Domain.Payment;
using Domain.Transaction;
using Endpoints.Data;
using Integration.OutboundTransactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private static ILogger<PaymentController> _logger;
        
        public PaymentController(SandBankDbContext db, EventPublisher<Transaction> transactionEventPublisher, ILogger<PaymentController> logger)
        { 
            _db = db;
            _transactionEventPublisher = transactionEventPublisher;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostPayment([FromBody] PostPaymentRequest postPaymentRequest)
        {
            _logger.LogInformation("incoming post payment request", postPaymentRequest);
            if (IsValid(postPaymentRequest))
            {
                var (debit, credit) = CreateTransactions(postPaymentRequest);
                
                var fromAccount = await GetAccount(postPaymentRequest.FromAccount);
                fromAccount.PostTransaction(debit);

                if (IsIntrabank(postPaymentRequest))
                {
                    _logger.LogInformation("intrabank payment request", postPaymentRequest);
                    var toAccount = await GetAccount(postPaymentRequest.ToAccount);
                    toAccount.PostTransaction(credit);
                }
                else
                {
                    _logger.LogInformation("outbound payment request", postPaymentRequest);
                    AddToSettlementBatch(credit);
                }

                await _db.SaveChangesAsync();

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
                Amount = postPaymentRequest.Amount,
                TransactionTimeUtc = utcNow,
                Description = postPaymentRequest.Description,
                MerchantName = postPaymentRequest.MerchantName
            };
            
            return (debitTransaction, creditTransaction);
        }

        private async Task<Account> GetAccount(string accountNumber)
        {
            return await _db.Accounts
                .Include(acc => acc.AccountTransactions)
                .FirstOrDefaultAsync(acc => acc.AccountNumber == accountNumber);
        }

        private bool IsIntrabank(PostPaymentRequest postPaymentRequest)
        {
            //this should route based on account prefix
            return IsExistingAccount(postPaymentRequest.ToAccount);
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
                _logger.LogInformation($"payment request finished with messageid={response.MessageId}",
                    response.MessageId);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"payment request failed with message={e.Message}", e.Message);
            }
        }
    }
}