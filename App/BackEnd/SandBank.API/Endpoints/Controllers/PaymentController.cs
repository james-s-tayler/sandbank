using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain.Account;
using Domain.Payment;
using Domain.Transaction;
using Endpoints.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Endpoints.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly SandBankDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(SandBankDbContext db,
            IConfiguration config,
            ILogger<PaymentController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> PostPayment([FromBody] PostPaymentRequest postPaymentRequest)
        {
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
                    AddToSettlementBatch(credit);
                    interIntra = "inter";
                }

                await _db.SaveChangesAsync();
                _logger.LogInformation($"Posted {interIntra}-bank transaction of ${credit.Amount} from {postPaymentRequest.FromAccount} to {postPaymentRequest.ToAccount}");

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

        private void AddToSettlementBatch(Transaction outgoingTransaction)
        {
            //transform and send to queue for outbound processing
        }
    }
}