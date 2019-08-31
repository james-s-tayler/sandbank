using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domain.Account
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountType
    {
        TRANSACTION,
        SAVING
    }
}