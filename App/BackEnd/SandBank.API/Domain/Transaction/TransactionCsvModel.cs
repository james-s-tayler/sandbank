using System;
using System.Globalization;

namespace Domain.Transaction
{
    public class TransactionCsvModel
    {
        public string Type { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public Transaction ConvertToTransaction()
        {
            return new Transaction
            {
                Amount = Amount,
                TransactionTimeUtc = TryParseDate(Date),
                Description = Description,
                TransactionType = Type
            };
        }

        private DateTime TryParseDate(string input)
        {
            DateTime result;
            var success = DateTime.TryParseExact(input, "dd/MM/yy", null, DateTimeStyles.None, out result);
            if (success == false)
            {
                var success2 = DateTime.TryParseExact(input, "d/MM/yy", null, DateTimeStyles.None, out result);
                if (success2 == false)
                {
                    throw new Exception();
                }
            }

            return result;
        }
    }
}