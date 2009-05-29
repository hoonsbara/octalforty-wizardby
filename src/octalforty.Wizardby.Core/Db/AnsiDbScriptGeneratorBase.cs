using System.IO;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Db
{
    public abstract class AnsiDbScriptGeneratorBase : DbScriptGeneratorBase
    {
        protected AnsiDbScriptGeneratorBase(TextWriter textWriter) : 
            base(textWriter)
        {
        }

        #region DbScriptGeneratorBase Members
        public override void Visit(IAddTableNode addTableNode)
        {
            ITableDefinition table = Environment.Schema.GetTable(addTableNode.Name);
            TextWriter.WriteLine("create table {0} (", Platform.Dialect.EscapeIdentifier(table.Name));

            bool firstColumn = true;

            foreach(IColumnDefinition column in table.Columns)
            {
                if(firstColumn)
                    firstColumn = false;
                else
                    TextWriter.WriteLine(",");

                TextWriter.Write("{0} {1} {2}", 
                    Platform.Dialect.EscapeIdentifier(column.Name),
                    MapToNativeType(column),
                    column.Nullable.HasValue ?
                        column.Nullable.Value ? "null" : "not null" :
                        "");
            } // foreach

            TextWriter.WriteLine(");");
        }
        #endregion
    }
}
