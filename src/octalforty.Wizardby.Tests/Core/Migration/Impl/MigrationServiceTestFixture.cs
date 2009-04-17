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

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Db.SqlServer2000;

using Rhino.Mocks;

namespace octalforty.Wizardby.Tests.Core.Migration.Impl
{
    [TestFixture()]
    public class MigrationServiceTestFixture
    {
        #region Private Fields
        private string connectionString;
        private IDbPlatform dbPlatform;
        private MockRepository mockRepository;
        #endregion

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            dbPlatform = new SqlServer2000Platform();
            connectionString = ConfigurationManager.AppSettings["connectionString"];
            mockRepository = new MockRepository();
        }

        [Test()]
        public void MigrateUpgrade()
        {
            IMigrationVersionInfoManager migrationVersionInfoManager = 
                mockRepository.StrictMock<IMigrationVersionInfoManager>();
            IMigrationScriptExecutive migrationScriptExecutive =
                mockRepository.StrictMock<IMigrationScriptExecutive>();

            IMigrationService migrationService = new MigrationService(dbPlatform, migrationVersionInfoManager, migrationScriptExecutive);

            using(Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Waffle.mdl"))
            {
                migrationService.Migrate(connectionString, null, new StreamReader(resourceStream, Encoding.UTF8));
            } // using
        }

        [Test()]
        public void MigrateDbUpgrade()
        {
            MigrationVersionInfoManager versionInfoManager = new MigrationVersionInfoManager(3);
            MigrationScriptExecutive scriptExecutive = new MigrationScriptExecutive();

            using(Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Waffle.mdl"))
            {
                IMigrationService migrationService = new MigrationService(dbPlatform, versionInfoManager, scriptExecutive);

                migrationService.Migrate("data source=(local)\\sqlexpress;initial catalog=test;integrated security=sspi", null, new StreamReader(resourceStream, Encoding.UTF8));
            } // using

            Assert.AreEqual(3, scriptExecutive.CurrentVersion.Value);
            Assert.IsFalse(scriptExecutive.TargetVersion.HasValue);
            Assert.AreEqual(MigrationMode.Upgrade, scriptExecutive.MigrationMode);

            Assert.IsNotNull(scriptExecutive.MigrationScripts);
            Assert.AreEqual(3, scriptExecutive.MigrationScripts.Count);
        }

        [Test()]
        //[ExpectedException(typeof(ArgumentNullException))]
        public void MigrateDbThrowsArgumentNullExceptionOnNullConnectionString()
        {
            IMigrationService migrationService = new MigrationService();
            //migrationService.Migrate(new JetPlatform(), null, "migration definition", 0, 1);
        }
    }
}
