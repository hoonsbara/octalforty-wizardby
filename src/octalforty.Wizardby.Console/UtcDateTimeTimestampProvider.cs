using System;

namespace octalforty.Wizardby.Console
{
    public class UtcDateTimeTimestampProvider : ITimestampProvider
    {
        public long GetTimestamp()
        {
            DateTime currentUtcDateTime = DateTime.Now.ToUniversalTime();
            return long.Parse(currentUtcDateTime.ToString("yyyyMMddHHmmss"));
        }
    }
}
