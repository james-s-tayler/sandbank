using System.ComponentModel.DataAnnotations;
using Core;

namespace Domain.Account
{
    public class AccountViewModel : ViewModel<Account, int>
    {
        [Required]
        [StringLength(50)]
        public string AccountType { get; }

        [Required]
        [StringLength(25)]
        public string AccountNumber { get; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; }
        public int AccountOwnerId { get; }
        
        public AccountViewModel(Account account) : base(account)
        {
            AccountType = account.AccountType;
            AccountNumber = account.AccountNumber;
            DisplayName = account.DisplayName;
            AccountOwnerId = account.AccountOwnerId;
        }
    }
}