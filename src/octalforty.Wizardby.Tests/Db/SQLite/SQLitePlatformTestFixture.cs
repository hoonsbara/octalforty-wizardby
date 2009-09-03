using System.Data.SQLite;

using NUnit.Framework;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Db.SQLite;

namespace octalforty.Wizardby.Tests.Db.SQLite
{
    [TestFixture()]
    public class SQLitePlatformTestFixture
    {
        [Test()]
        public void InitializePlatform()
        {
            IDbPlatform dbPlatform = new SQLitePlatform();
            Assert.IsInstanceOfType(typeof(SQLiteFactory), dbPlatform.ProviderFactory);
        }
    }
}
