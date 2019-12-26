using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Entities.Domain.Accounts
{
  [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountType
    {
        TRANSACTION,
        SAVING
    }
}