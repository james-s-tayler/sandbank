namespace Database
{
    public static class SeedData
    {
        public static void EnsureSeedData(SandBankDbContext context)
        {
            /*Console.WriteLine("Seeding database...");
            if (!context.Users.IgnoreQueryFilters().Any())
            {
                Console.WriteLine("Creating User..");
                var seedUser = new User
                {
                    FullName = "John Smith",
                    Address = "1 Queen Street",
                    City = "Auckland",
                    Country = "New Zealand",
                    PostCode = "1010",
                    DateOfBirth = DateTime.UtcNow.AddYears(-30),
                    Email = "hello@example.com",
                    Phone = "+642112345678",
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
            }*/
        }
    }
}
