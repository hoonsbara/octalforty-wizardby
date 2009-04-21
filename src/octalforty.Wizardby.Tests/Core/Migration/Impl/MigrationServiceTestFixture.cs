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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
                new DbMigrationScriptExecutive(new DbCommandExecutionStrategy()));
        }

        [Test()]
        public void MigrateReorderedVersions()
        {
            Assert.IsEmpty((ICollection)migrationVersionInfoManager.GetAllRegisteredMigrationVersions(connectionString));
            
            MigrateTo("octalforty.Wizardby.Tests.Resources.OxiteWithReorderedVersions.mdl", null);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627, 20090331140131 },
                new List<long>(migrationVersionInfoManager.GetAllRegisteredMigrationVersions(connectionString)).ToArray());

            MigrateTo("octalforty.Wizardby.Tests.Resources.OxiteWithReorderedVersions.mdl", 0);

            Assert.IsEmpty((ICollection)migrationVersionInfoManager.GetAllRegisteredMigrationVersions(connectionString));
        }

        [Test()]
        public void MigrateMissingVersions()
        {
            Assert.IsEmpty((ICollection)migrationVersionInfoManager.GetAllRegisteredMigrationVersions(connectionString));

            MigrateTo("octalforty.Wizardby.Tests.Resources.OxiteWithMissingVersion.mdl", null);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331140131 },
                new List<long>(migrationVersionInfoManager.GetAllRegisteredMigrationVersions(connectionString)).ToArray());

            MigrateTo("octalforty.Wizardby.Tests.Resources.Oxite.mdl", null);

            Assert.AreEqual(new long[] { 20090323103239, 20090330170528, 20090331135627, 20090331140131 },
                new List<long>(migrationVersionInfoManager.GetAllRegisteredMigrationVersions(connectionString)).ToArray());

            MigrateTo("octalforty.Wizardby.Tests.Resources.OxiteWithReorderedVersions.mdl", 0);

            Assert.IsEmpty((ICollection)migrationVersionInfoManager.GetAllRegisteredMigrationVersions(connectionString));
        }


        private void MigrateTo(string migrationDefinition, int? targetVersion)
        {
            using (Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(migrationDefinition))
            {
                migrationService.Migrate(connectionString, targetVersion, new StreamReader(resourceStream, Encoding.UTF8));
            } // using
        }
    }
}
