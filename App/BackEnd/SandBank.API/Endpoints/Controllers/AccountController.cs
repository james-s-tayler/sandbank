using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Account;
using Domain.Transaction;
using Endpoints.Configuration;
using Endpoints.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Controllers
{
    [ApiController]
    [Route("api/User/{id}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly SandBankDbContext _db;
        private INumberRangeService _numberRangeService;

        public AccountController(SandBankDbContext db,
            INumberRangeService numberRangeService)
        {
            _db = db;
            _numberRangeService = numberRangeService;
        }

        [HttpGet("")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AccountViewModel>))]
        public async Task<IActionResult> GetAccounts([FromRoute] int id)
        {
            var accounts = _db.Accounts.Where(acc => acc.AccountOwnerId == id).ToList();
            
            if (accounts != null)
            {
                return Ok(accounts.Select(acc => new AccountViewModel(acc)));
            }
            return NotFound();
        }
        
        [HttpGet("{accountId}")]
        [ProducesResponseType(200, Type = typeof(AccountViewModel))]
        public async Task<IActionResult> GetAccount([FromRoute] int id, [FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == id && acc.Id == accountId);
            
            if (account != null)
            {
                return Ok(new AccountViewModel(account));
            }
            return NotFound();
        }
        
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AccountViewModel))]
        public async Task<IActionResult> PostAccount([FromBody] OpenAccountRequest openAccountRequest, [FromRoute] int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null)
            {
                return UnprocessableEntity();
            }

            var account = openAccountRequest.ToDomainModel();
            account.AccountOwnerId = id;
            account.AccountNumber = await _numberRangeService.GetNextValue(NumberRangeType.Account);
            
            await _db.Accounts.AddAsync(account);
            await _db.SaveChangesAsync();
            
            return Ok(new AccountViewModel(account));
        }
        
        [HttpGet("{accountId}/Balance")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public async Task<IActionResult> GetBalance([FromRoute] int id, [FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .Include(acc => acc.AccountTransactions)
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == id && acc.Id == accountId);
            
            if (account != null)
            {
                return Ok(account.AccountTransactions.Sum(txn => txn.Amount));
            }
            return NotFound();
        }
        
        [HttpGet("{accountId}/Transaction")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Transaction>))]
        public async Task<IActionResult> GetTransactions([FromRoute] int id, [FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .Include(acc => acc.AccountTransactions)
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == id && acc.Id == accountId);
            
            if (account != null)
            {
                return Ok(account.AccountTransactions.Select(txn => new TransactionViewModel(txn)));
            }
            return NotFound();
        }
        
        [HttpPost("{accountId}/Seed")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Transaction>))]
        public async Task<IActionResult> SeedTransactions([FromRoute] int id, [FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .Include(acc => acc.AccountTransactions)
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == id && acc.Id == accountId);
            
            if (account != null)
            {
                if (account.AccountTransactions.Any())
                {
                    return UnprocessableEntity();
                }
                
                var timeStamp = DateTime.UtcNow;
                
                account.PostTransaction(new Transaction { Amount = 12.34M, TransactionTimeUtc = timeStamp, Description = "transaction 1"});
                account.PostTransaction(new Transaction { Amount = 0.34M, TransactionTimeUtc = timeStamp, Description = "transaction 2"});
                account.PostTransaction(new Transaction { Amount = 2.99M, TransactionTimeUtc = timeStamp, Description = "transaction 3"});
                
                
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(GetTransactions), new { id = id, accountId = accountId });
            }
            return NotFound();
        }
    }
}