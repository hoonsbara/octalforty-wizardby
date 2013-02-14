using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Db.SqlServer2000;

namespace octalforty.Wizardby.Db.SqlServer2008
{
    public class SqlServer2008Dialect : SqlServer2000Dialect
    {
        public override IDbScriptGenerator CreateScriptGenerator(IDbStatementBatchWriter statementBatchWriter)
        {
            var sqlServer2008ScriptGenerator = new SqlServer2008ScriptGenerator(statementBatchWriter);
            sqlServer2008ScriptGenerator.Platform = Platform;

            return sqlServer2008ScriptGenerator;
        }
    }
}