using NUnit.Framework;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Db.Jet;

namespace octalforty.Wizardby.Tests.Db.Jet
{
    [TestFixture()]
    public class JetConnectionStringBuilderTestFixture
    {
        [Test()]
        public void BuildConnectionString()
        {
            IDbConnectionStringBuilder connectionStringBuilder =
                new JetConnectionStringBuilder();

            connectionStringBuilder.AppendKeyValuePair("database", "oxite");

            Assert.AreEqual("data source=oxite.mdb;Provider=Microsoft.Jet.OLEDB.4.0;", connectionStringBuilder.ToString());
        }

        [Test()]
        public void BuildConnectionString2()
        {
            //
            // Wikipedia says that ".sdf" extension is not mandatory and
            // can be changed to anything
            IDbConnectionStringBuilder connectionStringBuilder =
                new JetConnectionStringBuilder();

            connectionStringBuilder.AppendKeyValuePair("database", "d:\\db\\oxite.db");

            Assert.AreEqual("data source=d:\\db\\oxite.db.mdb;Provider=Microsoft.Jet.OLEDB.4.0;", connectionStringBuilder.ToString());
        }
    }
}
