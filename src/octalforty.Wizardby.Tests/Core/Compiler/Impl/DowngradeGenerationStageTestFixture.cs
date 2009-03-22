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
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Impl;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Tests.Core.Compiler.Impl
{
    [TestFixture()]
    public class DowngradeGenerationStageTestFixture : AstTestFixtureBase
    {
        [Test()]
        public void GenerateDowngrade()
        {
            IAstNode astNode = GetAstNode();

            AssertVersionNode(astNode.ChildNodes[2], 20090226100407);
            AssertDowngradeNode(astNode.ChildNodes[2].ChildNodes[1]);

            IDowngradeNode downgradeNode = (IDowngradeNode)astNode.ChildNodes[2].ChildNodes[1];

            AssertRemoveReference(downgradeNode.ChildNodes[0], "FK4", "BlogPostTagJunction");
            AssertRemoveReference(downgradeNode.ChildNodes[1], "FK3", "BlogPostTagJunction");
            AssertRemoveReference(downgradeNode.ChildNodes[2], "FK2", "BlogPost");
            AssertRemoveReference(downgradeNode.ChildNodes[3], "FK1", "BlogPost");

            AssertRemoveIndex(downgradeNode.ChildNodes[4], "IX_Login", "Author");
            AssertRemoveIndex(downgradeNode.ChildNodes[5], "IX_EmailAddress", "Author");

            AssertRemoveTable(downgradeNode.ChildNodes[6], "BlogPostTagJunction");
            AssertRemoveTable(downgradeNode.ChildNodes[7], "BlogPost");
            AssertRemoveTable(downgradeNode.ChildNodes[8], "Blog");
            AssertRemoveTable(downgradeNode.ChildNodes[9], "Tag");
            AssertRemoveTable(downgradeNode.ChildNodes[10], "Author");

            AssertVersionNode(astNode.ChildNodes[3], 20090226100408);
            AssertDowngradeNode(astNode.ChildNodes[3].ChildNodes[1]);

            downgradeNode = (IDowngradeNode)astNode.ChildNodes[3].ChildNodes[1];

            AssertRemoveReference(downgradeNode.ChildNodes[0], "FK5", "BlogPostComment");
            AssertRemoveTable(downgradeNode.ChildNodes[1], "BlogPostComment");

            AssertVersionNode(astNode.ChildNodes[4], 20090226100409);
            AssertDowngradeNode(astNode.ChildNodes[4].ChildNodes[1]);

            downgradeNode = (IDowngradeNode)astNode.ChildNodes[4].ChildNodes[1];

            AssertRemoveIndex(downgradeNode.ChildNodes[0], "IX_Login", "User");
            AssertRemoveReference(downgradeNode.ChildNodes[1], "FK11", "Media");
            AssertRemoveReference(downgradeNode.ChildNodes[2], "FK10", "Media");
            AssertRemoveTable(downgradeNode.ChildNodes[3], "User");
            AssertRemoveTable(downgradeNode.ChildNodes[4], "Media");

            AssertVersionNode(astNode.ChildNodes[5], 20090226100411);
            AssertDowngradeNode(astNode.ChildNodes[5].ChildNodes[1]);

            downgradeNode = (IDowngradeNode)astNode.ChildNodes[5].ChildNodes[1];

            AssertRemoveReference(downgradeNode.ChildNodes[0], "FK_FOO", "Forum");
            AssertRemoveTable(downgradeNode.ChildNodes[1], "Forum");

            AssertVersionNode(astNode.ChildNodes[6], 20090226100554);
            AssertDowngradeNode(astNode.ChildNodes[6].ChildNodes[1]);

            downgradeNode = (IDowngradeNode)astNode.ChildNodes[6].ChildNodes[1];

            AssertAddIndex(downgradeNode.ChildNodes[0], "IX_Login", null, true,
                new IndexColumnDefinition("ID"), 
                new IndexColumnDefinition("Login", SortDirection.Descending));

            AssertVersionNode(astNode.ChildNodes[9], 20090226103435);
            AssertDowngradeNode(astNode.ChildNodes[9].ChildNodes[1]);

            downgradeNode = (IDowngradeNode)astNode.ChildNodes[9].ChildNodes[1];

            AssertAddReference(downgradeNode.ChildNodes[0], "FK_FOO", 
                "User", new string[] { "ID"},
                "Forum", new string[] { "ModeratorUserID" });
            AssertAlterTable(downgradeNode.ChildNodes[1], "Forum");
            AssertAlterColumn(downgradeNode.ChildNodes[1].ChildNodes[0], "Slug", "Forum", DbType.String, 200, null, null, null, null, false);
            AssertRemoveColumn(downgradeNode.ChildNodes[1].ChildNodes[1], "Slug", "Forum");
            AssertAddColumn(downgradeNode.ChildNodes[1].ChildNodes[2], "Slug", "Forum", DbType.String, 200, null, null, null, null, false);
            AssertRemoveColumn(downgradeNode.ChildNodes[1].ChildNodes[3], "Slug", "Forum");

            AssertVersionNode(astNode.ChildNodes[10], 20090227103435);
            AssertDowngradeNode(astNode.ChildNodes[10].ChildNodes[1]);

            downgradeNode = (IDowngradeNode)astNode.ChildNodes[10].ChildNodes[1];

            AssertAddTable(downgradeNode.ChildNodes[0], "Forum",
                new ColumnDefinition("ID", "Forum", DbType.Int32, false, null, null, null, true, true),
                new ColumnDefinition("Name", "Forum", DbType.String, false, 200, null, null, null, null),
                new ColumnDefinition("ModeratorUserID", "Forum", DbType.Int32, false, null, null, null, null, null),
                new ColumnDefinition("Slug", "Forum", DbType.String, false, 200, null, null, null, null));
        }

        private IAstNode GetAstNode()
        {
            IAstNode astNode;
            using(Stream resourceStream = 
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Blog.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                astNode = mdlParser.Parse();
            } // using

            //
            // Process type aliases
            astNode.Accept(new TypeAliasResolutionCompilerStage());

            //
            // Resolve PKs
            astNode.Accept(new PrimaryKeyResolutionCompilerStage());

            //
            // Bind stuff
            BindingCompilerStage bindingCompilerStage = new BindingCompilerStage();
            bindingCompilerStage.SetEnvironment(new Wizardby.Core.Compiler.Environment());

            astNode.Accept(bindingCompilerStage);

            //
            // Flatten AST
            astNode.Accept(new AstFlattenerCompilerStage());

            //
            // We also need to generate upgrade since that's what downgrade generator is expecting
            IMdlCompilerStage upgradeGenerationStage = new UpgradeGenerationStage();
            astNode.Accept(upgradeGenerationStage);

            IMdlCompilerStage downgradeGenerationStage = new DowngradeGenerationStage();
            downgradeGenerationStage.SetEnvironment(new Wizardby.Core.Compiler.Environment());
            
            astNode.Accept(downgradeGenerationStage);
            
            return astNode;
        }
    }
}
