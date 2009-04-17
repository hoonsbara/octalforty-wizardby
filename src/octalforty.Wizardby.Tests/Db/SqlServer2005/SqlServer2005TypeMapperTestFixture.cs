using System.Data;

using NUnit.Framework;

using octalforty.Wizardby.Db.SqlServer2005;
using octalforty.Wizardby.Tests.Db.SqlServer2000;

namespace octalforty.Wizardby.Tests.Db.SqlServer2005
{
    [TestFixture()]
    public class SqlServer2005TypeMapperTestFixture
    {
        private SqlServer2005TypeMapper typeMapper;

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            typeMapper = new SqlServer2005TypeMapper();
        }

        [Test()]
        public void MapToNativeType()
        {
            SqlServer2000TypeMapperTestFixture testFixture = 
                new SqlServer2000TypeMapperTestFixture(typeMapper);
            testFixture.MapToNativeType();

            Assert.AreEqual("xml", typeMapper.MapToNativeType(DbType.Xml, null));
        }

        [Test()]
        public void MapToDbType()
        {
            SqlServer2000TypeMapperTestFixture testFixture =
                new SqlServer2000TypeMapperTestFixture(typeMapper);
            testFixture.MapToDbType();

            Assert.AreEqual(DbType.Xml, typeMapper.MapToDbType("xml", null));
        }
    }
}
