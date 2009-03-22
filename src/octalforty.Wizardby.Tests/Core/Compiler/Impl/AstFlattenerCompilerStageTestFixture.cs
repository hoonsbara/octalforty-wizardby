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
    public class AstFlattenerCompilerStageTestFixture
    {
        [Test()]
        public void FlattenAst()
        {
            IMdlCompilerStage astFlattenerStage = new AstFlattenerCompilerStage();
            astFlattenerStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    version 1:            
        add table BlogPostApproval:
            BlogPostID nullable => true:
                reference ""FK0"" pk-table => BlogPost
                index unique => false
            ApprovedOn type => DateTime
            reference ""FK0"" pk-table => BlogPost
            index ""IXBAR"" unique => false

        alter table BlogPostApproval:
            remove reference ""FOO""
            remove index ""Bar""")).Parse();

            astNode.Accept(astFlattenerStage);

            IVersionNode version1Node = (IVersionNode)astNode.ChildNodes[0];

            Assert.IsInstanceOfType(typeof(IAddReferenceNode), version1Node.ChildNodes[2]);
            Assert.IsInstanceOfType(typeof(IAddIndexNode), version1Node.ChildNodes[3]);
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), version1Node.ChildNodes[4]);
            Assert.IsInstanceOfType(typeof(IAddIndexNode), version1Node.ChildNodes[5]);
        }
    }
}
