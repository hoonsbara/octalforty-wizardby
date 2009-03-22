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
    public class SchemaInfoBuilderCompilerStageTestFixture
    {
        [Test()]
        public void BuildSchemaInfo()
        {
            IMdlCompilerStage schemaInfoBuilderStage = new SchemaInfoBuilderCompilerStage();
            schemaInfoBuilderStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    version 1:    
        add table BlogPost:
            ID type => Int32, primary-key => true

    version 2:
        remove table BlogPost

    version 3:
        add table BlogPost:
            ID type => Int32, primary-key => true
            Version type => Int32

        alter table BlogPost:
            add column PublishedOn type => DateTime

        add table BlogPostApproval:
            BlogPostID type => Int32:
                reference ""FK0"" pk-table => BlogPost
            ApprovedOn type => DateTime

            add reference FK1 fk-columns => [BlogPostID, ApprovedOn], pk-table => BlogPost, pk-column => Version
            
        add reference FK2 fk-table => BlogPostApproval, fk-columns => [ApprovedOn,BlogPostID], pk-table => BlogPost")).Parse();

            astNode.Accept(schemaInfoBuilderStage);

            IAddTableNode addSchemaInfoTableNode = (IAddTableNode)astNode.ChildNodes[0].ChildNodes[0];
            
            Assert.AreEqual("SchemaInfo", addSchemaInfoTableNode.Name);

            IAddColumnNode addVersionColumnNode = (IAddColumnNode)addSchemaInfoTableNode.ChildNodes[0];

            Assert.AreEqual("Int64", addVersionColumnNode.Properties["type"].Value);
            Assert.AreEqual("true", addVersionColumnNode.Properties["unique"].Value);
            Assert.AreEqual("false", addVersionColumnNode.Properties["nullable"].Value);
        }
    }
}
