using System;
using System.ComponentModel.DataAnnotations;
using Core;
using Entities.Domain.Accounts;

namespace Entities.Domain.Transactions
{
    public class Transaction : DomainEntity<long>
    {
        
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime TransactionTimeUtc { get; set; }
        [StringLength(25)]
        public string TransactionType { get; set; }
        [StringLength(50)]
        public string MerchantName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
       
        [Required]
        public Account Account { get; set; }
        public int AccountId { get; set; }
    }
}