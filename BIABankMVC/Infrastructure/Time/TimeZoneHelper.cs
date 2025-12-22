using System.Globalization;

namespace BankingSystemMVC.Infrastructure.Time
{
    public class TimeZoneHelper
    {
        public static string ConvertUtcToLocal(DateTime utcTime, string? timeZoneId,
            string format = "MM-dd-yyyy hh:mm tt")
        {
            if (utcTime.Kind != DateTimeKind.Utc)
                utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);

            TimeZoneInfo tz;

            try
            {
                if (string.IsNullOrWhiteSpace(timeZoneId))
                {
                    tz = TimeZoneInfo.Local;
                }
                else
                {
                    tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                }
            }
            catch (TimeZoneNotFoundException)
            {
                tz = TimeZoneInfo.Local;
            }
            catch (InvalidTimeZoneException)
            {
                tz = TimeZoneInfo.Local;
            }

            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz);
            return localTime.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
