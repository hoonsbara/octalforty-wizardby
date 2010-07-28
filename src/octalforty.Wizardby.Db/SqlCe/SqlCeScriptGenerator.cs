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
using System.Collections.Generic;
using System.IO;
using System.Text;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Db.SqlServer2000;

namespace octalforty.Wizardby.Db.SqlCe
{
    public class SqlCeScriptGenerator : SqlServer2000ScriptGenerator
    {
        public SqlCeScriptGenerator(IDbStatementBatchWriter statementBatchWriter) : 
            base(statementBatchWriter)
        {
        }

        public override void Visit(IAddTableNode addTableNode)
        {
            TextWriter.WriteLine("create table {0} (", Platform.Dialect.EscapeIdentifier(addTableNode.Name));

            List<string> columnDefinitions = new List<string>();
            foreach (IAddColumnNode addColumnNode in (Filter<IAddColumnNode>(addTableNode.ChildNodes)))
            {
                string columnDefinition = GetColumnDefinition(addColumnNode);
                columnDefinitions.Add(columnDefinition);
            } // foreach

            TextWriter.WriteLine(string.Join("," + System.Environment.NewLine, columnDefinitions.ToArray()));
            TextWriter.WriteLine(");");
        }

        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            StringBuilder createIndexBuilder = new StringBuilder("create ");
            
            if(addIndexNode.Unique ?? false)
                createIndexBuilder.Append("unique ");

            //
            // Clusteded keys are not supported on SQL CE

            createIndexBuilder.AppendFormat("index {0} on {1} ({2});",
                Platform.Dialect.EscapeIdentifier(addIndexNode.Name),
                Platform.Dialect.EscapeIdentifier(addIndexNode.Table),
                Join(", ", GetIndexColumns(addIndexNode.Columns)));

            TextWriter.WriteLine(createIndexBuilder.ToString());
        }

        /// <summary>
        /// Visits the given <paramref name="removeIndexNode"/>.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            TextWriter.WriteLine("drop index {0}.{1};",
                Platform.Dialect.EscapeIdentifier(removeIndexNode.Table),
                Platform.Dialect.EscapeIdentifier(removeIndexNode.Name));
        }
    }
}
