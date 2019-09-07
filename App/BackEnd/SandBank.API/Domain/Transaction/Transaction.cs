using System;
using System.ComponentModel.DataAnnotations;
using Core;

namespace Domain.Transaction
{
    [EventTopic("OutboundTransactionTopic")]
    public class Transaction : DomainEntity<long>
    {
        
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime TransactionTimeUtc { get; set; }
        [StringLength(25)]
        public string TransactionCategory { get; set; }
        [StringLength(25)]
        public string TransactionClassification { get; set; }
        [StringLength(50)]
        public string MerchantName { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
       
        [Required]
        public Account.Account Account { get; set; }
        public int AccountId { get; set; }
    }
}