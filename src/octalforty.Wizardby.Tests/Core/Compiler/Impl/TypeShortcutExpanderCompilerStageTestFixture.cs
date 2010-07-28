using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Impl;

namespace octalforty.Wizardby.Tests.Core.Compiler.Impl
{
    [TestFixture()]
    public class TypeShortcutExpanderCompilerStageTestFixture
    {
        [Test()]
        public void ExpandTypeShortcuts()
        {
            var addTableNode = (IAddTableNode)MdlParserTestFixture.Parse(
@"add table Person:
    Age type => ""int32!""
    Name type => ""string ? (  200)""
    Salary type => ""decimal ! ( 18 , 2 ) """);
            addTableNode.Accept(new TypeShortcutExpanderCompilerStage());

            var addAgeColumn = (IAddColumnNode)addTableNode.ChildNodes[0];
            Assert.AreEqual("int32", AstNodePropertyUtil.AsString(addAgeColumn.Properties[MdlSyntax.Type].Value));
            Assert.AreEqual("false", AstNodePropertyUtil.AsString(addAgeColumn.Properties[MdlSyntax.Nullable].Value));

            var addNameColumn = (IAddColumnNode)addTableNode.ChildNodes[1];
            Assert.AreEqual("string", AstNodePropertyUtil.AsString(addNameColumn.Properties[MdlSyntax.Type].Value));
            Assert.AreEqual("true", AstNodePropertyUtil.AsString(addNameColumn.Properties[MdlSyntax.Nullable].Value));
            Assert.AreEqual(200, AstNodePropertyUtil.AsInteger(addNameColumn.Properties[MdlSyntax.Length].Value));

            var addSalaryColumn = (IAddColumnNode)addTableNode.ChildNodes[2];
            Assert.AreEqual("decimal", AstNodePropertyUtil.AsString(addSalaryColumn.Properties[MdlSyntax.Type].Value));
            Assert.AreEqual("false", AstNodePropertyUtil.AsString(addSalaryColumn.Properties[MdlSyntax.Nullable].Value));
            Assert.AreEqual(18, AstNodePropertyUtil.AsInteger(addSalaryColumn.Properties[MdlSyntax.Scale].Value));
            Assert.AreEqual(2, AstNodePropertyUtil.AsInteger(addSalaryColumn.Properties[MdlSyntax.Precision].Value));
        }
    }
}
