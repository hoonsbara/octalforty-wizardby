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
using System.IO;
using System.Reflection;

using NUnit.Framework;

using octalforty.Wizardby.Ci.MSBuild;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Db.SqlServer2005;
using octalforty.Wizardby.Tests.Core.Migration.Impl;
using octalforty.Wizardby.Tests.Util;

namespace octalforty.Wizardby.Tests.Ci.MSBuild
{
    [TestFixture()]
    public class SynchronizeDatabasesTestFixture
    {
        #region Private Fields
        private IDbPlatform dbPlatform;
        private IMigrationVersionInfoManager migrationVersionInfoManager;
        private string sourceConnectionString;
        private string targetConnectionString;
        private IMigrationService migrationService;
        #endregion

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            dbPlatform = new SqlServer2005Platform();
            migrationVersionInfoManager = new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");
            sourceConnectionString = ConfigurationManager.AppSettings["connectionString"];
            targetConnectionString = ConfigurationManager.AppSettings["secondaryConnectionString"];

            migrationService = new MigrationService(dbPlatform, migrationVersionInfoManager,
                new DbMigrationScriptExecutive(new DbCommandExecutionStrategy()));
        }

        [TestFixtureTearDown()]
        public void TestFixtureTearDown()
        {
            MigrationServiceUtil.MigrateTo(migrationService, sourceConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);
            MigrationServiceUtil.MigrateTo(migrationService, targetConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);
        }

        [SetUp()]
        public void SetUp()
        {
            MigrationServiceUtil.MigrateTo(migrationService, sourceConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);
            MigrationServiceUtil.MigrateTo(migrationService, targetConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);
        }

        [TearDown()]
        public void TearDown()
        {
            MigrationServiceUtil.MigrateTo(migrationService, sourceConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);
            MigrationServiceUtil.MigrateTo(migrationService, targetConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);
        }

        [Test()]
        public void ExecuteUpgrade()
        {
            MigrationServiceUtil.MigrateTo(migrationService, sourceConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, null);
            MigrationServiceUtil.MigrateTo(migrationService, targetConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);

            SynchronizeDatabases synchronizeDatabases = new SynchronizeDatabases();
            synchronizeDatabases.MigrationDefinitionPath = 
                Path.Combine(Path.Combine(GetAssemblyLocation(Assembly.GetExecutingAssembly()), "Resources"), "OxiteWithMissingVersion.mdl");
            synchronizeDatabases.SourceConnectionString = sourceConnectionString;
            synchronizeDatabases.SourceDbPlatformType = string.Format("{0}, {1}",
                typeof(SqlServer2005Platform).FullName, typeof(SqlServer2005Platform).Assembly.GetName().Name);
            synchronizeDatabases.TargetConnectionString = targetConnectionString;

            Assert.IsTrue(synchronizeDatabases.Execute());

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331140131 },
                GetRegisteredMigrationVersions(targetConnectionString));
        }

        [Test()]
        public void ExecuteDowngrade()
        {
            MigrationServiceUtil.MigrateTo(migrationService, sourceConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);
            MigrationServiceUtil.MigrateTo(migrationService, targetConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, null);

            SynchronizeDatabases synchronizeDatabases = new SynchronizeDatabases();
            synchronizeDatabases.AllowDowngrade = true;
            synchronizeDatabases.MigrationDefinitionPath =
                Path.Combine(Path.Combine(GetAssemblyLocation(Assembly.GetExecutingAssembly()), "Resources"), "OxiteWithMissingVersion.mdl");
            synchronizeDatabases.SourceConnectionString = sourceConnectionString;
            synchronizeDatabases.SourceDbPlatformType = string.Format("{0}, {1}",
                typeof(SqlServer2005Platform).FullName, typeof(SqlServer2005Platform).Assembly.GetName().Name);
            synchronizeDatabases.TargetConnectionString = targetConnectionString;

            Assert.IsTrue(synchronizeDatabases.Execute());

            Assert.AreEqual(new long[] { },
                GetRegisteredMigrationVersions(targetConnectionString));
        }

        [Test()]
        public void ExecuteDowngradeWhenNotAllowed()
        {
            MigrationServiceUtil.MigrateTo(migrationService, sourceConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, 0);
            MigrationServiceUtil.MigrateTo(migrationService, targetConnectionString,
                MigrationServiceTestFixture.OxiteWithMissingVersion, null);

            SynchronizeDatabases synchronizeDatabases = new SynchronizeDatabases();
            synchronizeDatabases.MigrationDefinitionPath =
                Path.Combine(Path.Combine(GetAssemblyLocation(Assembly.GetExecutingAssembly()), "Resources"), "OxiteWithMissingVersion.mdl");
            synchronizeDatabases.SourceConnectionString = sourceConnectionString;
            synchronizeDatabases.SourceDbPlatformType = string.Format("{0}, {1}",
                typeof(SqlServer2005Platform).FullName, typeof(SqlServer2005Platform).Assembly.GetName().Name);
            synchronizeDatabases.TargetConnectionString = targetConnectionString;

            Assert.IsFalse(synchronizeDatabases.Execute());
        }

        private long[] GetRegisteredMigrationVersions(string connectionString)
        {
            return new List<long>(
                MigrationVersionInfoManagerUtil.GetRegisteredMigrationVersions(migrationVersionInfoManager,
                    dbPlatform, connectionString)).ToArray();
        }

        private static string GetAssemblyLocation(Assembly assembly)
        {
            return Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
        }
    }
}
