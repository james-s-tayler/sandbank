using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core;
using Entities.Domain.Transactions;
using Entities.System.Users;

namespace Entities.Domain.Accounts
{
    public class Account : DomainEntity<int>
    {
        [Required]
        public AccountType AccountType { get; set; }

        [Required]
        //add unique constraint and dont allow user to pass this in, generate ourselves
        [StringLength(25)]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }
        
        public User AccountOwner { get; set; }
        public int AccountOwnerId { get; set; }
        
        public List<Transaction> AccountTransactions { get; set; } = new List<Transaction>();

        public void PostTransaction(Transaction transaction)
        {
            transaction.Account = this;
            AccountTransactions.Add(transaction);
        }
    }
}