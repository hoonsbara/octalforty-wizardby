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
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Db
{
    public abstract class AnsiDbScriptGeneratorBase : DbScriptGeneratorBase
    {
        protected AnsiDbScriptGeneratorBase(IDbStatementBatchWriter statementBatchWriter) : 
            base(statementBatchWriter)
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

        /// <summary>
        /// Visits the given <paramref name="executeNativeSqlNode"/>.
        /// </summary>
        /// <param name="executeNativeSqlNode"></param>
        public override void Visit(IExecuteNativeSqlNode executeNativeSqlNode)
        {
            IVersionNode versionNode = TraverseToParent<IVersionNode>(executeNativeSqlNode);

            string resourceName = MigrationMode == MigrationMode.Upgrade ?
                executeNativeSqlNode.UpgradeResource :
                executeNativeSqlNode.DowngradeResource;

            if(string.IsNullOrEmpty(resourceName))
                return;

            string[] nativeSqlResources = MigrationMode == MigrationMode.Upgrade ?
                NativeSqlResourceProvider.GetUpgradeResources(Platform, resourceName, versionNode.Number) :
                NativeSqlResourceProvider.GetDowngradeResources(Platform, resourceName, versionNode.Number);

            if(nativeSqlResources == null || nativeSqlResources.Length == 0)
                return;

            StatementBatchWriter.EndBatch();
            foreach(string nativeSqlResource in nativeSqlResources)
            {
                TextWriter.Write(nativeSqlResource);
                StatementBatchWriter.EndBatch();
            } // foreach

            StatementBatchWriter.EndBatch();
        }

        #region Overridables
        protected virtual bool SupportsClustededIndexes
        {
            get { return true; }
        }
        #endregion


        #region AnsiDbScriptGeneratorBase Members
        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            StringBuilder createIndexBuilder = new StringBuilder("create ");

            if (addIndexNode.Unique ?? false)
                createIndexBuilder.Append("unique ");

            if(SupportsClustededIndexes)
                if(addIndexNode.Clustered ?? false)
                    createIndexBuilder.Append("clustered ");
                else
                    createIndexBuilder.Append("nonclustered ");

            createIndexBuilder.AppendFormat("index {0} on {1} ({2});",
                Platform.Dialect.EscapeIdentifier(addIndexNode.Name),
                Platform.Dialect.EscapeIdentifier(addIndexNode.Table),
                Join(", ", GetIndexColumns(addIndexNode.Columns)));

            TextWriter.WriteLine(createIndexBuilder.ToString());
        }
        #endregion

        protected string Join(string separator, IEnumerable<string> strings)
        {
            StringBuilder joinBuilder = new StringBuilder();

            foreach(string s in strings)
            {
                if(joinBuilder.Length > 0)
                    joinBuilder.Append(separator);

                joinBuilder.Append(s);
            } // if

            return joinBuilder.ToString();
        }

        protected IEnumerable<string> GetIndexColumns(IList<IIndexColumnDefinition> columns)
        {
            foreach(IIndexColumnDefinition indexColumnDefinition in columns)
            {
                if(indexColumnDefinition.SortDirection.HasValue)
                    yield return string.Format("{0} {1}",
                        Platform.Dialect.EscapeIdentifier(indexColumnDefinition.Name),
                        indexColumnDefinition.SortDirection.Value == SortDirection.Ascending ? "asc" : "desc");
                else
                    yield return Platform.Dialect.EscapeIdentifier(indexColumnDefinition.Name);
            } // foreach
        }
        #endregion
    }
}
