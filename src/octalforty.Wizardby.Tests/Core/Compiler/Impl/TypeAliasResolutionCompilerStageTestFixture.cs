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
    public class TypeAliasResolutionCompilerStageTestFixture
    {
        [Test()]
        public void ResolveTypeAlises()
        {
            IMdlCompilerStage typeAliasResolutionStage = new TypeAliasResolutionCompilerStage();
            typeAliasResolutionStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    type-aliases:
        type-alias t type => String, nullable => false, length => 200, ignore => true, default => ""foo""
        i type => Int32, nullable => false
        ni type => i, nullable => true
        d type => Decimal, nullable => false, scale => 20, precision => 10, precision-ignored => true

    version 1:
        add table Foo:
            Bar type => t
            Baz type => DateTime
            Bax type => i
            Bull type => d
            Buff type => ni")).Parse();

            astNode.Accept(typeAliasResolutionStage);

            IAstNode version1Node = astNode.ChildNodes[1];
            IAstNode addTableFooNode = version1Node.ChildNodes[0];

            IAddColumnNode addColumnNode = (IAddColumnNode)addTableFooNode.ChildNodes[0];

            Assert.IsNotNull(addColumnNode);
            Assert.AreEqual(4, addColumnNode.Properties.Count);
            Assert.AreEqual("Bar", addColumnNode.Name);
            Assert.AreEqual("String", addColumnNode.Properties["type"].Value);
            Assert.AreEqual("false", addColumnNode.Properties["nullable"].Value);
            Assert.AreEqual(200, addColumnNode.Properties["length"].Value);
            Assert.AreEqual("foo", addColumnNode.Properties["default"].Value);

            addColumnNode = (IAddColumnNode)addTableFooNode.ChildNodes[1];

            Assert.IsNotNull(addColumnNode);
            Assert.AreEqual("Baz", addColumnNode.Name);
            Assert.AreEqual("DateTime", addColumnNode.Properties["type"].Value);

            addColumnNode = (IAddColumnNode)addTableFooNode.ChildNodes[2];

            Assert.IsNotNull(addColumnNode);
            Assert.AreEqual("Bax", addColumnNode.Name);
            Assert.AreEqual("Int32", addColumnNode.Properties["type"].Value);
            Assert.AreEqual("false", addColumnNode.Properties["nullable"].Value);

            addColumnNode = (IAddColumnNode)addTableFooNode.ChildNodes[3];

            Assert.IsNotNull(addColumnNode);
            Assert.AreEqual(4, addColumnNode.Properties.Count);
            Assert.AreEqual("Bull", addColumnNode.Name);
            Assert.AreEqual("Decimal", addColumnNode.Properties["type"].Value);
            Assert.AreEqual("false", addColumnNode.Properties["nullable"].Value);
            Assert.AreEqual(20, addColumnNode.Properties["scale"].Value);
            Assert.AreEqual(10, addColumnNode.Properties["precision"].Value);

            addColumnNode = (IAddColumnNode)addTableFooNode.ChildNodes[4];

            Assert.IsNotNull(addColumnNode);
            Assert.AreEqual("Buff", addColumnNode.Name);
            Assert.AreEqual("Int32", addColumnNode.Properties["type"].Value);
            Assert.AreEqual("true", addColumnNode.Properties["nullable"].Value);
        }
    }
}
