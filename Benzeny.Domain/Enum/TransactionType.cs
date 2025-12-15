
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Enum
{
    public enum TransactionType
    {
        //[JsonStringEnumMemberName("مرة واحدة")]
        OneTime = 1,
        //[JsonStringEnumMemberName("اكثر من مره")]
        Limit = 2
    }
}
