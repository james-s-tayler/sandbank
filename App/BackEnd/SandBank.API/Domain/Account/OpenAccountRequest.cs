using System.ComponentModel.DataAnnotations;
using Core;

namespace Domain.Account
{
    public class OpenAccountRequest : CreateModel<Account, int>
    {
        [Required]
        [StringLength(50)]
        public string AccountType { get; set; }

        [Required]
        [StringLength(25)]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }

        public override Account ToDomainModel()
        {
            return new Account
            {
                AccountNumber = AccountNumber,
                AccountType = AccountType,
                DisplayName = DisplayName
            };
        }
    }
}