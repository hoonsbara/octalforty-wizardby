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
using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Impl;

namespace octalforty.Wizardby.Tests.Core.Compiler.Impl
{
    [TestFixture()]
    public class PrimaryKeyResolutionCompilerStageTestFixture
    {
        [Test()]
        public void ResolvePrimaryKeys()
        {
            IMdlCompilerStage primaryKeyResolutionStage = new PrimaryKeyResolutionCompilerStage();
            primaryKeyResolutionStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    defaults:
        default-primary-key ID type => Int32, nullable => false, ignore-primary-key => true, identity => true, default => ""foo""

    version 1:
        add table Foo

        add table Bar:
            Ident primary-key => true")).Parse();

            astNode.Accept(primaryKeyResolutionStage);

            IAstNode version1Node = astNode.ChildNodes[1];
            IAstNode addTableFooNode = version1Node.ChildNodes[0];
            IAstNode addTableBarNode = version1Node.ChildNodes[1];

            IAddColumnNode addColumnNode = (IAddColumnNode)addTableFooNode.ChildNodes[0];

            Assert.IsNotNull(addColumnNode);
            Assert.AreEqual(5, addColumnNode.Properties.Count);
            Assert.AreEqual("ID", addColumnNode.Name);
            Assert.AreEqual("Int32", addColumnNode.Properties["type"].Value);
            Assert.AreEqual("false", addColumnNode.Properties["nullable"].Value);
            Assert.AreEqual("true", addColumnNode.Properties["primary-key"].Value);
            Assert.AreEqual("true", addColumnNode.Properties["identity"].Value);
            Assert.AreEqual("foo", addColumnNode.Properties["default"].Value);

            addColumnNode = (IAddColumnNode)addTableBarNode.ChildNodes[0];

            Assert.AreEqual("Ident", addColumnNode.Name);
            Assert.AreEqual("true", addColumnNode.Properties["primary-key"].Value);
        }
    }
}
