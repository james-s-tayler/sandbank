using System;
using System.ComponentModel.DataAnnotations;
using Core;

namespace Domain.Transaction
{
    public class TransactionViewModel : ViewModel<Transaction, long>
    {
        
        [Required]
        public decimal Amount { get; }
        [Required]
        public DateTime TransactionTimeUtc { get; }
        [StringLength(25)]
        public string TransactionType { get; }
        [StringLength(50)]
        public string MerchantName { get; }
        [StringLength(200)]
        public string Description { get; }
        
        public TransactionViewModel(Transaction transaction) : base(transaction)
        {
            Amount = transaction.Amount;
            TransactionTimeUtc = transaction.TransactionTimeUtc;
            TransactionType = transaction.TransactionType;
            MerchantName = transaction.MerchantName;
            Description = transaction.Description;
        }
    }
}