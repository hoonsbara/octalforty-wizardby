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
using octalforty.Wizardby.Db.SQLite;
using octalforty.Wizardby.Db.SqlServer2005;
using octalforty.Wizardby.Tests.Util;

namespace octalforty.Wizardby.Tests.Ci.MSBuild
{
    [TestFixture()]
    public class UpgradeDatabaseTestFixture
    {
        private IDbPlatform dbPlatform;
        private IMigrationVersionInfoManager migrationVersionInfoManager;
        private string connectionString;

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            dbPlatform = new SQLitePlatform();
            migrationVersionInfoManager = new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");
            connectionString = ConfigurationManager.AppSettings["connectionString"];

            dbPlatform.DeploymentManager.Deploy(connectionString, DbDeploymentMode.Redeploy);
        }

        [Test()]
        public void Execute()
        {
            var upgradeDatabase = new UpgradeDatabase
                {
                    DbPlatformType = dbPlatform.GetType().AssemblyQualifiedName,
                    ConnectionString = connectionString,
                    MigrationDefinitionPath = PathUtil.Combine(GetAssemblyLocation(Assembly.GetExecutingAssembly()), "Resources", "Oxite.mdl")
                };

            Assert.IsTrue(upgradeDatabase.Execute());

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627, 20090331140131 },
                GetRegisteredMigrationVersions());

            upgradeDatabase.TargetVersion = 0;
            upgradeDatabase.AllowDowngrade = true;
            Assert.IsTrue(upgradeDatabase.Execute());

            Assert.AreEqual(new long[] { },
                GetRegisteredMigrationVersions());
        }

        private long[] GetRegisteredMigrationVersions()
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
