using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Core.MultiTenant;
using Database;
using Entities.Domain.Accounts;
using Entities.Domain.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services.Domain.Accounts;
using Services.System.NumberRange;
using TechDebtTags;
using AccountMetadata = Models.DynamoDB.AccountMetadata;

namespace Api.Controllers
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
        private readonly IAmazonDynamoDB _dynamoDb;

        public AccountController(SandBankDbContext db,
            INumberRangeService numberRangeService,
            IConfiguration config,
            ITenantProvider tenantProvider,
            ISeedTransactionDataService seedTransactionDataService,
            IAccountService accountService,
            IAmazonDynamoDB dynamoDb)
        {
            _db = db;
            _numberRangeService = numberRangeService;
            _config = config;
            _tenantProvider = tenantProvider;
            _seedTransactionDataService = seedTransactionDataService;
            _accountService = accountService;
            _dynamoDb = dynamoDb;
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
        
        [TechnicalDebt("This is an incorrect use of HttpClient", 
            "Newing up HttpClients can lead to socket exhaustion on the server if heavily trafficked.",
            "We should register a special http client on Startup just for this purpose and let the HttpClientFactory do it's magic.")]
        [HttpGet("picture")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(decimal))]
        public async Task<IActionResult> GetPicture()
        {
            
            using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip, AllowAutoRedirect = false}) { Timeout = TimeSpan.FromSeconds(30) })
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://source.unsplash.com/random/100x100"),
                    Method = HttpMethod.Get
                };
 
                HttpResponseMessage response = await client.SendAsync(request);
                var statusCode = (int)response.StatusCode;
                // We want to handle redirects ourselves so that we can determine the final redirect Location (via header)
                if (statusCode >= 300 && statusCode <= 399)
                {
                    var redirectUri = response.Headers.Location;
                    
                    if (!redirectUri.IsAbsoluteUri)
                    {
                        redirectUri = new Uri(request.RequestUri.GetLeftPart(UriPartial.Authority) + redirectUri);
                    }

                    return Ok(redirectUri);
                }
                else if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }
 
                return NotFound();
            }
        }
        
        [HttpGet("{accountId}/Metadata")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(decimal))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMetadata([FromRoute] int accountId)
        {
            var userId = _tenantProvider.GetTenantId();
            using (var context = new DynamoDBContext(_dynamoDb))
            {
                var accountMetadata = await context.LoadAsync<AccountMetadata>(userId, accountId);
                
                if (accountMetadata == null) return NotFound();

                return Ok(new AccountMetadataViewModel
                {
                    Nickname = accountMetadata.Nickname,
                    AccountId = accountMetadata.AccountId,
                    UserId = accountMetadata.UserId,
                    ImageUrl = accountMetadata.ImageUrl,
                });
            }
        }

        [TechnicalDebt("There should be some validation to check the user actually has an account with the given id",
            "User will be able to create bogus records in DynamoDB",
            "We need to setup a postgresql container to run as part of the integration tests, so that we can handle this, then simply perform the check")]
        [HttpPost("{accountId}/Metadata")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateMetadata([FromRoute] int accountId, [FromBody] AccountMetadata metadata)
        {
            var userId = _tenantProvider.GetTenantId();
            metadata.UserId = userId;
            metadata.AccountId = accountId;
            metadata.LastModified = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

            using (var context = new DynamoDBContext(_dynamoDb))
            {
                await context.SaveAsync(metadata);
                return Ok();
            }
            return Ok();
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
