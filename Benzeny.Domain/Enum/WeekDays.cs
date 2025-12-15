
namespace BenzenyMain.Domain.Enum
{
    [Flags]
    public enum WeekDays
    {
        None = 0,
        Saturday = 1 << 0,
        Sunday = 1 << 1,
        Monday = 1 << 2,
        Tuesday = 1 << 3,
        Wednesday = 1 << 4,
        Thursday = 1 << 5,
        Friday = 1 << 6
    }
}
