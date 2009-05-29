using System;
using System.Collections.Generic;
using System.Text;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SQLite
{
    public class SQLiteDialect : DbDialectBase
    {
        public override IDbScriptGenerator CreateScriptGenerator(System.IO.TextWriter textWriter)
        {
            SQLiteScriptGenerator scriptGenerator = new SQLiteScriptGenerator(textWriter);
            scriptGenerator.Platform = Platform;

            return scriptGenerator;
        }
    }
}
