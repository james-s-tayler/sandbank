using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Core;

namespace Domain.Account
{
    public class Account : DomainEntity<int>
    {
        [Required]
        public AccountType AccountType { get; set; }

        [Required]
        //should add a uniq constraint to this
        [StringLength(25)]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }
        
        public User.User AccountOwner { get; set; }
        public int AccountOwnerId { get; set; }
        
        public List<Transaction.Transaction> AccountTransactions { get; set; } = new List<Transaction.Transaction>();

        public void PostTransaction(Transaction.Transaction transaction)
        {
            transaction.Account = this;
            AccountTransactions.Add(transaction);
        }
    }
}