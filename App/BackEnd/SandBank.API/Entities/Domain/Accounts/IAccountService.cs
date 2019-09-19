namespace Entities.Domain.Accounts
{
    public interface IAccountService
    {
        Account OpenAccount(OpenAccountRequest openAccountRequest);
    }
}