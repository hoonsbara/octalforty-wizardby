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
