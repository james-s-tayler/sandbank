using System.Threading.Tasks;
using Entities.System.NumberRanges;

namespace Services.System.NumberRange
{
    public interface INumberRangeService
    {
        Task<string> GetNextValue(NumberRangeType rangeType);
    }
}