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

using NUnit.Framework;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;

using octalforty.Wizardby.Db.SqlServer;

namespace octalforty.Wizardby.Tests.Core.Migration.Impl
{
    [TestFixture()]
    public class DbMigrationVersionInfoManagerTestFixture
    {
        #region Private Fields
        private string connectionString;
        private IDbPlatform dbPlatform;
        #endregion

        [TestFixtureSetUp()]
        public void TextFixtureSetUp()
        {
            connectionString = ConfigurationManager.AppSettings["connectionString"];
            dbPlatform = new SqlServerPlatform();
        }

        [Test()]
        public void GetAllRegisteredMigrationVersionsWithMissingTable()
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");
            
            Assert.AreEqual(0, migrationVersionInfoManager.GetAllRegisteredMigrationVersions(connectionString).Count);
        }

        [Test()]
        public void GetCurrentMigrationVersionWithMissingTable()
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");

            Assert.IsNull(migrationVersionInfoManager.GetCurrentMigrationVersion(connectionString));
        }

        [Test()]
        public void GetAllRegisteredMigrationVersions()
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");

            ExecuteInTransaction(delegate(IDbTransaction dbTransaction)
                {
                    Execute(dbTransaction, "create table SchemaInfo (Version bigint);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (1);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (2);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (4);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (7);");

                    IList<long> registeredMigrationVersions = 
                        migrationVersionInfoManager.GetAllRegisteredMigrationVersions(dbTransaction);
                    
                    Assert.AreEqual(4, registeredMigrationVersions.Count);
                    Assert.AreEqual(1, registeredMigrationVersions[0]);
                    Assert.AreEqual(2, registeredMigrationVersions[1]);
                    Assert.AreEqual(4, registeredMigrationVersions[2]);
                    Assert.AreEqual(7, registeredMigrationVersions[3]);

                    Execute(dbTransaction, "drop table SchemaInfo;");
                    
                    return null;
                });
        }

        [Test()]
        public void GetCurrentMigrationVersion()
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");

            ExecuteInTransaction(delegate(IDbTransaction dbTransaction)
                {
                    Execute(dbTransaction, "create table SchemaInfo (Version bigint);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (1);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (2);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (4);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (7);");

                    Assert.AreEqual(7, migrationVersionInfoManager.GetCurrentMigrationVersion(dbTransaction));

                    Execute(dbTransaction, "drop table SchemaInfo;");
                    
                    return null;
                });
        }

        [Test()]
        public void RegisterMigrationVersion()
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo");

            ExecuteInTransaction(delegate(IDbTransaction dbTransaction)
                {
                    Execute(dbTransaction, "create table SchemaInfo (Version bigint);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (1);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (2);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (4);");
                    Execute(dbTransaction, "insert into SchemaInfo (Version) values (7);");

                    migrationVersionInfoManager.RegisterMigrationVersion(dbTransaction, MigrationMode.Upgrade, 8);
                    Assert.AreEqual(8, migrationVersionInfoManager.GetCurrentMigrationVersion(dbTransaction));

                    migrationVersionInfoManager.RegisterMigrationVersion(dbTransaction, MigrationMode.Upgrade, 9);
                    Assert.AreEqual(9, migrationVersionInfoManager.GetCurrentMigrationVersion(dbTransaction));

                    migrationVersionInfoManager.RegisterMigrationVersion(dbTransaction, MigrationMode.Downgrade, 5);
                    Assert.AreEqual(9, migrationVersionInfoManager.GetCurrentMigrationVersion(dbTransaction));

                    migrationVersionInfoManager.RegisterMigrationVersion(dbTransaction, MigrationMode.Downgrade, 9);
                    Assert.AreEqual(8, migrationVersionInfoManager.GetCurrentMigrationVersion(dbTransaction));

                    migrationVersionInfoManager.RegisterMigrationVersion(dbTransaction, MigrationMode.Upgrade, 3);
                    Assert.AreEqual(8, migrationVersionInfoManager.GetCurrentMigrationVersion(dbTransaction));

                    Execute(dbTransaction, "drop table SchemaInfo;");
                    
                    return null;
                });
        }

        public void ExecuteInTransaction(Converter<IDbTransaction, object> action)
        {
            using(IDbConnection dbConnection = dbPlatform.ProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();

                using(IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    action(dbTransaction);
            } // using
        }

        public void Execute(IDbTransaction transaction, string sqlCommandText)
        {
            using(IDbCommand dbCommand = transaction.Connection.CreateCommand())
            {
                dbCommand.Transaction = transaction;
                dbCommand.CommandText = sqlCommandText;
                dbCommand.CommandType = CommandType.Text;

                dbCommand.ExecuteNonQuery();
            } // using
        }
    }
}
