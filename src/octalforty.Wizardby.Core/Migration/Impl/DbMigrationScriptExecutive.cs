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
using octalforty.Wizardby.Core.Util;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    public class DbMigrationScriptExecutive : IMigrationScriptExecutive
    {
        public event MigrationScriptExecutionEventHandler Migrating;

        private void InvokeMigrating(MigrationScriptExecutionEventArgs args)
        {
            MigrationScriptExecutionEventHandler migratingHandler = Migrating;
            if(migratingHandler != null) migratingHandler(this, args);
        }

        public event MigrationScriptExecutionEventHandler Migrated;

        private void InvokeMigrated(MigrationScriptExecutionEventArgs args)
        {
            MigrationScriptExecutionEventHandler migratedHandler = Migrated;
            if(migratedHandler != null) migratedHandler(this, args);
        }

        public void ExecuteMigrationScripts(IDbPlatform dbPlatform, IMigrationVersionInfoManager migrationVersionInfoManager, 
            string connectionString, MigrationScriptCollection migrationScripts, 
            long? currentVersion, long? targetVersion, MigrationMode migrationMode)
        {
            using(IDbConnection dbConnection = dbPlatform.ProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();

                IList<MigrationScript> effectiveMigrationScripts = 
                    new List<MigrationScript>(GetMigrationScripts(migrationScripts, currentVersion, targetVersion, migrationMode));
                foreach(MigrationScript migrationScript in effectiveMigrationScripts)
                {
                    using(IDbTransaction ddlTransaction = BeginDdlTransaction(dbPlatform, dbConnection))
                    {
                        using(IDbCommand dbCommand = dbConnection.CreateCommand())
                        {
                            dbCommand.CommandText = migrationScript.DdlScript;
                            dbCommand.CommandType = CommandType.Text;

                            //
                            // Workaround for Jet
                            if(!(ddlTransaction is NullDbTransaction))
                                dbCommand.Transaction = ddlTransaction;

                            //Console.WriteLine(dbCommand.CommandText);

                            InvokeMigrating(new MigrationScriptExecutionEventArgs(migrationMode, migrationScript.MigrationVersion));

                            dbCommand.ExecuteNonQuery();

                            InvokeMigrated(new MigrationScriptExecutionEventArgs(migrationMode, migrationScript.MigrationVersion));
                        } // using

                        //
                        // If we're downgrading all way down, do not register it, since SchemaInfo
                        // will be deleted too.
                        if(!(migrationMode == MigrationMode.Downgrade && targetVersion == 0 && effectiveMigrationScripts.IndexOf(migrationScript) ==
                            effectiveMigrationScripts.Count - 1))
                            migrationVersionInfoManager.RegisterMigrationVersion(ddlTransaction, migrationMode,
                                migrationScript.MigrationVersion);

                        ddlTransaction.Commit();
                    } // using
                } // foreach
            } // foreach
        }

        private IEnumerable<MigrationScript> GetMigrationScripts(MigrationScriptCollection migrationScripts, 
            long? currentVersion, long? targetVersion, MigrationMode migrationMode)
        {
            if(migrationMode == MigrationMode.Upgrade)
            {
                foreach(MigrationScript ms in migrationScripts)
                    if((!currentVersion.HasValue || currentVersion.Value < ms.MigrationVersion) &&
                        (!targetVersion.HasValue || targetVersion.Value >= ms.MigrationVersion))
                        yield return ms;
            } // if
            else
            {
                foreach(MigrationScript ms in Algorithms.Reverse(migrationScripts))
                    if((!currentVersion.HasValue || currentVersion.Value >= ms.MigrationVersion) &&
                        (!targetVersion.HasValue || targetVersion.Value < ms.MigrationVersion))
                        yield return ms;
            } // else
        }

        private static IDbTransaction BeginDdlTransaction(IDbPlatform dbPlatform, IDbConnection connection)
        {
            return dbPlatform.SupportsTransactionalDdl ? 
                connection.BeginTransaction() :
                new NullDbTransaction(connection);
        }
    }
}
