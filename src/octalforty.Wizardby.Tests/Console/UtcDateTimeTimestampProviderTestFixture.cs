using System;

using NUnit.Framework;

using octalforty.Wizardby.Console;

namespace octalforty.Wizardby.Tests.Console
{
    [TestFixture()]
    public class UtcDateTimeTimestampProviderTestFixture
    {
        [Test()]
        public void GetTimestamp()
        {
            ITimestampProvider timestampProvider = new UtcDateTimeTimestampProvider();
            long timestamp = timestampProvider.GetTimestamp();
            long utcTimestamp = long.Parse(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));

            Assert.IsTrue(utcTimestamp - timestamp < 2, "{0} returned {1}, expected {2}", 
                timestampProvider.GetType().Name, timestamp, utcTimestamp);
        }
    }
}
