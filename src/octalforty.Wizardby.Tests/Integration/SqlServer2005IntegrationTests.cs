using NUnit.Framework;
using octalforty.Wizardby.Db.SqlServer2005;

namespace octalforty.Wizardby.Tests.Integration
{
    [TestFixture()]
    public class SqlServer2005IntegrationTests : DbPlatformIntegrationTestsBase<SqlServer2005Platform>
    {
        protected override string GetConnectionString()
        {
            var connectionStringBuilder = new SqlServer2005ConnectionStringBuilder();
            connectionStringBuilder.AppendKeyValuePair(
                "database", "integration");
            connectionStringBuilder.AppendKeyValuePair(
                "server", "(local)\\sqlexpress");
            connectionStringBuilder.AppendKeyValuePair(
                "integrated-security", "true");

            return connectionStringBuilder.ToString();
        }
    }
}
