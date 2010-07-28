using System;
using System.IO;

using NUnit.Framework;

using octalforty.Wizardby.Db.SQLite;

namespace octalforty.Wizardby.Tests.Integration
{
    [TestFixture()]
    public class SQLiteIntegrationTests : DbPlatformIntegrationTestsBase<SQLitePlatform>
    {
        protected override string GetConnectionString()
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.AppendKeyValuePair(
                "database", 
                Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "sqlite.db"));

            return connectionStringBuilder.ToString();
        }
    }
}