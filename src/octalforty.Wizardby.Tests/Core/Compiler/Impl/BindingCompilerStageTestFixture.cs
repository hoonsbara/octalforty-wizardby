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

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Impl;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Tests.Core.Compiler.Impl
{
    [TestFixture()]
    public class BindingCompilerStageTestFixture : AstTestFixtureBase
    {
        [Test()]
        public void BindReferences()
        {
            Environment environment = new Environment();

            NamingCompilerStage namingCompilerStage = new NamingCompilerStage();
            namingCompilerStage.SetEnvironment(environment);

            IMdlCompilerStage bindingStage = new BindingCompilerStage();
            bindingStage.SetEnvironment(environment);

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    templates:
        table template Foo

    version 1:    
        add table ""BlogPost"":
            ID type => Int32, primary-key => true

    version 2:
        remove table BlogPost

    version 3:
        add table BlogPost:            
            Version type => Int32
            ID type => Int32, primary-key => true

        alter table BlogPost:
            add column PublishedOn type => DateTime

        add table ""BlogPostApproval"":
            BlogPostID type => Int32:
                reference ""FK0"" pk-table => BlogPost
            ApprovedOn type => DateTime

            add reference FK1 fk-columns => [BlogPostID, ApprovedOn], pk-table => BlogPost, pk-column => Version
            
        add reference FK2 fk-table => BlogPostApproval, fk-columns => [ApprovedOn,BlogPostID], pk-table => BlogPost

        alter table BlogPostApproval:
            remove reference FK2
            add column Bar:
                reference pk-table => BlogPost, pk-column => ID, fk-table => BlogPost, fk-column => ID

        remove reference FK1 fk-table => BlogPostApproval")).Parse();

            astNode.Accept(namingCompilerStage);
            astNode.Accept(bindingStage);

            IVersionNode version3Node = (IVersionNode)astNode.ChildNodes[3];
            IAddTableNode addTableBlogPostApprovalNode = (IAddTableNode)version3Node.ChildNodes[2];

            IAddReferenceNode addReferenceBlogPostNode = (IAddReferenceNode)addTableBlogPostApprovalNode.ChildNodes[0].ChildNodes[0];

            Assert.AreEqual("BlogPostApproval", addReferenceBlogPostNode.FkTable);
            Assert.AreEqual("BlogPostID", addReferenceBlogPostNode.FkColumns[0]);
            Assert.AreEqual("BlogPost", addReferenceBlogPostNode.PkTable);
            Assert.AreEqual("ID", addReferenceBlogPostNode.PkColumns[0]);

            IAddReferenceNode addReferenceFk1Node = (IAddReferenceNode)addTableBlogPostApprovalNode.ChildNodes[2];

            Assert.AreEqual("FK1", addReferenceFk1Node.Name);
            Assert.AreEqual("BlogPostApproval", addReferenceFk1Node.FkTable);
            Assert.AreEqual("BlogPostID", addReferenceFk1Node.FkColumns[0]);
            Assert.AreEqual("ApprovedOn", addReferenceFk1Node.FkColumns[1]);
            Assert.AreEqual("BlogPost", addReferenceFk1Node.PkTable);
            Assert.AreEqual("Version", addReferenceFk1Node.PkColumns[0]);

            IAddReferenceNode addReferenceFk2Node = (IAddReferenceNode)version3Node.ChildNodes[3];

            Assert.AreEqual("FK2", addReferenceFk2Node.Name);
            Assert.AreEqual("BlogPostApproval", addReferenceFk2Node.FkTable);
            Assert.AreEqual("ApprovedOn", addReferenceFk2Node.FkColumns[0]);
            Assert.AreEqual("BlogPostID", addReferenceFk2Node.FkColumns[1]);
            Assert.AreEqual("BlogPost", addReferenceFk2Node.PkTable);
            Assert.AreEqual("ID", addReferenceFk2Node.PkColumns[0]);

            IAlterTableNode alterTableBlogPostApprovalNode = (IAlterTableNode)version3Node.ChildNodes[4];
            IRemoveReferenceNode removeReferenceFk2Node =
                (IRemoveReferenceNode)alterTableBlogPostApprovalNode.ChildNodes[0];

            Assert.AreEqual("FK2", removeReferenceFk2Node.Name);
            Assert.AreEqual("BlogPostApproval", removeReferenceFk2Node.Table);

            IRemoveReferenceNode removeReferenceFk1Node = (IRemoveReferenceNode)version3Node.ChildNodes[5];

            Assert.AreEqual("FK1", removeReferenceFk1Node.Name);
            Assert.AreEqual("BlogPostApproval", removeReferenceFk1Node.Table);

            ITableDefinition blogPostTable = environment.Schema.GetTable("BlogPost");

            Assert.IsNotNull(blogPostTable);
            Assert.IsNotNull(blogPostTable.GetColumn("ID"));
            Assert.IsNotNull(blogPostTable.GetColumn("Version"));
            Assert.IsNotNull(blogPostTable.GetColumn("PublishedOn"));

            ITableDefinition blogPostApprovalTable = environment.Schema.GetTable("BlogPostApproval");

            Assert.IsNotNull(blogPostApprovalTable);
            Assert.IsNotNull(blogPostApprovalTable.GetColumn("BlogPostID"));
            Assert.IsNotNull(blogPostApprovalTable.GetColumn("ApprovedOn"));
            Assert.IsNotNull(blogPostApprovalTable.GetColumn("Bar"));

            Assert.AreEqual(2, blogPostApprovalTable.References.Count);

            IReferenceDefinition referenceFk0 = blogPostApprovalTable.GetReference("FK0");
            Assert.IsNotNull(referenceFk0);

            IReferenceDefinition referenceAnonymous = null;
            foreach (IReferenceDefinition r in blogPostApprovalTable.References)
                if(environment.IsAnonymousIdentifier(r.Name))
                {
                    referenceAnonymous = r;
                    break;
                }

            Assert.IsNotNull(referenceAnonymous);
        }

        [Test()]
        public void InferTypes()
        {
            IMdlCompilerStage bindingStage = new BindingCompilerStage();
            bindingStage.SetEnvironment(new Environment());

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
            BlogPostID nullable => true:
                reference ""FK0"" pk-table => BlogPost
            ApprovedOn type => DateTime")).Parse();

            astNode.Accept(bindingStage);

            IVersionNode version3Node = (IVersionNode)astNode.ChildNodes[2];
            IAddTableNode addTableBlogPostApprovalNode = (IAddTableNode)version3Node.ChildNodes[2];
            IAddColumnNode addColumnBlogPostIDNode = (IAddColumnNode)addTableBlogPostApprovalNode.ChildNodes[0];

            Assert.AreEqual(DbType.Int32, addColumnBlogPostIDNode.Type);
            Assert.AreEqual(true, addColumnBlogPostIDNode.Nullable);
        }

        [Test()]
        [ExpectedException(typeof(MdlCompilerException), ExpectedMessage = "Unknown type 'Int128'.")]
        public void InferUnknownType()
        {
            IMdlCompilerStage bindingStage = new BindingCompilerStage();
            bindingStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    version 1:    
        add table BlogPost:
            ID type => Int128, primary-key => true")).Parse();

            astNode.Accept(bindingStage);
        }

        [Test()]
        public void BindIndexes()
        {
            Environment environment = new Environment();

            NamingCompilerStage namingStage = new NamingCompilerStage();
            namingStage.SetEnvironment(environment);

            IMdlCompilerStage bindingStage = new BindingCompilerStage();
            bindingStage.SetEnvironment(environment);

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    version 1:    
        add table BlogPost:
            ID type => Int32, primary-key => true:
                index ""IX_ID"" unique => true, clustered => true
            Version type => Int32

        alter table BlogPost:
            add column PublishedOn type => DateTime:
                index ""IX_PublishedOn""
                index

            add index ""IX_Pub"" column => PublishedOn

        add index IX_Foo table => BlogPost, columns => [[ID, descending], PublishedOn]

        remove index IX_Foo table => BlogPost

        alter table BlogPost:
            remove index IX_Pub")).Parse();

            astNode.Accept(namingStage);
            astNode.Accept(bindingStage);

            IVersionNode version1Node = (IVersionNode)astNode.ChildNodes[0];

            IAddIndexNode addIxIdIndexNode = (IAddIndexNode)version1Node.ChildNodes[0].ChildNodes[0].ChildNodes[0];

            Assert.AreEqual("IX_ID", addIxIdIndexNode.Name);
            Assert.AreEqual("ID", addIxIdIndexNode.Columns[0].Name);
            Assert.IsFalse(addIxIdIndexNode.Columns[0].SortDirection.HasValue);
            Assert.IsTrue(addIxIdIndexNode.Unique.Value);
            Assert.IsTrue(addIxIdIndexNode.Clustered.Value);

            IAddIndexNode addIxPubIndexNode = (IAddIndexNode)version1Node.ChildNodes[1].ChildNodes[1];

            Assert.AreEqual("IX_Pub", addIxPubIndexNode.Name);
            Assert.AreEqual("BlogPost", addIxPubIndexNode.Table);

            IAddIndexNode addIxFooIndexNode = (IAddIndexNode)version1Node.ChildNodes[2];

            Assert.AreEqual("IX_Foo", addIxFooIndexNode.Name);
            Assert.AreEqual("BlogPost", addIxFooIndexNode.Table);
            Assert.AreEqual("PublishedOn", addIxPubIndexNode.Columns[0].Name);
            Assert.IsFalse(addIxPubIndexNode.Columns[0].SortDirection.HasValue);
            
            Assert.AreEqual(2, addIxFooIndexNode.Columns.Count);

            Assert.AreEqual("ID", addIxFooIndexNode.Columns[0].Name);
            Assert.AreEqual(SortDirection.Descending, addIxFooIndexNode.Columns[0].SortDirection.Value);

            Assert.AreEqual("PublishedOn", addIxFooIndexNode.Columns[1].Name);

            IRemoveIndexNode removeIndexIxFooNode = (IRemoveIndexNode)version1Node.ChildNodes[3];

            Assert.AreEqual("IX_Foo", removeIndexIxFooNode.Name);
            Assert.AreEqual("BlogPost", removeIndexIxFooNode.Table);

            IRemoveIndexNode removeIndexIxPubNode = (IRemoveIndexNode)version1Node.ChildNodes[4].ChildNodes[0];

            Assert.AreEqual("IX_Pub", removeIndexIxPubNode.Name);
            Assert.AreEqual("BlogPost", removeIndexIxPubNode.Table);

            ITableDefinition blogPostTable = environment.Schema.GetTable("BlogPost");

            Assert.IsNotNull(blogPostTable);
            
            Assert.AreEqual(3, blogPostTable.Columns.Count);
            Assert.IsNotNull(blogPostTable.GetColumn("ID"));
            Assert.IsNotNull(blogPostTable.GetColumn("Version"));
            Assert.IsNotNull(blogPostTable.GetColumn("PublishedOn"));

            Assert.AreEqual(3, blogPostTable.Indexes.Count);
            Assert.AreEqual("IX_ID", blogPostTable.Indexes[0].Name);
            Assert.AreEqual("IX_PublishedOn", blogPostTable.Indexes[1].Name);
            Assert.IsTrue(environment.IsAnonymousIdentifier(blogPostTable.Indexes[2].Name));
        }

        [Test()]
        [ExpectedException(typeof(MdlCompilerException))]
        public void BindReferencesThrowsMdlCompilerExceptionOnMissingPkTable()
        {
            IMdlCompilerStage bindingStage = new BindingCompilerStage();
            bindingStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    version 1:    
        add table BlogPost:
            ID type => Int32, primary-key => true
            add reference FK1 pk-column => ID")).Parse();

            astNode.Accept(bindingStage);
        }

        [Test()]
        [ExpectedException(typeof(MdlCompilerException))]
        public void BindReferencesThrowsMdlCompilerExceptionOnMissingFkTable()
        {
            IMdlCompilerStage bindingStage = new BindingCompilerStage();
            bindingStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    version 1:    
        add table BlogPost:
            ID type => Int32, primary-key => true
        
        add reference FK1 pk-table => BlogPost")).Parse();

            astNode.Accept(bindingStage);
        }

        [Test()]
        [ExpectedException(typeof(MdlCompilerException))]
        public void BindReferencesThrowsMdlCompilerExceptionOnMissingFkColumn()
        {
            IMdlCompilerStage bindingStage = new BindingCompilerStage();
            bindingStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    version 1:    
        add table BlogPost:
            ID type => Int32, primary-key => true
        
        add reference FK1 pk-table => BlogPost, fk-table => BlogPost")).Parse();

            astNode.Accept(bindingStage);
        }

        [Test()]
        public void BindColumnProperties()
        {
            IMdlCompilerStage bindingStage = new BindingCompilerStage();
            bindingStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    version 1:    
        add table BlogPost:
            ID type => Int32, primary-key => true, nullable => true, length => 8, scale => 20, precision => 10, identity => true, default => ""0""")).Parse();

            astNode.Accept(bindingStage);

            IAddColumnNode addColumnNode = (IAddColumnNode)astNode.ChildNodes[0].ChildNodes[0].ChildNodes[0];

            Assert.AreEqual(DbType.Int32, addColumnNode.Type);
            Assert.AreEqual(true, addColumnNode.Identity);
            Assert.AreEqual(true, addColumnNode.PrimaryKey);
            Assert.AreEqual(true, addColumnNode.Nullable);
            Assert.AreEqual(8, addColumnNode.Length.Value);
            Assert.AreEqual(20, addColumnNode.Scale.Value);
            Assert.AreEqual(10, addColumnNode.Precision.Value);
            Assert.AreEqual("0", addColumnNode.Default);
        }
    }
}
