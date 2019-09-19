using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Entities.Domain.Transactions;

namespace Services.Domain.Accounts
{
    public interface ISeedTransactionDataService
    {
        IList<Transaction> ReadFromFile();
        IList<Transaction> ReadFromFormPost(IFormFile formFile);
    }
}
