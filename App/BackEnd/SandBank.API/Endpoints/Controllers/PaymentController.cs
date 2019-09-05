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

namespace Endpoints.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly SandBankDbContext _db;
        private readonly IOutboundTransactionProcessor _outboundTransactionProcessor;

        public PaymentController(SandBankDbContext db, IOutboundTransactionProcessor outboundTransactionProcessor)
        { 
            _db = db;
            _outboundTransactionProcessor = outboundTransactionProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> PostPayment([FromBody] PostPaymentRequest postPaymentRequest)
        {
            if (IsValid(postPaymentRequest))
            {
                var (debit, credit) = CreateTransactions(postPaymentRequest);
                
                var fromAccount = await GetAccount(postPaymentRequest.FromAccount);
                fromAccount.PostTransaction(debit);

                if (IsIntrabank(postPaymentRequest))
                {
                    var toAccount = await GetAccount(postPaymentRequest.ToAccount);
                    toAccount.PostTransaction(credit);
                }
                else
                {
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

        private async void AddToSettlementBatch(Transaction outgoingTransaction)
        {
            //ideally decouple this by simply placing a message on a queue
            await _outboundTransactionProcessor.Process(outgoingTransaction);
        }
    }
}