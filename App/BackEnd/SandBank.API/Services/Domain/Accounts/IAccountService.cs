using Entities.Domain.Accounts;

namespace Services.Domain.Accounts
{
    public interface IAccountService
    {
        Account GetAccount(int accountId);
    }
}