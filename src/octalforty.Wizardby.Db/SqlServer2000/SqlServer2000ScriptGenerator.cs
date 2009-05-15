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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Db.SqlServer2000
{
    public class SqlServer2000ScriptGenerator : DbScriptGeneratorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public SqlServer2000ScriptGenerator(TextWriter textWriter) : 
            base(textWriter)
        {
        }

        #region DbScriptGeneratorBase
        /// <summary>
        /// Visits the given <paramref name="addTableNode"/>.
        /// </summary>
        /// <param name="addTableNode"></param>
        public override void Visit(IAddTableNode addTableNode)
        {
            TextWriter.WriteLine("create table {0} (", Platform.Dialect.EscapeIdentifier(addTableNode.Name));

            foreach(IAddColumnNode addColumnNode in (Filter<IAddColumnNode>(addTableNode.ChildNodes)))
            {
                string columnDefinition = GetColumnDefinition(addColumnNode);
                TextWriter.WriteLine("{0},", columnDefinition);
            } // foreach

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

        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            TextWriter.WriteLine("alter table {0} add constraint {1} foreign key ({2}) references {3} ({4});",
                Platform.Dialect.EscapeIdentifier(addReferenceNode.FkTable), 
                Platform.Dialect.EscapeIdentifier(addReferenceNode.Name),
                Join(", ", EscapeIdentifiers(addReferenceNode.FkColumns)),
                Platform.Dialect.EscapeIdentifier(addReferenceNode.PkTable),
                Join(", ", EscapeIdentifiers(addReferenceNode.PkColumns)));
                
        }

        /// <summary>
        /// Visits the given <paramref name="removeTableNode"/>.
        /// </summary>
        /// <param name="removeTableNode"></param>
        public override void Visit(IRemoveTableNode removeTableNode)
        {
            TextWriter.WriteLine("drop table {0};", Platform.Dialect.EscapeIdentifier(removeTableNode.Name));
        }

        /// <summary>
        /// Visits the given <paramref name="removeReferenceNode"/>.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
            TextWriter.WriteLine("alter table {0} drop constraint {1};",
                Platform.Dialect.EscapeIdentifier(removeReferenceNode.Table),
                Platform.Dialect.EscapeIdentifier(removeReferenceNode.Name));
        }

        /// <summary>
        /// Visits the given <paramref name="removeIndexNode"/>.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            TextWriter.WriteLine("drop index {0} on {1};", 
                Platform.Dialect.EscapeIdentifier(removeIndexNode.Name),
                Platform.Dialect.EscapeIdentifier(removeIndexNode.Table));
        }

        /// <summary>
        /// Visits the given <paramref name="alterTableNode"/>.
        /// </summary>
        /// <param name="alterTableNode"></param>
        public override void Visit(IAlterTableNode alterTableNode)
        {
            foreach(IAstNode astNode in alterTableNode.ChildNodes)
            {
                if(astNode is IAddColumnNode)
                {
                    IAddColumnNode addColumnNode = (IAddColumnNode)astNode;
                    TextWriter.WriteLine("alter table {0} add {1};",
                        Platform.Dialect.EscapeIdentifier(alterTableNode.Name), 
                        GetColumnDefinition(addColumnNode));
                } // if
                if(astNode is IRemoveColumnNode)
                {
                    IRemoveColumnNode removeColumnNode = (IRemoveColumnNode)astNode;
                    TextWriter.WriteLine("alter table {0} drop column {1};",
                        Platform.Dialect.EscapeIdentifier(alterTableNode.Name),
                        Platform.Dialect.EscapeIdentifier(removeColumnNode.Name));
                } // if
                if(astNode is IAlterColumnNode)
                {
                    IAlterColumnNode alterColumnNode = (IAlterColumnNode)astNode;
                    TextWriter.WriteLine("alter table {0} alter column {1};",
                        Platform.Dialect.EscapeIdentifier(alterTableNode.Name),
                        GetAlterColumnDefinition(alterColumnNode));
                } // if
            } // foreach
        }

        public override void Visit(IAddConstraintNode addConstraintNode)
        {
            IConstraintDefinition constraint =
                Environment.Schema.GetTable(addConstraintNode.Table).GetConstraint(addConstraintNode.Name);

            if(constraint is IDefaultConstraintDefinition)
            {
                IDefaultConstraintDefinition defaultConstraint = (IDefaultConstraintDefinition)constraint;
                TextWriter.WriteLine("alter table {0} alter column {1} set default {2}",
                    constraint.Table, constraint.Columns[0], defaultConstraint.Default);
            } // if
        }

        public override void Visit(IRemoveConstraintNode removeConstraintNode)
        {
            throw new NotImplementedException();
        }
        #endregion

        private IEnumerable<string> EscapeIdentifiers(IEnumerable<string> identifiers)
        {
            foreach(string identifier in identifiers)
                yield return Platform.Dialect.EscapeIdentifier(identifier);
        }

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

        protected virtual string GetColumnDefinition(IColumnDefinition columnDefinition)
        {
            StringBuilder columnDefinitionBuilder = new StringBuilder();
            columnDefinitionBuilder.AppendFormat("{0} {1} {2}", 
                Platform.Dialect.EscapeIdentifier(columnDefinition.Name),
                MapToNativeType(columnDefinition),
                columnDefinition.Nullable.HasValue ?
                    columnDefinition.Nullable.Value ? "null" : "not null" :
                    "");

            if(columnDefinition.Identity.HasValue && columnDefinition.Identity.Value)
                columnDefinitionBuilder.Append(" identity");

            if(columnDefinition.PrimaryKey.HasValue && columnDefinition.PrimaryKey.Value)
                columnDefinitionBuilder.Append(" primary key");
            else
                columnDefinitionBuilder.Append("");

            if(columnDefinition.Default != null)
                columnDefinitionBuilder.AppendFormat(" default ({0})", columnDefinition.Default);

            return columnDefinitionBuilder.ToString();
        }

        private string GetAlterColumnDefinition(IColumnDefinition columnDefinition)
        {
            StringBuilder columnDefinitionBuilder = new StringBuilder();
            columnDefinitionBuilder.Append(Platform.Dialect.EscapeIdentifier(columnDefinition.Name));

            columnDefinitionBuilder.AppendFormat(" {0}", MapToNativeType(columnDefinition));

            if(columnDefinition.Nullable.HasValue)
                columnDefinitionBuilder.AppendFormat(" {0}", columnDefinition.Nullable.Value ? "null" : "not null");

            return columnDefinitionBuilder.ToString();
        }
    }
}
