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
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;

namespace octalforty.Wizardby.Tests.Integration
{
    public abstract class DbPlatformIntegrationTestsBase<TDbPlatform>
        where TDbPlatform : class, IDbPlatform, new()
    {
        public const string Oxite = "octalforty.Wizardby.Tests.Resources.Oxite.mdl";

        #region Private Fields
        private string connectionString;
        private IDbPlatform dbPlatform;
        private IMigrationService migrationService;
        private DbMigrationVersionInfoManager migrationVersionInfoManager;
        #endregion

        protected abstract string GetConnectionString();

        [TestFixtureSetUp()]
        public void TestFixtureSetUp()
        {
            dbPlatform = new TDbPlatform();

            connectionString = GetConnectionString();

            migrationVersionInfoManager = new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");
            migrationService = new MigrationService(
                dbPlatform,
                migrationVersionInfoManager,
                new DbMigrationScriptExecutive(new DbCommandExecutionStrategy()),
                null);
        }

        [SetUp()]
        public void SetUp()
        {
            try
            {
                dbPlatform.DeploymentManager.Deploy(connectionString, DbDeploymentMode.Redeploy);
                MigrateTo(Oxite, 0);
            } // try
            catch(Exception e)
            {
                Assert.Fail(e.ToString());
            } // catch
        }

        [Test()]
        public void Migrate()
        {
            try
            {
                MigrateTo(Oxite, null);
            } // try
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            } // catch
        }

        protected void MigrateTo(string migrationDefinition, int? targetVersion)
        {
            WithResource(migrationDefinition,
                stream => migrationService.Migrate(connectionString, targetVersion, new StreamReader(stream, Encoding.UTF8)));
        }

        private void WithResource(string migrationDefinition, Action<Stream> action)
        {
            using(var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(migrationDefinition))
                action(resourceStream);
        }
    }
}