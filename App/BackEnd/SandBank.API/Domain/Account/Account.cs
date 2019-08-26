using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core;

namespace Domain.Account
{
    public class Account : DomainEntity<int>
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
        
        public User.User AccountOwner { get; set; }
        public int AccountOwnerId { get; set; }
        
        public List<Transaction.Transaction> AccountTransactions { get; set; } = new List<Transaction.Transaction>();
    }
}