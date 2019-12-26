using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.MultiTenant;
using Database;
using Entities.Domain.Accounts;
using Entities.Domain.Transactions;
using Entities.System.NumberRanges;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services.Domain.Accounts;
using Services.System.NumberRange;

namespace Endpoints.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly SandBankDbContext _db;
        private readonly INumberRangeService _numberRangeService;
        private readonly IConfiguration _config;
        private readonly ITenantProvider _tenantProvider;
        private readonly ISeedTransactionDataService _seedTransactionDataService;
        private readonly IAccountService _accountService;

        public AccountController(SandBankDbContext db,
            INumberRangeService numberRangeService,
            IConfiguration config,
            ITenantProvider tenantProvider,
            ISeedTransactionDataService seedTransactionDataService,
            IAccountService accountService)
        {
            _db = db;
            _numberRangeService = numberRangeService;
            _config = config;
            _tenantProvider = tenantProvider;
            _seedTransactionDataService = seedTransactionDataService;
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<AccountViewModel>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetAccounts()
        {
            var accounts = _db.Accounts.Where(acc => acc.AccountOwnerId == _tenantProvider.GetTenantId()).ToList();
            
            if (accounts != null)
            {
                return Ok(accounts.Select(acc => new AccountViewModel(acc)));
            }
            
            return NotFound();
        }
        
        [HttpGet("{accountId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AccountViewModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAccount([FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == _tenantProvider.GetTenantId() && acc.Id == accountId);
            
            if (account != null)
            {
                return Ok(new AccountViewModel(account));
            }
            
            return NotFound();
        }
        
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AccountViewModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> OpenAccount([FromBody] OpenAccountRequest openAccountRequest)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _tenantProvider.GetTenantId());
            
            if (user == null)
            {
                return NotFound();
            }

            var account = await _accountService.OpenAccount(openAccountRequest, _tenantProvider.GetTenantId());
            await _db.SaveChangesAsync();
            
            return Ok(new AccountViewModel(account));
        }
        
        [HttpGet("{accountId}/Balance")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(decimal))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetBalance([FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .Include(acc => acc.AccountTransactions)
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == _tenantProvider.GetTenantId() && acc.Id == accountId);
            
            if (account != null)
            {
                return Ok(account.AccountTransactions.Sum(txn => txn.Amount));
            }
            
            return NotFound();
        }
        
        [HttpGet("{accountId}/Transaction")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<TransactionViewModel>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[CalledBy("Transactions.vue")]
        public async Task<IActionResult> GetTransactions([FromRoute] int accountId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            if (from == null || to == null)
            {
                from = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));
                to = DateTime.UtcNow;
            }

            if (from.Value > to.Value)
            {
                return BadRequest();
            }
            
            var account = await _db.Accounts
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == _tenantProvider.GetTenantId() && acc.Id == accountId);

            if (account == null)
            {
                return NotFound();
            }

            var transactions = await _db.Transactions
                    .Where(t => t.AccountId == account.Id)
                    .Where(t => t.TransactionTimeUtc > from)
                    .Where(t => t.TransactionTimeUtc < to)
                    .ToListAsync();
            
            return Ok(transactions.Select(txn => new TransactionViewModel(txn)));
        }
        
        [HttpPost("{accountId}/Seed")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SeedTransactions([FromRoute] int accountId)
        {
            var account = await _db.Accounts
                .Include(acc => acc.AccountTransactions)
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == _tenantProvider.GetTenantId() && acc.Id == accountId);

            if (account == null)
            {
                return NotFound();
            }
            
            if (account.AccountTransactions.Any())
            {
                return UnprocessableEntity();
            }

            try
            {
                var transactions = _seedTransactionDataService.ReadFromFile();
                foreach (var transaction in transactions)
                {
                    account.PostTransaction(transaction);
                }
                await _db.SaveChangesAsync();
            }
            catch
            {
                return UnprocessableEntity();
            }

            return Ok();
        }
        
        [HttpPost("{accountId}/SeedFile")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SeedTransactions([FromRoute] int accountId, IFormFile csvFile)
        {
            if (csvFile == null)
            {
                return BadRequest("No csv file submitted");
            }
            
            var account = await _db.Accounts
                .Include(acc => acc.AccountTransactions)
                .FirstOrDefaultAsync(acc => acc.AccountOwnerId == _tenantProvider.GetTenantId() && acc.Id == accountId);

            if (account == null)
            {
                return NotFound();
            }
            
            if (account.AccountTransactions.Any())
            {
                return UnprocessableEntity();
            }

            try
            {
                var transactions = _seedTransactionDataService.ReadFromFormPost(csvFile);
                foreach (var transaction in transactions)
                {
                    account.PostTransaction(transaction);
                }
                await _db.SaveChangesAsync();
            }
            catch
            {
                return UnprocessableEntity();
            }
            
            return Ok();
        }
    }
}
