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
using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Compiler.Ast
{
    /// <summary>
    /// Builds an Abstract Syntax Tree which reflects a <see cref="SchemaDefinition"/>.
    /// </summary>
    public class AstBuilder
    {
        /// <summary>
        /// Builds an AST tree for <paramref name="schemaDefinition"/> and uses <paramref name="parent"/>
        /// as a parent for the AST.
        /// </summary>
        /// <param name="schemaDefinition"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IAstNode BuildAst(SchemaDefinition schemaDefinition, IAstNode parent)
        {
            foreach(ITableDefinition table in schemaDefinition.Tables)
            {
                BuildAddTableNode(table, parent);

                foreach(IIndexDefinition index in table.Indexes)
                    BuildAddIndexNode(index, parent);
            } // foreach

            return parent;
        }

        private void BuildAddTableNode(ITableDefinition table, IAstNode parent)
        {
            IAddTableNode addTableNode = new AddTableNode(parent, table.Name);
            parent.ChildNodes.Add(addTableNode);

            foreach(IColumnDefinition column in table.Columns)
            {
                BuildAddColumnNode(column, addTableNode);
            } // foreach
        }

        private void BuildAddColumnNode(IColumnDefinition column, IAddTableNode parent)
        {
            IAddColumnNode addColumnNode = new AddColumnNode(parent, column.Name);
            parent.ChildNodes.Add(addColumnNode);

            SemanticModelUtil.Copy(column, addColumnNode);
        }

        private void BuildAddIndexNode(IIndexDefinition index, IAstNode parent)
        {
            IAddIndexNode addIndexNode = new AddIndexNode(parent, index.Name);
            parent.ChildNodes.Add(addIndexNode);

            SemanticModelUtil.Copy(index, addIndexNode);
        }
    }
}
