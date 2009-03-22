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
using System.Data;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Refactoring
{
    [Refactoring("Add Audit Table", "add-audit-table")]
    public class AddAuditTableRefactoring : IRefactoring
    {
        public void Execute(IDbPlatform platform, IRefactorNode refactorNode, Environment environment)
        {
            string auditTableName = refactorNode.Properties["audit-table"].Value.ToString();
            string tableName = refactorNode.Properties["table"].Value.ToString();
            //
            // We only need columns here
            ITableDefinition tableDefinition = environment.Schema.GetTable(tableName);

            ITableDefinition auditTable = new TableDefinition(auditTableName);
            foreach(ColumnDefinition column in tableDefinition.Columns)
            {
                //
                // Keep name and type only.
                IColumnDefinition auditColumn = new ColumnDefinition(column.Name);
                
                //
                // Special handling for rowversion
                if(column.Type == DbType.Time)
                {
                    auditColumn.Length = 8;
                    auditColumn.Type = DbType.Binary;
                } // if
                else
                {
                    auditColumn.Length = column.Length;
                    auditColumn.Type = column.Type;
                } // else

                auditColumn.Nullable = true;

                auditTable.AddColumn(auditColumn);
            } // foreach

            environment.Schema.AddTable(auditTable);

            //
            // Now build an AST
            IAddTableNode addAuditTableNode = new AddTableNode(refactorNode.Parent, auditTableName);
            foreach(IColumnDefinition auditColumn in auditTable.Columns)
            {
                IAddColumnNode addAuditColumnNode = new AddColumnNode(addAuditTableNode, auditColumn.Name);
                addAuditColumnNode.Type = auditColumn.Type;
                addAuditColumnNode.Length = auditColumn.Length;
                addAuditColumnNode.Nullable = auditColumn.Nullable;

                addAuditTableNode.ChildNodes.Add(addAuditColumnNode);
            } // foreach

            addAuditTableNode.Parent.ChildNodes.Add(addAuditTableNode);
        }
    }
}
