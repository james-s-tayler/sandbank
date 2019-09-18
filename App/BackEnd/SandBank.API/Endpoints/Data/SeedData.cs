using Domain.Account;
using Domain.User;
using Endpoints.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Endpoints.Data
{
    public static class SeedData
    {
        public static void EnsureSeedData(SandBankDbContext context, ISeedTransactionDataService seedTransactionDataService)
        {
            Console.WriteLine("Seeding database...");
            if (!context.Users.IgnoreQueryFilters().Any())
            {
                Console.WriteLine("Creating User..");
                var seedUser = new User
                {
                    FullName = "Mike Jones",
                    Address = "17 Frater Ave",
                    City = "Auckland",
                    Country = "New Zealand",
                    PostCode = "0620",
                    DateOfBirth = new DateTime(1994, 3, 13),
                    Email = "hello@bankengine.nz",
                    Phone = "+642108124230",
                    CreatedOn = DateTime.UtcNow
                };

                context.Add(seedUser);
                context.SaveChanges();

                Console.WriteLine("Creating Accounts..");
                var seedAccount = new Account
                {
                    DisplayName = "Go",
                    AccountType = AccountType.TRANSACTION,
                    AccountNumber = "99-0001-0000001-00",
                    AccountOwnerId = seedUser.Id,
                    AccountOwner = seedUser,
                    CreatedOn = DateTime.UtcNow,
                };

                var seedAccount2 = new Account
                {
                    DisplayName = "Savings",
                    AccountType = AccountType.SAVING,
                    AccountNumber = "99-0001-0000001-50",
                    AccountOwnerId = seedUser.Id,
                    AccountOwner = seedUser,
                    CreatedOn = DateTime.UtcNow,
                };


                context.Add(seedAccount);
                context.Add(seedAccount2);
                context.SaveChangesSeed(seedUser.Id);

                Console.WriteLine("Creating Transactions..");
                var transactions = seedTransactionDataService.ReadFromFile();
                foreach (var transaction in transactions)
                {
                    seedAccount.PostTransaction(transaction);
                }
                context.Update(seedAccount);
                context.SaveChangesSeed(seedUser.Id);
            }
            else
            {
                Console.WriteLine("Users already populated");
            }
        }
    }
}
