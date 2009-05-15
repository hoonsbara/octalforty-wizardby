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
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Core.ReverseEngineering;
using octalforty.Wizardby.Core.ReverseEngineering.Impl;
using octalforty.Wizardby.Core.Util;
using octalforty.Wizardby.Db.SqlServer2005;
using octalforty.Wizardby.Tests.Core.Compiler;
using octalforty.Wizardby.Tests.Core.Compiler.Impl;

namespace octalforty.Wizardby.Tests.Core.ReverseEngineering.Impl
{
    [TestFixture()]
    public class ReverseEngineeringServiceTestFixture : AstTestFixtureBase
    {
        #region Private Fields
        private IDbPlatform dbPlatform;
        private string connectionString;
        private IMigrationService migrationService;
        #endregion

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            dbPlatform = new SqlServer2005Platform();
            connectionString = ConfigurationManager.AppSettings["connectionString"];

            migrationService = new MigrationService(
                dbPlatform,
                new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo"),
                new DbMigrationScriptExecutive(new DbCommandExecutionStrategy()));

            try
            {
                MigrateTo(null);
            } // try
            catch(Exception e)
            {
                MigrateTo(0);
                Assert.Fail(e.Message);
            } // catch
        }

        [TestFixtureTearDown()]
        public void TestFixtureTearDown()
        {
            MigrateTo(0);
        }

        [Test()]
        public void ReverseEngineer()
        {
            IReverseEngineeringService reverseEngineeringService = new ReverseEngineeringService();
            IAstNode astNode = reverseEngineeringService.ReverseEngineer(dbPlatform, connectionString);

            Assert.IsInstanceOfType(typeof(IBaselineNode), astNode);

            IAddTableNode addBlogAuthorJunctionTableNode = 
                (IAddTableNode)Algorithms.FindFirst<IAstNode>(astNode.ChildNodes,
                    delegate(IAstNode an) 
                        { return an is IAddTableNode && ((IAddTableNode)an).Name == "BlogAuthorJunction"; });
            
            Assert.IsNotNull(addBlogAuthorJunctionTableNode);

            AssertAddReference(Algorithms.FindFirst(astNode.ChildNodes,
                delegate(IAstNode an)
                    { return an is IAddReferenceNode && ((IAddReferenceNode)an).Name == "FK12"; }),
                    "FK12", 
                    "Blog", new string[] { "ID" },
                    "BlogAuthorJunction", new string[] { "BlogID" });
            /*Assert.IsNotNull(Algorithms.FindFirst<IAstNode>(addBlogAuthorJunctionTableNode.ChildNodes,
                delegate(IAstNode an)
                    { return an is IAddReferenceNode && ((IAddReferenceNode)an).Name == "FK13"; }));*/

            //
            // Assert that all "add tables" go first, and indexes and references
            // go afterwards
            int lastAddTable = Algorithms.LastIndexOf(astNode.ChildNodes, 
                delegate(IAstNode node) { return node is IAddTableNode; });
            int firstAddReference = Algorithms.FirstIndexOf(astNode.ChildNodes,
                delegate(IAstNode node) { return node is IAddReferenceNode; });

            Assert.IsTrue(lastAddTable < firstAddReference);

            //
            // Ensure that the AST produced by the RES can be compiled
            MdlCompiler mdlCompiler = new MdlCompiler(new NullCodeGenerator(), new Wizardby.Core.Compiler.Environment());
            mdlCompiler.Compile(astNode, MdlCompilationOptions.All);
        }


        private void MigrateTo(int? targetVersion)
        {
            using(Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Blog.mdl"))
            {
                migrationService.Migrate(connectionString, targetVersion, new StreamReader(resourceStream, Encoding.UTF8));
            } // using
        }
    }
}
