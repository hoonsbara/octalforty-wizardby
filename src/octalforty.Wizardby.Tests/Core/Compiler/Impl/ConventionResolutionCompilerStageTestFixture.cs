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
    public class ConventionResolutionCompilerStageTestFixture : AstTestFixtureBase
    {
        [Test()]
        public void ResolveAddIndex()
        {
            IAddTableNode addTableNode = (IAddTableNode)MdlParserTestFixture.Parse(
@"add table Foo:
    Bar:
        index");
            addTableNode.Accept(new ConventionResolutionCompilerStage());

            IAddIndexNode addIndexNode = (IAddIndexNode)addTableNode.ChildNodes[0].ChildNodes[0];

            Assert.AreEqual("Foo", AstNodePropertyUtil.AsString(addIndexNode.Properties, MdlSyntax.Table));
            Assert.AreEqual("Bar", AstNodePropertyUtil.AsString(addIndexNode.Properties, MdlSyntax.Column));
        }

        [Test()]
        public void ResolveAddIndex2()
        {
            IAddTableNode addTableNode = (IAddTableNode)MdlParserTestFixture.Parse(
@"add table Foo:
    index """" column => Bar");
            addTableNode.Accept(new ConventionResolutionCompilerStage());

            IAddIndexNode addIndexNode = (IAddIndexNode)addTableNode.ChildNodes[0];

            Assert.AreEqual("Foo", AstNodePropertyUtil.AsString(addIndexNode.Properties, MdlSyntax.Table));
            Assert.AreEqual("Bar", AstNodePropertyUtil.AsString(addIndexNode.Properties, MdlSyntax.Column));
        }

        [Test()]
        [ExpectedException(typeof(MdlParserException))]
        public void ResolveAddIndexNoColumn()
        {
            IAddTableNode addTableNode = (IAddTableNode)MdlParserTestFixture.Parse(
@"add table Foo:
    index");
            addTableNode.Accept(new ConventionResolutionCompilerStage());
        }

        [Test()]
        [ExpectedException(typeof(MdlParserException))]
        public void ResolveAddIndexNoColumnAndTable()
        {
            IAddIndexNode addIndexNode = (IAddIndexNode)MdlParserTestFixture.Parse(@"add index");
            addIndexNode.Accept(new ConventionResolutionCompilerStage());
        }
    }
}
