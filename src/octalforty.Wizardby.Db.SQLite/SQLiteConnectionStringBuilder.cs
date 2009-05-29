using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SQLite
{
    public class SQLiteConnectionStringBuilder : DbConnectionStringBuilderBase
    {
        public SQLiteConnectionStringBuilder() :
            base(true)
        {
            RegisterKeyMapping("database", "data source");
        }
    }
}
