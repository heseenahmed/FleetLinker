
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Enum
{
    public enum LimitType
    {
        //[JsonStringEnumMemberName("يومي")]
        Daily = 1,
        //[JsonStringEnumMemberName("أسبوعي")]
        Weekly = 2,
        //[JsonStringEnumMemberName("شهري")]
        Monthly = 3,
    }
}
