using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.ReverseEngineering.Impl
{
    public class ReverseEngineeringService : IReverseEngineeringService
    {
        public IAstNode ReverseEngineer(IDbPlatform dbPlatform, string connectionString)
        {
            SchemaDefinition schema = dbPlatform.SchemaProvider.GetSchemaDefinition(connectionString);

            IBaselineNode baselineNode = new BaselineNode(null);

            foreach(ITableDefinition table in schema.Tables)
            {
                IAstNode addTableNode = new AddTableNode(baselineNode, table.Name);

                foreach(IColumnDefinition column in table.Columns)
                {
                    IAddColumnNode addColumnNode = new AddColumnNode(addTableNode, column.Name);
                    AstUtil.Copy(column, addColumnNode);
                    AstUtil.CopyToProperties(addColumnNode);

                    addTableNode.ChildNodes.Add(addColumnNode);
                } // foreach

                baselineNode.ChildNodes.Add(addTableNode);
            } // foreach

            return baselineNode;
        }
    }
}
