
using BenzenyMain.Domain.Enum;

namespace BenzenyMain.Application.Common
{
    public static class WeekDaysHelper
    {
        public static List<Days> FromFlags(WeekDays? flags)
        {
            var list = new List<Days>();
            if (flags == null || flags == WeekDays.None) return list;

            var f = flags.Value;
            if (f.HasFlag(WeekDays.Saturday)) list.Add(Days.Saturday);
            if (f.HasFlag(WeekDays.Sunday)) list.Add(Days.Sunday);
            if (f.HasFlag(WeekDays.Monday)) list.Add(Days.Monday);
            if (f.HasFlag(WeekDays.Tuesday)) list.Add(Days.Tuesday);
            if (f.HasFlag(WeekDays.Wednesday)) list.Add(Days.Wednesday);
            if (f.HasFlag(WeekDays.Thursday)) list.Add(Days.Thursday);
            if (f.HasFlag(WeekDays.Friday)) list.Add(Days.Friday);

            return list;
        }
    }
}
