using System.Threading.Tasks;

namespace Endpoints.Configuration
{
    public interface INumberRangeService
    {
        Task<string> GetNextValue(NumberRangeType rangeType);
    }
}