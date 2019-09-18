using Domain.Transaction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Endpoints.Configuration
{
    public interface ISeedTransactionDataService
    {
        IList<Transaction> ReadFromFile();
        IList<Transaction> ReadFromFormPost(IFormFile formFile);
    }
}
