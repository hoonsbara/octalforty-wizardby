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
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Tests.Core.Compiler
{
    [TestFixture()]
    public class MdlParserTestFixture
    {
        [Test()]
        public void ParsePropertyBlock()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"environment development 
	platform => sqlserver ,	host => ""(local)\sqlexpress""
	database => Waffle
	integrated-authentication => true"));
            IEnvironmentNode environmentNode = (IEnvironmentNode)mdlParser.Parse();

            Assert.AreEqual(new Location(0, 0), environmentNode.Location);

            Assert.AreEqual("sqlserver", AstNodePropertyUtil.AsString(environmentNode.Properties, "platform"));
            Assert.AreEqual("(local)\\sqlexpress", ((IStringAstNodePropertyValue)environmentNode.Properties["host"].Value).Value);
            Assert.AreEqual("Waffle", ((IStringAstNodePropertyValue)environmentNode.Properties["database"].Value).Value);
            Assert.AreEqual("true", ((IStringAstNodePropertyValue)environmentNode.Properties["integrated-authentication"].Value).Value);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsArgumentNullExceptionOnNullScanner()
        {
            IMdlParser mdlParser = new MdlParser(null);  
        }

        [Test()]
        public void ParseComplexFile()
        {
            using(Stream resourceStream = 
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Waffle.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                IAstNode astNode = mdlParser.Parse();
            } // using
        }

        [Test()]
        public void ParseComplexFile2()
        {
            using (Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Blog.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                IAstNode astNode = mdlParser.Parse();
            } // using
        }

        [Test()]
        public void ParseMigrationNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner("migration \"Waffle\" revision => 1, configuration => Debug"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IMigrationNode), astNode);
            Assert.IsNull(astNode.Parent);
            Assert.IsEmpty((ICollection)astNode.ChildNodes);

            IMigrationNode migrationNode = (IMigrationNode)astNode;

            Assert.AreEqual(new Location(0, 0), migrationNode.Location);

            Assert.AreEqual("Waffle", migrationNode.Name);
            
            Assert.AreEqual(1, AstNodePropertyUtil.AsInteger(migrationNode.Properties, "revision"));
            Assert.AreEqual(new Location(0, 19), migrationNode.Properties["revision"].Location);

            Assert.AreEqual("Debug", AstNodePropertyUtil.AsString(migrationNode.Properties, "configuration"));
            Assert.AreEqual(new Location(0, 34), migrationNode.Properties["configuration"].Location);
        }

        [Test()]
        public void ParseMigrationNodeWithoutProperties()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner("migration \"Waffle\""));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IMigrationNode), astNode);
            Assert.IsNull(astNode.Parent);
            Assert.IsEmpty((ICollection)astNode.ChildNodes);
            Assert.AreEqual(0, astNode.Properties.Count);
        }

        [Test()]
        [ExpectedException(typeof(MdlParserException))]
        public void ParseMigrationWithoutName()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner("migration"));
            IAstNode astNode = mdlParser.Parse();
        }

        [Test()]
        [ExpectedException(typeof(MdlParserException))]
        public void ParseMigrationWithoutName2()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner("migration revision => 1"));
            IAstNode astNode = mdlParser.Parse();
        }

        [Test()]
        public void ParseDeploymentNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner("deployment"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IDeploymentNode), astNode);
        }
        
        [Test()]
        public void ParseDeploymentNodeTree()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"deployment:
    environment development 
	    platform => sqlserver ,	host => ""(local)\sqlexpress""
	    database => Waffle, integrated-authentication => true"));

            IAstNode astNode = mdlParser.Parse();

            Assert.AreEqual(new Location(0, 0), astNode.Location);
            Assert.IsInstanceOfType(typeof(IDeploymentNode), astNode);

            Assert.AreEqual(new Location(1, 4), astNode.ChildNodes[0].Location);
            Assert.IsInstanceOfType(typeof(IEnvironmentNode), astNode.ChildNodes[0]);
        }
        
        [Test()]
        public void ParseTypeAliasesNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"type-aliases"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(ITypeAliasesNode), astNode);
        }

        [Test()]
        public void ParseTypeAliasNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"type-alias ndt type => DateTime, nullable => true"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(ITypeAliasNode), astNode);

            ITypeAliasNode typeAliasNode = (ITypeAliasNode)astNode;

            AssertNdtTypeAliasNode(typeAliasNode);
        }

        [Test()]
        public void ParseTypeAliasesNodeTree()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"type-aliases:
    type-alias ndt type => DateTime, nullable => true
    dt type => DateTime, nullable => false"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(ITypeAliasesNode), astNode);
            Assert.IsInstanceOfType(typeof(ITypeAliasNode), astNode.ChildNodes[0]);

            ITypeAliasNode ndtTypeAliasNode = (ITypeAliasNode)astNode.ChildNodes[0];
            ITypeAliasNode dtTypeAliasNode = (ITypeAliasNode)astNode.ChildNodes[1];

            AssertNdtTypeAliasNode(ndtTypeAliasNode);
            AssertDtTypeAliasNode(dtTypeAliasNode);

            Assert.AreSame(astNode, ndtTypeAliasNode.Parent);
            Assert.AreSame(astNode, dtTypeAliasNode.Parent);
        }

        private static void AssertNdtTypeAliasNode(ITypeAliasNode typeAliasNode)
        {
            Assert.AreEqual("ndt", typeAliasNode.Name);
            Assert.AreEqual(2, typeAliasNode.Properties.Count);
            Assert.AreEqual("DateTime", AstNodePropertyUtil.AsString(typeAliasNode.Properties, "type"));
            Assert.AreEqual("true", AstNodePropertyUtil.AsString(typeAliasNode.Properties, "nullable"));
        }

        private static void AssertDtTypeAliasNode(ITypeAliasNode typeAliasNode)
        {
            Assert.AreEqual("dt", typeAliasNode.Name);
            Assert.AreEqual(2, typeAliasNode.Properties.Count);
            Assert.AreEqual("DateTime", AstNodePropertyUtil.AsString(typeAliasNode.Properties, "type"));
            Assert.AreEqual("false", AstNodePropertyUtil.AsString(typeAliasNode.Properties, "nullable"));
        }

        [Test()]
        public void ParseTemplatesNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"templates"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(ITemplatesNode), astNode);
        }

        [Test()]
        public void ParseTemplatesNodeTree()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"templates:
    table template T1"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(ITemplatesNode), astNode);
            
            Assert.IsInstanceOfType(typeof(ITableTemplateNode), astNode.ChildNodes[0]);
            ITableTemplateNode tableTemplateNode = (ITableTemplateNode)astNode.ChildNodes[0];
            Assert.AreEqual("T1", tableTemplateNode.Name);
        }

        [Test()]
        public void ParseTemplatesNodeTree2()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"templates:
    table template T1:
        Foo type => Int32,
        index IX_Foo
        reference FK_Bar"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(ITemplatesNode), astNode);

            Assert.IsInstanceOfType(typeof(ITableTemplateNode), astNode.ChildNodes[0]);
            ITableTemplateNode tableTemplateNode = (ITableTemplateNode)astNode.ChildNodes[0];
            Assert.AreEqual("T1", tableTemplateNode.Name);

            Assert.IsInstanceOfType(typeof(IAddColumnNode), tableTemplateNode.ChildNodes[0]);
            Assert.IsInstanceOfType(typeof(IAddIndexNode), tableTemplateNode.ChildNodes[1]);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), tableTemplateNode.ChildNodes[2]);
        }

        [Test()]
        public void ParseBaselineNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"baseline"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IBaselineNode), astNode);
        }

        [Test()]
        public void ParseBaselineNodeTree()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"baseline:
    add table User
    table Role
    table UserRoleJunction"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IBaselineNode), astNode);
            Assert.AreEqual(3, astNode.ChildNodes.Count);

            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode.ChildNodes[0]);
            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode.ChildNodes[1]);
            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode.ChildNodes[2]);
        }

        [Test()]
        [ExpectedException(typeof(MdlParserException))]
        public void ParseVersionNodeWithoutVersionNumber()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner("version"));
            IAstNode astNode = mdlParser.Parse();
        }

        [Test()]
        public void ParseAddTableNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"add table User"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode);

            IAddTableNode addTableNode = (IAddTableNode)astNode;
            Assert.AreEqual("User", addTableNode.Name);
        }

        [Test()]
        public void ParseAddTableNodeTree()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"add table User:
    Foo
    reference Bar
    index Baz"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode);

            IAddTableNode addTableNode = (IAddTableNode)astNode;
            Assert.AreEqual("User", addTableNode.Name);

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode.ChildNodes[0]);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), astNode.ChildNodes[1]);
            Assert.IsInstanceOfType(typeof(IAddIndexNode), astNode.ChildNodes[2]);
        }

        [Test()]
        public void ParseAlterTableNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"alter table User"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAlterTableNode), astNode);

            IAlterTableNode alterTableNode = (IAlterTableNode)astNode;
            Assert.AreEqual("User", alterTableNode.Name);
        }

        [Test()]
        public void ParseAlterTableNodeTree()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"alter table User:
    remove index ID"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAlterTableNode), astNode);

            IAlterTableNode alterTableNode = (IAlterTableNode)astNode;
            Assert.AreEqual("User", alterTableNode.Name);

            Assert.IsInstanceOfType(typeof(IRemoveIndexNode), astNode.ChildNodes[0]);
        }

        [Test()]
        public void ParseImplicitAddTableNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"version 1:
    User"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IVersionNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode.ChildNodes[0]);

            IAddTableNode addTableNode = (IAddTableNode)astNode.ChildNodes[0];

            Assert.AreEqual("User", addTableNode.Name);
        }

        [Test()]
        public void ParseRemoveTableNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"remove table User"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IRemoveTableNode), astNode);

            IRemoveTableNode removeTableNode = (IRemoveTableNode)astNode;
            Assert.AreEqual("User", removeTableNode.Name);
        }

        [Test()]
        public void ParseImplicitAddColumnNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"add table User:
    Login type => String, length => 200, nullable => false"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode);

            IAddTableNode addTableNode = (IAddTableNode)astNode;
            Assert.AreEqual("User", addTableNode.Name);

            Assert.IsInstanceOfType(typeof(IAddColumnNode), addTableNode.ChildNodes[0]);

            IAddColumnNode addColumnNode = (IAddColumnNode)addTableNode.ChildNodes[0];

            Assert.AreSame(addTableNode, addColumnNode.Parent);
            Assert.AreEqual("Login", addColumnNode.Name);
            Assert.AreEqual("String", addColumnNode.Properties["type"].Value);
            Assert.AreEqual(200, addColumnNode.Properties["length"].Value);
            Assert.AreEqual("false", addColumnNode.Properties["nullable"].Value);
        }

        [Test()]
        public void ParseAddColumnNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"add column Login type => String, length => 200, nullable => false"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);

            IAddColumnNode addColumnNode = (IAddColumnNode)astNode;

            Assert.AreEqual("Login", addColumnNode.Name);
            Assert.AreEqual("String", addColumnNode.Properties["type"].Value);
            Assert.AreEqual(200, addColumnNode.Properties["length"].Value);
            Assert.AreEqual("false", addColumnNode.Properties["nullable"].Value);
        }

        [Test()]
        public void ParseAddColumnNodeTree()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"add column Login:
    constraint ""DF_Foo"" default => ""foo"""));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);
            IAddColumnNode addColumnNode = (IAddColumnNode)astNode;
            Assert.AreEqual("Login", addColumnNode.Name);
            
            Assert.IsInstanceOfType(typeof(IAddConstraintNode), astNode.ChildNodes[0]);
            IAddConstraintNode addConstraintNode = (IAddConstraintNode)astNode.ChildNodes[0];
            Assert.AreEqual("DF_Foo", addConstraintNode.Name);
            Assert.AreEqual("foo", addConstraintNode.Properties["default"].Value);
        }

        [Test()]
        public void ParseAlterColumnNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"alter column Login nullable => false"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAlterColumnNode), astNode);

            IAlterColumnNode alterColumnNode = (IAlterColumnNode)astNode;

            Assert.AreEqual("Login", alterColumnNode.Name);
            Assert.AreEqual("false", alterColumnNode.Properties["nullable"].Value);
        }

        [Test()]
        public void ParseAddIndexNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"add index ""UQ_Login"" columns => [Login, ID]"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddIndexNode), astNode);
            Assert.IsInstanceOfType(typeof(object[]), astNode.Properties["columns"].Value);
        }

        [Test()]
        public void ParseAnonymousAddIndexNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"add column Foo:
    add index unique => true"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);
            
            Assert.IsInstanceOfType(typeof(IAddIndexNode), astNode.ChildNodes[0]);
            IAddIndexNode addIndexNode = (IAddIndexNode)astNode.ChildNodes[0];

            Assert.IsEmpty(addIndexNode.Name);
            Assert.AreEqual("true", addIndexNode.Properties["unique"].Value);
        }

        [Test()]
        public void ParseAddIndexNode2()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"add index ""UQ_Login"" columns => [[Login, desc], ID]"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddIndexNode), astNode);
            Assert.IsInstanceOfType(typeof(object[]), astNode.Properties["columns"].Value);

            IListAstNodePropertyValue columnsList = (IListAstNodePropertyValue)astNode.Properties["columns"].Value;

            Assert.IsInstanceOfType(typeof(IListAstNodePropertyValue), columnsList);
            Assert.IsInstanceOfType(typeof(IListAstNodePropertyValue), columnsList.Items[0]);

            IListAstNodePropertyValue loginList = (IListAstNodePropertyValue)columnsList.Items[0];

            Assert.AreEqual("Login", ((IStringAstNodePropertyValue)loginList.Items[0]).Value);
            Assert.AreEqual("desc", ((IStringAstNodePropertyValue)loginList.Items[1]).Value);
            Assert.AreEqual("ID", ((IStringAstNodePropertyValue)columnsList.Items[1]).Value);
        }

        [Test()]
        public void ParseImplicitAnonymousAddIndexNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"add column Login type => String, length => 200:
    index unique => true"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddIndexNode), astNode.ChildNodes[0]);

            IAddIndexNode addIndexNode = (IAddIndexNode)astNode.ChildNodes[0];

            Assert.IsEmpty(addIndexNode.Name);
            Assert.AreEqual("true", addIndexNode.Properties["unique"].Value);
        }

        [Test()]
        public void ParseImplicitAddIndexNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"add column Login type => String, length => 200:
    index ""UQ_Login""unique => true"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddIndexNode), astNode.ChildNodes[0]);

            IAddIndexNode addIndexNode = (IAddIndexNode)astNode.ChildNodes[0];

            Assert.AreEqual("UQ_Login", addIndexNode.Name);
            Assert.AreEqual("true", addIndexNode.Properties["unique"].Value);
        }

        [Test()]
        public void ParseImplicitAnonymousAddReferenceNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"add column Login type => String, length => 200:
    reference"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), astNode.ChildNodes[0]);

            IAddReferenceNode addReferenceNode = (IAddReferenceNode)astNode.ChildNodes[0];

            Assert.IsEmpty(addReferenceNode.Name);
        }

        [Test()]
        public void ParseAnonymousAddReferenceNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"add column Login type => String, length => 200:
    add reference"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), astNode.ChildNodes[0]);

            IAddReferenceNode addReferenceNode = (IAddReferenceNode)astNode.ChildNodes[0];

            Assert.IsEmpty(addReferenceNode.Name);
        }

        [Test()]
        public void ParseImplicitAddReferenceNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"add column Login type => String, length => 200:
    reference ""FK_User_Login_Whatever_Else"""));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), astNode.ChildNodes[0]);

            IAddReferenceNode addReferenceNode = (IAddReferenceNode)astNode.ChildNodes[0];

            Assert.AreEqual("FK_User_Login_Whatever_Else", addReferenceNode.Name);
        }

        [Test()]
        public void ParseAddReferenceNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"add column Login type => String, length => 200:
    add reference ""FK_User_Login_Whatever_Else"""));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), astNode.ChildNodes[0]);

            IAddReferenceNode addReferenceNode = (IAddReferenceNode)astNode.ChildNodes[0];

            Assert.AreEqual("FK_User_Login_Whatever_Else", addReferenceNode.Name);
        }

        [Test()]
        public void ParseAddReferenceNode2()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"add table Foo:
    add reference ""FK_User_Login_Whatever_Else"""));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), astNode.ChildNodes[0]);

            IAddReferenceNode addReferenceNode = (IAddReferenceNode)astNode.ChildNodes[0];

            Assert.AreEqual("FK_User_Login_Whatever_Else", addReferenceNode.Name);
        }

        [Test()]
        public void ParseRemoveReferenceNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"remove reference ""FK_User_Login_Whatever_Else"" fk-table => User"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IRemoveReferenceNode), astNode);

            IRemoveReferenceNode removeReferenceNode = (IRemoveReferenceNode)astNode;

            Assert.AreEqual("FK_User_Login_Whatever_Else", removeReferenceNode.Name);
            Assert.AreEqual("User", removeReferenceNode.Properties["fk-table"].Value.ToString());
        }

        [Test()]
        public void ParseRemoveIndexNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(@"remove index ""IX_User_Login"" table-name => User"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IRemoveIndexNode), astNode);

            IRemoveIndexNode removeIndexNode = (IRemoveIndexNode)astNode;

            Assert.AreEqual(new Location(0, 0), removeIndexNode.Location);
            Assert.AreEqual("IX_User_Login", removeIndexNode.Name);
            Assert.AreEqual("User", removeIndexNode.Properties["table-name"].Value.ToString());
        }

        [Test()]
        public void ParseMigrationNodeTree()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"migration ""Waffle"" revision => 1, configuration => Debug:
    baseline
        
    version 1 description => ""Updating database for the first time ever"":  
                     
        add table User:
            Login type => String, length => 200
            add column Password type => Binary, length => 64

    version 2 description => ""Updating the second time"""));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IMigrationNode), astNode);
            Assert.IsNull(astNode.Parent);
            Assert.AreEqual(3, astNode.ChildNodes.Count);

            IMigrationNode migrationNode = (IMigrationNode)astNode;

            Assert.AreEqual("Waffle", migrationNode.Name);
            Assert.AreEqual(1, migrationNode.Properties["revision"].Value);
            Assert.AreEqual("Debug", migrationNode.Properties["configuration"].Value);

            Assert.IsInstanceOfType(typeof(IBaselineNode), astNode.ChildNodes[0]);
            Assert.IsInstanceOfType(typeof(IVersionNode), astNode.ChildNodes[1]);
            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode.ChildNodes[1].ChildNodes[0]);
            Assert.IsInstanceOfType(typeof(IVersionNode), astNode.ChildNodes[2]);
        }

        [Test()]
        public void ParseRefactorNode()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner("refactor add-mirror-table table => Foo"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IRefactorNode), astNode);
            Assert.IsNull(astNode.Parent);
            Assert.AreEqual(1, astNode.Properties.Count);

            IRefactorNode refactorNode = (IRefactorNode)astNode;

            Assert.AreEqual("add-mirror-table", refactorNode.Name);
        }

        [Test()]
        [ExpectedException(typeof(MdlParserException))]
        public void ParseIncorrectBlockLayout()
        {
            IMdlParser mdlParser = new MdlParser(CreateScanner(
@"add table Foo:
    ID
        index ""FK_User_Login_Whatever_Else"""));
            IAstNode astNode = mdlParser.Parse();
        }

        [Test()]
        public void ParseAddConstraintNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"add constraint ""DF_Foo"" default => ""def"""));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IAddConstraintNode), astNode);

            IAddConstraintNode addConstraintNode = (IAddConstraintNode)astNode;
            Assert.AreEqual("def", addConstraintNode.Properties["default"].Value);
        }

        [Test()]
        public void ParseRemoveConstraintNode()
        {
            MdlParser mdlParser = new MdlParser(CreateScanner(@"remove constraint DF_Foo"));
            IAstNode astNode = mdlParser.Parse();

            Assert.IsInstanceOfType(typeof(IRemoveConstraintNode), astNode);

            IRemoveConstraintNode removeConstraintNode = (IRemoveConstraintNode)astNode;
            Assert.AreEqual("DF_Foo", removeConstraintNode.Name);
        }

        public static IMdlScanner CreateScanner(string source)
        {
            return CreateScanner(new StringReader(source));
        }

        public static IMdlScanner CreateScanner(TextReader textReader)
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(textReader));
            mdlScanner.RegisterKeyword("migration");
            mdlScanner.RegisterKeyword("deployment");
            mdlScanner.RegisterKeyword("type-aliases");
            mdlScanner.RegisterKeyword("type-alias");
            mdlScanner.RegisterKeyword("environment");
            mdlScanner.RegisterKeyword("defaults");
            mdlScanner.RegisterKeyword("default-primary-key");
            mdlScanner.RegisterKeyword("baseline");
            mdlScanner.RegisterKeyword("version");
            mdlScanner.RegisterKeyword("add");
            mdlScanner.RegisterKeyword("remove");
            mdlScanner.RegisterKeyword("table");
            mdlScanner.RegisterKeyword("column");
            mdlScanner.RegisterKeyword("index");
            mdlScanner.RegisterKeyword("reference");
            mdlScanner.RegisterKeyword("alter");
            mdlScanner.RegisterKeyword("templates");
            mdlScanner.RegisterKeyword("template");
            mdlScanner.RegisterKeyword("refactor");
            mdlScanner.RegisterKeyword("constraint");

            return mdlScanner;
        }
    }
}
