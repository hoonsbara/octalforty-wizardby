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
using System.Data;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.SemanticModel;

using octalforty.Wizardby.Tests.Core.Compiler.Impl;

namespace octalforty.Wizardby.Tests.Core.Compiler.Ast
{
    [TestFixture()]
    public class AstBuilderTestFixture : AstTestFixtureBase
    {
        #region Private Fields
        private SchemaDefinition schemaDefinition;
        private IndexDefinition uqNameIndex;
        #endregion

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            schemaDefinition = new SchemaDefinition();

            TableDefinition barTable = new TableDefinition("Bar");
            barTable.AddColumn(new ColumnDefinition("ID", "Bar", DbType.Int32, false, null, null, null, true, false));
            barTable.AddColumn(new ColumnDefinition("Name", DbType.String, false, 1000, null, null));
            barTable.AddColumn(new ColumnDefinition("ParentID", DbType.Int32, true, null, null, null));

            uqNameIndex = new IndexDefinition("UQ_Name", new IndexColumnDefinition("Name", SortDirection.Ascending));
            uqNameIndex.Unique = true;
            uqNameIndex.Clustered = true;
            barTable.AddIndex(uqNameIndex);

            ReferenceDefinition fkBarReference = new ReferenceDefinition("FK_Bar", "Bar", "Bar");
            fkBarReference.PkColumns.Add("ID");
            fkBarReference.FkColumns.Add("ParentID");

            schemaDefinition.AddTable(barTable);
        }

        [Test()]
        public void BuildAst()
        {
            AstBuilder astBuilder = new AstBuilder();
            IAstNode astNode = astBuilder.BuildAst(schemaDefinition, new BaselineNode(null));

            Assert.IsInstanceOfType(typeof(IBaselineNode), astNode);
            AssertAddTable(astNode.ChildNodes[0], "Bar",
                schemaDefinition.Tables[0].Columns[0],
                schemaDefinition.Tables[0].Columns[1],
                schemaDefinition.Tables[0].Columns[2]);

            AssertAddIndex(astNode.ChildNodes[1], uqNameIndex.Name, uqNameIndex.Clustered, uqNameIndex.Unique, 
                new List<IIndexColumnDefinition>(uqNameIndex.Columns).ToArray());
        }
    }
}
