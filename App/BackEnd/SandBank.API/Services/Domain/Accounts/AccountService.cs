using System.Threading.Tasks;
using Database;
using Entities.Domain.Accounts;
using Entities.System.NumberRanges;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.System.NumberRange;

namespace Services.Domain.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly SandBankDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountService> _logger;
        private readonly INumberRangeService _numberRangeService;

        public AccountService(SandBankDbContext db,
            IConfiguration config,
            ILogger<AccountService> logger,
            INumberRangeService numberRangeService)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _numberRangeService = numberRangeService;
        }

        public async Task<Account> OpenAccount(OpenAccountRequest openAccountRequest, int accountOwnerId)
        {
            _logger.LogInformation($"Opening {openAccountRequest.AccountType} Account '{openAccountRequest.DisplayName}' for userId: {accountOwnerId}");
            
            var account = openAccountRequest.ToDomainModel();
            account.AccountOwnerId = accountOwnerId;

            //hacked this slightly to be NZ format. Not nice, but it works.
            var nextAccountNum = await _numberRangeService.GetNextValue(account.AccountType == AccountType.TRANSACTION
                ? NumberRangeType.Cheque
                : NumberRangeType.Savings);
            var bankPrefix = _config["BankPrefix"];
            var branch = "0001";
            var suffix = account.AccountType == AccountType.TRANSACTION ? "00" : "30"; 
            account.AccountNumber = $"{bankPrefix}-{branch}-{nextAccountNum}-{suffix}";

            await _db.Accounts.AddAsync(account);
            return account;
        }
    }
}