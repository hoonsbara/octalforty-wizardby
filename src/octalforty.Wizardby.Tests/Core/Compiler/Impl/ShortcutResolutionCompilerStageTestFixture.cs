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
    public class ShortcutResolutionCompilerStageTestFixture : AstTestFixtureBase
    {
        [Test()]
        public void ResolveShortcuts()
        {
            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(@"add table Foo:
    Bar type => Int32, references => Bar, unique => true")).Parse();

            IMdlCompilerStage shortcutResolutionStage = new ShortcutResolutionCompilerStage();
            shortcutResolutionStage.SetEnvironment(new Environment());

            astNode.Accept(shortcutResolutionStage);

            AssertAddTable(astNode, "Foo", null);

            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode);
            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode.ChildNodes[0]);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), astNode.ChildNodes[0].ChildNodes[0]);
            Assert.IsInstanceOfType(typeof(IAddIndexNode), astNode.ChildNodes[0].ChildNodes[1]);

            IAddReferenceNode addReferenceNode = (IAddReferenceNode)astNode.ChildNodes[0].ChildNodes[0];
            Assert.AreEqual("Bar", AstNodePropertyUtil.AsString(addReferenceNode.Properties, "pk-table"));
            Assert.IsNotNull(addReferenceNode.Location);

            IAddIndexNode addIndexNode = (IAddIndexNode)astNode.ChildNodes[0].ChildNodes[1];
            Assert.AreEqual("true", AstNodePropertyUtil.AsString(addIndexNode.Properties, "unique"));
            Assert.IsNotNull(addIndexNode.Location);
        }
    }
}
