
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Enum
{
    public enum Days
    {
        //[JsonStringEnumMemberName("السبت")]
        Saturday = 1,

        //[JsonStringEnumMemberName("الأحد")]
        Sunday = 2,

        //[JsonStringEnumMemberName("الاثنين")]
        Monday = 3,

        //[JsonStringEnumMemberName("الثلاثاء")]
        Tuesday = 4,

        //[JsonStringEnumMemberName("الأربعاء")]
        Wednesday = 5,

        //[JsonStringEnumMemberName("الخميس")]
        Thursday = 6,

        //[JsonStringEnumMemberName("الجمعة")]
        Friday = 7
    }
}
