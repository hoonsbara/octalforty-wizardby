using System.IO;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SQLite
{
    public class SQLiteScriptGenerator : AnsiDbScriptGeneratorBase
    {
        public SQLiteScriptGenerator(TextWriter textWriter) : 
            base(textWriter)
        {
        }
    }
}
 