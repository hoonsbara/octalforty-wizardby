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
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.SemanticModel;
using octalforty.Wizardby.Tests.Core.Compiler.Impl;

namespace octalforty.Wizardby.Tests.Core.Compiler.Ast
{
    [TestFixture()]
    public class AstUtilTestFixture : AstTestFixtureBase
    {
        [Test()]
        public void BuildAstFromSchema()
        {
            Schema schema = GetSchema();
            IAstNode astNode = AstUtil.BuildAstNodeFromSchema(new BaselineNode(null), schema);

            Assert.IsInstanceOfType(typeof(IBaselineNode), astNode);

            AssertAddTable(astNode.ChildNodes[0],
                "SchemaInfo",
                new ColumnDefinition("Version", "SchemaInfo", DbType.Int64, false, null, null, null, null, null));

            AssertAddIndex(astNode.ChildNodes[1], 
                null, null, true,
                new IndexColumnDefinition("Version"));

            AssertAddTable(astNode.ChildNodes[2],
                "Author",
                new ColumnDefinition("ID", "Author", DbType.Int32, false, null, null, null, true, true),
                new ColumnDefinition("FirstName", "Author", DbType.String, false, 200, null, null, null, null),
                new ColumnDefinition("LastName", "Author", DbType.String, false, 200, null, null, null, null),
                new ColumnDefinition("EmailAddress", "Author", DbType.String, false, 200, null, null, null, null),
                new ColumnDefinition("Login", "Author", DbType.String, false, 200, null, null, null, null),
                new ColumnDefinition("Password", "Author", DbType.Binary, true, 64, null, null, null, null));

            AssertAddIndex(astNode.ChildNodes[3],
                "IX_EmailAddress", null, true,
                new IndexColumnDefinition("EmailAddress"));

            AssertAddIndex(astNode.ChildNodes[4], 
                "IX_Login", null, true,
                new IndexColumnDefinition("Login"));

            AssertAddReference(astNode.ChildNodes[7],
                "FK1", 
                "Blog", new string[] { "ID" }, "BlogPost", new string[] { "BlogID" } );

            AssertAddReference(astNode.ChildNodes[8],
                "FK2",
                "Author", new string[] { "ID" }, "BlogPost", new string[] { "AuthorID" });
        }

        [Test()]
        public void CloneAddColumnNode()
        {
            IBaselineNode baselineNode = new BaselineNode(null);
            IAddColumnNode originalNode = new AddColumnNode(baselineNode, "Add Column");
            originalNode.Identity = true;
            originalNode.Length = 321;
            originalNode.Nullable = true;
            originalNode.Precision = 32;
            originalNode.PrimaryKey = true;
            originalNode.Scale = 55;
            originalNode.Table = "Qux";
            originalNode.Type = DbType.Boolean;
            
            IAddColumnNode clonedNode = AstUtil.Clone(originalNode);

            Assert.AreSame(originalNode.Parent, clonedNode.Parent);
            Assert.AreEqual(originalNode.Name, clonedNode.Name);
            Assert.AreEqual(originalNode.Identity, clonedNode.Identity);
            Assert.AreEqual(originalNode.Length, clonedNode.Length);
            Assert.AreEqual(originalNode.Nullable, clonedNode.Nullable);
            Assert.AreEqual(originalNode.Precision, clonedNode.Precision);
            Assert.AreEqual(originalNode.PrimaryKey, clonedNode.PrimaryKey);
            Assert.AreEqual(originalNode.Scale, clonedNode.Scale);
            Assert.AreEqual(originalNode.Type, clonedNode.Type);
            Assert.AreEqual(originalNode.Table, clonedNode.Table);
        }

        [Test()]
        public void CopyColumnProperties()
        {
            IColumnDefinition columnDefinition = new ColumnDefinition("Foo", null, DbType.Binary, true, 234, 56, 78, true, true);
            
            IColumnNode columnNode = new AddColumnNode(null, "Foo");
            AstUtil.CopyProperties(columnDefinition, columnNode);

            Assert.AreEqual("true", AstNodePropertyUtil.AsString(columnNode.Properties[MdlSyntax.Identity].Value));
            Assert.AreEqual(234, AstNodePropertyUtil.AsInteger(columnNode.Properties[MdlSyntax.Length].Value));
            Assert.AreEqual("true", AstNodePropertyUtil.AsString(columnNode.Properties[MdlSyntax.Nullable].Value));
            Assert.AreEqual(78, AstNodePropertyUtil.AsInteger(columnNode.Properties[MdlSyntax.Precision].Value));
            Assert.AreEqual("true", AstNodePropertyUtil.AsString(columnNode.Properties[MdlSyntax.PrimaryKey].Value));
            Assert.AreEqual(56, AstNodePropertyUtil.AsInteger(columnNode.Properties[MdlSyntax.Scale].Value));
            Assert.AreEqual("Binary", AstNodePropertyUtil.AsString(columnNode.Properties[MdlSyntax.Type].Value));
        }

        private Schema GetSchema()
        {
            Environment environment = new Environment();

            using (Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Blog.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(
                                                         new StreamReader(resourceStream, Encoding.UTF8)));

                IMdlCompiler mdlCompiler = new MdlCompiler(new NullCodeGenerator(), environment);
                mdlCompiler.Compile(mdlParser.Parse(), MdlCompilationOptions.All);
            } // using
            
            return environment.Schema;
        }
    }
}
