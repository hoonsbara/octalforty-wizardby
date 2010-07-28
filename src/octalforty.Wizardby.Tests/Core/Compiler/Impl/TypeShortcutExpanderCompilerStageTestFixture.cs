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
