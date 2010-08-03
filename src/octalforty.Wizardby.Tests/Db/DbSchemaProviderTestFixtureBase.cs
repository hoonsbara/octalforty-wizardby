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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Core.SemanticModel;

using NUnit.Framework;

namespace octalforty.Wizardby.Tests.Db
{
    public abstract class DbSchemaProviderTestFixtureBase<TDbPlatform>
        where TDbPlatform : IDbPlatform, new()
    {
        #region Private Fields
        private IDbPlatform dbPlatform;
        private string connectionString;
        private IMigrationService migrationService;
        #endregion

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            dbPlatform = new TDbPlatform();
            connectionString = ConfigurationManager.AppSettings["connectionString"];

            migrationService = new MigrationService(
                dbPlatform,
                new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo"),
                new DbMigrationScriptExecutive(new DbCommandExecutionStrategy()),
                null);

            try
            {
                MigrateTo(null);
            }
            catch(Exception e)
            {
                MigrateTo(0);
                Assert.Fail(e.Message);
            }
        }

        [TestFixtureTearDown()]
        public void TestFixtureTearDown()
        {
            MigrateTo(0);
        }

        [Test()]
        public void GetSchema()
        {
            Schema schema = dbPlatform.SchemaProvider.GetSchema(connectionString);

            Assert.AreEqual(9, schema.Tables.Count);

            ITableDefinition authorTable = schema.GetTable("dbo", "Author");
         
            var primaryKeyColumn = authorTable.GetPrimaryKeyColumns()[0];
            
            Assert.AreSame(authorTable.GetColumn("ID"), primaryKeyColumn);
            Assert.AreEqual(DbType.Int32, primaryKeyColumn.Type.Value);
            Assert.IsFalse(primaryKeyColumn.Nullable.Value);
            Assert.IsTrue(primaryKeyColumn.PrimaryKey.Value);
            Assert.IsTrue(primaryKeyColumn.Identity.Value);
            Assert.IsFalse(primaryKeyColumn.Scale.HasValue);
            Assert.IsFalse(primaryKeyColumn.Precision.HasValue);

            ITableDefinition blogTable = schema.GetTable("dbo", "Blog");

            IColumnDefinition descriptionColumn = blogTable.GetColumn("Description");
            
            Assert.AreEqual(DbType.String, descriptionColumn.Type.Value);
            Assert.IsFalse(descriptionColumn.Length.HasValue);

            ITableDefinition blogPostTable = schema.GetTable("dbo", "BlogPost");

            IReferenceDefinition fk1Reference = blogPostTable.GetReference("FK1");

            Assert.IsNotNull(fk1Reference);
            Assert.AreEqual("dbo", fk1Reference.PkTableSchema);
            Assert.AreEqual("Blog", fk1Reference.PkTable);
            Assert.AreEqual(1, fk1Reference.PkColumns.Count);
            Assert.AreEqual("ID", fk1Reference.PkColumns[0]);

            Assert.AreEqual("dbo", fk1Reference.FkTableSchema);
            Assert.AreEqual("BlogPost", fk1Reference.FkTable);
            Assert.AreEqual(1, fk1Reference.FkColumns.Count);
            Assert.AreEqual("BlogID", fk1Reference.FkColumns[0]);

            ITableDefinition userTable = schema.GetTable("dbo", "User");

            IIndexDefinition ixLoginIndex = userTable.GetIndex("IX_Login");

            Assert.IsNotNull(ixLoginIndex);
            Assert.IsTrue(ixLoginIndex.Unique.Value);
            Assert.IsFalse(ixLoginIndex.Clustered.Value);
            
            Assert.AreEqual(2, ixLoginIndex.Columns.Count);

            Assert.AreEqual("ID", ixLoginIndex.Columns[0].Name);
            Assert.AreEqual(SortOrder.Ascending, (SortOrder)ixLoginIndex.Columns[0].SortDirection.Value);

            Assert.AreEqual("Login", ixLoginIndex.Columns[1].Name);
            Assert.AreEqual(SortOrder.Descending, (SortOrder)ixLoginIndex.Columns[1].SortDirection.Value);
        }

        private void MigrateTo(int? targetVersion)
        {
            try
            {
                using(Stream resourceStream =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Blog.mdl"))
                {
                    migrationService.Migrate(connectionString, targetVersion, new StreamReader(resourceStream, Encoding.UTF8));
                } // using
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
