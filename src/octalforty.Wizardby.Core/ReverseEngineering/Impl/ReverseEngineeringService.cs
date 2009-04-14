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
                    SemanticModelUtil.Copy(column, addColumnNode);
                    AstUtil.CopyToProperties(addColumnNode);

                    addTableNode.ChildNodes.Add(addColumnNode);
                } // foreach

                baselineNode.ChildNodes.Add(addTableNode);
            } // foreach

            return baselineNode;
        }
    }
}
