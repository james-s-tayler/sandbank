using System;
using System.Threading.Tasks;
using Endpoints.Data;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Configuration
{
    //probably much better off using a database serial for this
    public class NumberRangeService : INumberRangeService
    {
        private readonly SandBankDbContext _db;

        public NumberRangeService(SandBankDbContext db) => _db = db;

        private async Task<NumberRange> CreateDefault(NumberRangeType rangeType)
        {
            var numberRange = new NumberRange
            {
                RangeType = rangeType
            };
            //make configurable
            switch (rangeType)
            {
                case NumberRangeType.Account:
                    numberRange.Prefix = "01";
                    break;
                case NumberRangeType.Transaction:
                    numberRange.Prefix = "TXN";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rangeType), rangeType, null);
            }

            await _db.NumberRanges.AddAsync(numberRange);
            await _db.SaveChangesAsync();
            
           return numberRange;
        }

        public async Task<string> GetNextValue(NumberRangeType rangeType)
        {
            var nextValue = await GetLastValue(rangeType) + 1;
            while (await Exists(nextValue, rangeType))
            {
                nextValue = await GetLastValue(rangeType) + 1;
            }

            var numberRange = await GetNumberRange(rangeType);
            numberRange.LastValue = nextValue;
            await _db.SaveChangesAsync();

            var numZeros = numberRange.RangeEnd.ToString().Length - nextValue.ToString().Length;
            var paddedNextVal = nextValue.ToString($"D{numZeros}");
            
            return $"{numberRange.Prefix}-{paddedNextVal}";
        }

        private async Task<bool> Exists(int value, NumberRangeType rangeType)
        {
            return await _db.NumberRanges.AnyAsync(range => range.RangeType == rangeType && range.LastValue == value);
        }

        private async Task<int> GetLastValue(NumberRangeType rangeType)
        {
            var numberRange = await GetNumberRange(rangeType);
            return numberRange.LastValue ;
        }

        private async Task<NumberRange> GetNumberRange(NumberRangeType rangeType)
        {
            var numberRange = await _db.NumberRanges.SingleOrDefaultAsync(range => range.RangeType == rangeType);

            if (numberRange == null)
            {
                numberRange = await CreateDefault(rangeType);
            }

            return numberRange;
        }
    }
}