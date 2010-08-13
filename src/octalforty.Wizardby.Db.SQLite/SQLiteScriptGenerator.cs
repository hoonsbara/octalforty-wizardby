#region The MIT License
// The MIT License
// 
// Copyright (c) 2009 octalforty studios
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Db.SQLite
{
    public class SQLiteScriptGenerator : AnsiDbScriptGeneratorBase
    {
        public SQLiteScriptGenerator(IDbStatementBatchWriter statementBatchWriter) : 
            base(statementBatchWriter)
        {
        }

        #region AnsiDbScriptGeneratorBase Members
        public override void Visit(IAddTableNode addTableNode)
        {
            /*var table = Environment.Schema.GetTable(addTableNode.Name);
            if(table == null)
                throw new MigrationException(string.Format("Could not resolve table '{0}' (at {1})",
                    addTableNode.Name, addTableNode.Location));

            TextWriter.WriteLine("create table {0} (", Platform.Dialect.EscapeIdentifier(table.Name));

            bool firstColumn = true;

            foreach(IColumnDefinition column in table.Columns)
            {
                if(firstColumn)
                    firstColumn = false;
                else
                    TextWriter.WriteLine(",");

                TextWriter.Write("{0} ", 
                    Platform.Dialect.EscapeIdentifier(column.Name));

                if((column.Identity.HasValue && column.Identity.Value) && (column.PrimaryKey.HasValue && column.PrimaryKey.Value))
                    TextWriter.Write("integer primary key autoincrement ");
                else
                    TextWriter.Write("{0} ", MapToNativeType(column));

                TextWriter.Write(
                        column.Nullable.HasValue ?
                            column.Nullable.Value ? 
                                "null" : 
                                "not null" :
                            "");
            } // foreach

            TextWriter.WriteLine(");");*/
        }

        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            TextWriter.WriteLine("drop index {0};",
                Platform.Dialect.EscapeIdentifier(removeIndexNode.Name));
        }

        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
        }
        #endregion

    }
}
 