using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain.Transactions;

namespace Services.Domain.Accounts
{
    public interface ISeedTransactionDataService
    {
        Task SeedData();
        IList<Transaction> ReadFromFile();
        IList<Transaction> ReadFromFormPost(IFormFile formFile);
    }
}
