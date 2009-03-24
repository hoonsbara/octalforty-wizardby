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
using System.Data;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    /// <summary>
    /// Implementation of the <see cref="IMigrationVersionInfoManager"/> which uses a table to track
    /// version information.
    /// </summary>
    public class DbMigrationVersionInfoManager : IMigrationVersionInfoManager
    {
        #region Private Fields
        private readonly IDbPlatform dbPlatform;
        private readonly string tableName;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DbMigrationVersionInfoManager"/> class.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="tableName"></param>
        public DbMigrationVersionInfoManager(IDbPlatform dbPlatform, string tableName)
        {
            this.dbPlatform = dbPlatform;
            this.tableName = tableName;
        }

        #region IMigrationVersionInfoManager Members
        /// <summary>
        /// Returns a collection of all registered versions for the given <paramref name="dbTransaction"/> or
        /// empty collection if no migration versions were registered.
        /// </summary>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public IList<long> GetAllRegisteredMigrationVersions(IDbTransaction dbTransaction)
        {
            List<long> registeredVersions = new List<long>();

            try
            {
                using(IDbCommand dbCommand = PrepareGetAllRegisteredMigrationVersionsCommand(dbTransaction))
                    using(IDataReader dataReader = dbPlatform.CommandExecutive.ExecuteReader(dbCommand))
                        while(dataReader.Read())
                        {
                            int ordinal = dataReader.GetOrdinal("Version");
                            registeredVersions.Add(dataReader.GetInt64(ordinal));
                        } // while

                registeredVersions.Sort();
            } // try
            
            catch(Exception)
            {
                // TODO: We're assuming "SchemaInfo" table simply does not exist. Reconsider?
            } // catch

            return registeredVersions;
        }

        /// <summary>
        /// Returns a collection of all registered versions for the given <paramref name="connectionString"/> or
        /// empty collection if no migration versions were registered.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public IList<long> GetAllRegisteredMigrationVersions(string connectionString)
        {
            return Execute<IList<long>>(connectionString,
                delegate(IDbTransaction dbTransaction) { return GetAllRegisteredMigrationVersions(dbTransaction); });
        }

        /// <summary>
        /// Returns a value which contains the maximum migration version for the given <paramref name="dbTransaction"/>
        /// or <c>null</c> if no versioning information is present.
        /// </summary>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public long? GetCurrentMigrationVersion(IDbTransaction dbTransaction)
        {
            IList<long> registeredMigrationVersions = GetAllRegisteredMigrationVersions(dbTransaction);
            return registeredMigrationVersions.Count == 0 ?
                null :
                (long?)registeredMigrationVersions[registeredMigrationVersions.Count - 1];
        }

        /// <summary>
        /// Returns a value which contains the maximum migration version for the given <paramref name="connectionString"/>
        /// or <c>null</c> if no versioning information is present.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public long? GetCurrentMigrationVersion(string connectionString)
        {
            return Execute<long?>(connectionString,
                delegate(IDbTransaction dbTransaction) { return GetCurrentMigrationVersion(dbTransaction); });
        }

        /// <summary>
        /// Registers the fact of migrating to version <paramref name="version"/> with mode <paramref name="migrationMode"/>
        /// for the given <paramref name="dbTransaction"/>.
        /// </summary>
        /// <param name="dbTransaction"></param>
        /// <param name="migrationMode"></param>
        /// <param name="version"></param>
        public void RegisterMigrationVersion(IDbTransaction dbTransaction, MigrationMode migrationMode, long version)
        {
            switch(migrationMode)
            {
                case MigrationMode.Upgrade:
                    RegisterUpgradeMigrationVersion(dbTransaction, version);
                    break;
                case MigrationMode.Downgrade:
                    RegisterDowngradeMigrationVersion(dbTransaction, version);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("migrationMode");
            }
        }
        #endregion

        private T Execute<T>(string connectionString, Converter<IDbTransaction, T> action)
        {
            using(IDbConnection dbConnection = dbPlatform.ProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();

                using(IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    return action(dbTransaction);
            } // using
        }

        private void RegisterDowngradeMigrationVersion(IDbTransaction dbTransaction, long version)
        {
            using(IDbCommand dbCommand = PrepareRegisterDowngradeMigrationVersionCommand(dbTransaction, version))
                ExecuteRegisterMigrationVersionCommand(dbCommand);
        }

        private void RegisterUpgradeMigrationVersion(IDbTransaction dbTransaction, long version)
        {
            using(IDbCommand dbCommand = PrepareRegisterUpgradeMigrationVersionCommand(dbTransaction, version))
                ExecuteRegisterMigrationVersionCommand(dbCommand);
        }

        private void ExecuteRegisterMigrationVersionCommand(IDbCommand dbCommand)
        {
            dbPlatform.CommandExecutive.ExecuteNonQuery(dbCommand);
        }

        private IDbCommand PrepareRegisterDowngradeMigrationVersionCommand(IDbTransaction dbTransaction, long version)
        {
            string commandText = string.Format("delete from {0} where {1} = {2}",
                dbPlatform.Dialect.EscapeIdentifier(tableName), "Version", version);
            return PrepareDbCommand(dbTransaction, commandText);
        }

        private IDbCommand PrepareRegisterUpgradeMigrationVersionCommand(IDbTransaction dbTransaction, long version)
        {
            string commandText = string.Format("insert into {0} ({1}) values ({2})",
                dbPlatform.Dialect.EscapeIdentifier(tableName), "Version", version);
            return PrepareDbCommand(dbTransaction, commandText);
        }

        private IDbCommand PrepareGetAllRegisteredMigrationVersionsCommand(IDbTransaction dbTransaction)
        {
            string commandText = string.Format("select {0} from {1}",
                dbPlatform.Dialect.EscapeIdentifier("Version"), tableName);
            return PrepareDbCommand(dbTransaction, commandText);
        }

        private static IDbCommand PrepareDbCommand(IDbTransaction dbTransaction, string commandText)
        {
            IDbCommand dbCommand = dbTransaction.Connection.CreateCommand();

            dbCommand.CommandText = commandText;
            dbCommand.CommandType = CommandType.Text;
            dbCommand.Transaction = dbTransaction;

            return dbCommand;
        }
    }
}
