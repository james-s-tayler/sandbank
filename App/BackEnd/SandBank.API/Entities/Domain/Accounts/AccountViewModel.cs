using System.ComponentModel.DataAnnotations;
using Core;

namespace Entities.Domain.Accounts
{
    public class AccountViewModel : ViewModel<Account, int>
    {
        [Required]
        public AccountType AccountType { get; }

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