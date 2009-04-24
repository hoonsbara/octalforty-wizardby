using System.Collections.Generic;
using System.Data;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Core.Migration
{
    public class MigrationVersionInfoManagerUtil
    {
        public static IList<long> GetRegisteredMigrationVersions(IMigrationVersionInfoManager migrationVersionInfoManager, 
            IDbPlatform dbPlatform, string connectionString)
        {
            return ExecuteInTransaction<IList<long>>(dbPlatform, connectionString, 
                delegate(IDbTransaction transaction)
                    {
                        return migrationVersionInfoManager.GetRegisteredMigrationVersions(transaction);
                    });
        }

        public static long GetCurrentMigrationVersion(IMigrationVersionInfoManager migrationVersionInfoManager,
            IDbPlatform dbPlatform, string connectionString)
        {
            return ExecuteInTransaction<long>(dbPlatform, connectionString, 
                delegate(IDbTransaction transaction)
                    {
                        return migrationVersionInfoManager.GetCurrentMigrationVersion(transaction);
                    });
        }

        private static T ExecuteInTransaction<T>(IDbPlatform dbPlatform, string connectionString,
            DbAction<IDbTransaction, T> action)
        {
            using(IDbConnection dbConnection = dbPlatform.ProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();

                using(IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                {
                    return action(dbTransaction);
                } // using
            } // using
        } 
    }
}
