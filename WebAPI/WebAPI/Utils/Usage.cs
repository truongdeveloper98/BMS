using System;

namespace WebAPI.Utils
{
    public  class Usage
    {
        public static DateTime? Date2Utc(DateTime? time)
        {
            if (time.HasValue)
            {
                var d = (DateTime)time;
                return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, DateTimeKind.Utc);
            }

            return null;
        }
    }
}