using System.ComponentModel.DataAnnotations;
using Core;

namespace Entities.Domain.Accounts
{
    public class OpenAccountRequest : CreateModel<Account, int>
    {
        [Required]
        public AccountType AccountType { get; set; }

        public override Account ToDomainModel()
        {
            return new Account
            {
                AccountType = AccountType,
                DisplayName = AccountType.ToString()
            };
        }
    }
}