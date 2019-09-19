using CsvHelper;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entities.Domain.Transactions;

namespace Services.Domain.Accounts
{
    public class SeedTransactionDataService : ISeedTransactionDataService
    {
        private string SEED_FILE = "seed-transactions.csv";

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
