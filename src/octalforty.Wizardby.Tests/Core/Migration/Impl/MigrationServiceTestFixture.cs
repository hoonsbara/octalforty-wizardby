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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Db.SqlServer2005;

namespace octalforty.Wizardby.Tests.Core.Migration.Impl
{
    [TestFixture()]
    public class MigrationServiceTestFixture
    {
        #region Public Constants
        public const string Oxite = "octalforty.Wizardby.Tests.Resources.Oxite.mdl";
        public const string OxiteWithReorderedVersions = "octalforty.Wizardby.Tests.Resources.OxiteWithReorderedVersions.mdl";
        public const string OxiteWithMissingVersion = "octalforty.Wizardby.Tests.Resources.OxiteWithMissingVersion.mdl";
        public const string OxiteWithConstraints = "octalforty.Wizardby.Tests.Resources.OxiteWithConstraints.mdl";
        public const string OxiteWithNativeSql = "octalforty.Wizardby.Tests.Resources.OxiteWithNativeSql.mdl";
        #endregion

        #region Private Fields
        private string connectionString;
        private IDbPlatform dbPlatform;
        private IMigrationService migrationService;
        private DbMigrationVersionInfoManager migrationVersionInfoManager;
        #endregion

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            dbPlatform = new SqlServer2005Platform();
            connectionString = ConfigurationManager.AppSettings["connectionString"];

            migrationVersionInfoManager = new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");
            migrationService = new MigrationService(
                dbPlatform,
                migrationVersionInfoManager,
                new DbMigrationScriptExecutive(new DbCommandExecutionStrategy()),
                new FileSystemNativeSqlResourceProvider(
                Path.Combine(
                    GetAssemblyLocation(Assembly.GetExecutingAssembly()), "Resources")));
        }

        [TestFixtureTearDown()]
        public void TestFixtureTearDown()
        {
            MigrateTo(Oxite, 0);
        }

        [SetUp()]
        public void SetUp()
        {
            try
            {
                MigrateTo(Oxite, 0);
            } // try
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            } // catch
        }

        [Test()]
        public void MigrateReorderedVersions()
        {
            MigrateTo(OxiteWithReorderedVersions, null);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627, 20090331140131 },
                GetRegisteredMigrationVersions());

            MigrateTo(OxiteWithReorderedVersions, 0);

            Assert.IsEmpty(GetRegisteredMigrationVersions());
        }

        [Test()]
        public void MigrateMissingVersions()
        {
            MigrateTo(OxiteWithMissingVersion, null);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331140131 },
                GetRegisteredMigrationVersions());

            MigrateTo(Oxite, null);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627, 20090331140131 },
                GetRegisteredMigrationVersions());

            MigrateTo(OxiteWithReorderedVersions, 0);

            Assert.IsEmpty(GetRegisteredMigrationVersions());
        }

        [Test()]
        public void Rollback()
        {
            MigrateTo(Oxite, null);
            Rollback(Oxite, 1);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627 },
                GetRegisteredMigrationVersions());
        }

        [Test()]
        public void RollbackAllDefinedVersions()
        {
            MigrateTo(Oxite, null);
            Rollback(Oxite, 4);

            Assert.IsEmpty(GetRegisteredMigrationVersions());
        }

        [Test()]
        public void RollbackOnEmptyDatabase()
        {
            Rollback(Oxite, 10);
            Assert.IsEmpty(GetRegisteredMigrationVersions());
        }

        [Test()]
        public void RollbackMoreVersionsThanDefined()
        {
            MigrateTo(Oxite, null);
            Rollback(Oxite, 1000);

            Assert.IsEmpty(GetRegisteredMigrationVersions());
        }

        [Test()]
        public void Redo()
        {
            MigrateTo(Oxite, null);
            Redo(Oxite, 2);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627, 20090331140131 },
                GetRegisteredMigrationVersions());
        }

        [Test()]
        public void RedoAllDefinedVersions()
        {
            MigrateTo(Oxite, null);
            Redo(Oxite, 4);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627, 20090331140131 },
                GetRegisteredMigrationVersions());
        }

        [Test()]
        public void RedoOnEmptyDatabase()
        {
            Redo(Oxite, 10);
            Assert.IsEmpty(GetRegisteredMigrationVersions());
        }

        [Test()]
        public void RedoMoreVersionsThanDefined()
        {
            MigrateTo(Oxite, null);
            Redo(Oxite, 1000);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627, 20090331140131 },
                GetRegisteredMigrationVersions());
        }

        [Test()]
        public void UpgradeDowngradeWithConstraints()
        {
            try
            {
                MigrateTo(OxiteWithConstraints, null);
                Redo(OxiteWithConstraints, 1000);
            }
            catch(MigrationException me)
            {
                Assert.Fail(string.Format("{0}: {1}", me.Message, me.SqlStatement));
            }
        }

        [Test()]
        public void MigrateWithNativeSqlScripts()
        {
            try
            {
                MigrateTo(OxiteWithNativeSql, null);

                Assert.AreEqual(1, ExecuteScalar("exec dbo.Sample"));

                Redo(OxiteWithNativeSql, 1000);
            }
            catch(MigrationException me)
            {
                Assert.Fail(string.Format("{0}: {1}", me.Message, me.SqlStatement));
            }
        }

        private long[] GetRegisteredMigrationVersions()
        {
            return new List<long>(
                MigrationVersionInfoManagerUtil.GetRegisteredMigrationVersions(migrationVersionInfoManager, 
                    dbPlatform, connectionString)).ToArray();
        }

        private long GetCurrentMigrationVersion()
        {
            return MigrationVersionInfoManagerUtil.GetCurrentMigrationVersion(migrationVersionInfoManager,
                    dbPlatform, connectionString);
        }

        private void MigrateTo(string migrationDefinition, int? targetVersion)
        {
            WithResource(migrationDefinition, 
                delegate(Stream stream)
                    {
                        migrationService.Migrate(connectionString, targetVersion, new StreamReader(stream, Encoding.UTF8));
                    });
        }

        private void Rollback(string migrationDefinition, int step)
        {
            WithResource(migrationDefinition, 
                delegate(Stream stream)
                    {
                        migrationService.Rollback(connectionString, step, new StreamReader(stream, Encoding.UTF8));
                    });
        }

        private void Redo(string migrationDefinition, int step)
        {
            WithResource(migrationDefinition, 
                delegate(Stream stream)
                    {
                        migrationService.Redo(connectionString, step, new StreamReader(stream, Encoding.UTF8));
                    });
        }

        private void WithResource(string migrationDefinition, Action<Stream> action)
        {
            using (Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(migrationDefinition))
            {
                action(resourceStream);
            } // using
        }

        private object ExecuteScalar(string sql)
        {
            return DbUtil.Execute<object>(dbPlatform, connectionString, 
                delegate(IDbConnection dbConnection)
                    {
                        using(IDbCommand dbCommand = dbConnection.CreateCommand())
                        {
                            dbCommand.CommandText = sql;
                            return dbCommand.ExecuteScalar();
                        } // using
                    });
        }

        private static string GetAssemblyLocation(Assembly assembly)
        {
            return Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
        }
    }
}
