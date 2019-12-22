using System.Threading.Tasks;
using Entities.Domain.Accounts;

namespace Services.Domain.Accounts
{
    public interface IAccountService
    {
        Task<Account> OpenAccount(OpenAccountRequest openAccountRequest, int accountOwnerId);
    }
}