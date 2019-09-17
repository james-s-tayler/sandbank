using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.MultiTenant;
using CsvHelper;
using Domain.Account;
using Domain.Transaction;
using Endpoints.Configuration;
using Endpoints.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Endpoints.Controllers
{
    [Authorize]
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

        public AccountController(SandBankDbContext db,
            INumberRangeService numberRangeService,
            IConfiguration config,
            ITenantProvider tenantProvider)
        {
            _db = db;
            _numberRangeService = numberRangeService;
            _config = config;
            _tenantProvider = tenantProvider;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<AccountViewModel>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAccounts()
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
        public async Task<IActionResult> PostAccount([FromBody] OpenAccountRequest openAccountRequest)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _tenantProvider.GetTenantId());
            
            if (user == null)
            {
                return NotFound();
            }

            var account = openAccountRequest.ToDomainModel();
            account.AccountOwnerId = _tenantProvider.GetTenantId();

            //hacked this slightly to be NZ format. Not nice, but it works.
            var nextAccountNum = await _numberRangeService.GetNextValue(account.AccountType == AccountType.TRANSACTION
                ? NumberRangeType.Cheque
                : NumberRangeType.Savings);
            var bankPrefix = _config["BankPrefix"];
            var branch = "0001";
            var suffix = account.AccountType == AccountType.TRANSACTION ? "00" : "30"; 
            account.AccountNumber = $"{bankPrefix}-{branch}-{nextAccountNum}-{suffix}";

            await _db.Accounts.AddAsync(account);
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
                using (var reader = System.IO.File.OpenText("seed-transactions.csv"))
                using (var csv = new CsvReader(reader))
                {
                    var transactionsCsvModels = csv.GetRecords<TransactionCsvModel>();
                    var transactions = transactionsCsvModels.Select(t => t.ConvertToTransaction());
                    foreach (var transaction in transactions)
                    {
                        account.PostTransaction(transaction);
                    }
                    await _db.SaveChangesAsync();
                }
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
                using (var csvStream = csvFile.OpenReadStream())
                using (var reader = new StreamReader(csvStream))
                using (var csv = new CsvReader(reader))
                {
                    var transactionsCsvModels = csv.GetRecords<TransactionCsvModel>();
                    var transactions = transactionsCsvModels.Select(t => t.ConvertToTransaction());
                    foreach (var transaction in transactions)
                    {
                        account.PostTransaction(transaction);
                    }
                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                return UnprocessableEntity();
            }
            
            return Ok();
        }
    }
}
