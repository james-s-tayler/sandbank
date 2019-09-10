using System;
using System.Threading.Tasks;
using Endpoints.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SlxLuhnLibrary;

namespace Endpoints.Configuration
{
    //probably much better off using a database serial for this
    public class NumberRangeService : INumberRangeService
    {
        private readonly SandBankDbContext _db;
        private readonly ILogger<NumberRangeService> _logger;

        public NumberRangeService(SandBankDbContext db, ILogger<NumberRangeService> logger)
        {
            _db = db;
            _logger = logger;
        }

        private async Task<NumberRange> CreateDefault(NumberRangeType rangeType)
        {
            var numberRange = new NumberRange
            {
                RangeType = rangeType
            };
            //make configurable
            switch (rangeType)
            {
                case NumberRangeType.Cheque:
                    numberRange.Prefix = "01";
                    break;
                case NumberRangeType.Savings:
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
            _logger.LogInformation($"GetNextValue({rangeType.ToString()}) -> nextValue={nextValue}");

            var numberRange = await GetNumberRange(rangeType);
            numberRange.LastValue = nextValue;
            await _db.SaveChangesAsync();

            var paddedNextVal = nextValue.ToString($"D{numberRange.RangeEnd.ToString().Length}");
            var nextValWithLuhn = ClsLuhnLibrary.WithLuhn_Base10(paddedNextVal);
            return nextValWithLuhn;
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