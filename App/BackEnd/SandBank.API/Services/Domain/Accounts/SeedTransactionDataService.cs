using System;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.MultiTenant;
using Database;
using Entities.Domain.Accounts;
using Entities.Domain.Transactions;
using Entities.System.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services.Domain.Accounts
{
    public class SeedTransactionDataService : ISeedTransactionDataService
    {
        private string SEED_FILE = "seed-transactions.csv";
        
        private readonly SandBankDbContext _db;
        private readonly ILogger<SeedTransactionDataService> _logger;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;
        private readonly ITenantProvider _tenantProvider;

        public SeedTransactionDataService(SandBankDbContext db,
            ILogger<SeedTransactionDataService> logger,
            IConfiguration config,
            IAccountService accountService,
            ITenantProvider tenantProvider)
        {
            _db = db;
            _logger = logger;
            _config = config;
            _accountService = accountService;
            _tenantProvider = tenantProvider;
        }

        public async Task SeedData()
        {
            var seedUserEmail = _config["SeedUser:Email"];
            var seedUser = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(user => user.Email == seedUserEmail);

            if (seedUser == null)
            {
                _logger.LogInformation($"Seeding Database...");
                _logger.LogInformation($"Creating User...");
                var user = new User
                {
                    FullName = "John Smith",
                    Address = "1099 King Street",
                    City = "Auckland",
                    Country = "New Zealand",
                    PostCode = "1024",
                    DateOfBirth = DateTime.UtcNow.AddYears(-30),
                    Email = seedUserEmail,
                    Phone = "+642112345678",
                    CreatedOn = DateTime.UtcNow
                };
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
                
                _tenantProvider.SetTenantId(user.Id);

                _logger.LogInformation($"Creating Accounts...");
                var account1 = await _accountService.OpenAccount(
                    new OpenAccountRequest { AccountType = AccountType.TRANSACTION, DisplayName = "Go"}, user.Id);
                var account2 = await _accountService.OpenAccount(
                    new OpenAccountRequest { AccountType = AccountType.SAVING, DisplayName = "Savings"}, user.Id);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Seeding Transactions...");
                var transactions = ReadFromFile();
                foreach (var transaction in transactions)
                {
                    account1.PostTransaction(transaction);
                }
                _db.Update(account1);
                await _db.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation($"Seed User {seedUserEmail} Already Exists...");    
            }
        }

        public IList<Transaction> ReadFromFile()
        {
            using (var reader = File.OpenText(SEED_FILE))
            using (var csv = new CsvReader(reader))
            {
                return ReadTransactions(csv);
            }
        }

        public IList<Transaction> ReadFromFormPost(IFormFile formFile)
        {
            using (var csvStream = formFile.OpenReadStream())
            using (var reader = new StreamReader(csvStream))
            using (var csv = new CsvReader(reader))
            {
                return ReadTransactions(csv);
            }
        }

        private IList<Transaction> ReadTransactions(CsvReader reader)
        {
            var transactionsCsvModels = reader.GetRecords<TransactionCsvModel>();
            var transactions = transactionsCsvModels.Select(t => t.ConvertToTransaction());
            return transactions.ToList();
        }
    }
}
