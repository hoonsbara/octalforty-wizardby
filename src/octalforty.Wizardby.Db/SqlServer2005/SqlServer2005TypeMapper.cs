using System.Data;

using octalforty.Wizardby.Db.SqlServer2000;

namespace octalforty.Wizardby.Db.SqlServer2005
{
    public class SqlServer2005TypeMapper : SqlServer2000TypeMapper
    {
        public SqlServer2005TypeMapper()
        {
            RegisterTypeMapping(DbType.Xml, "xml");
        }
    }
}
