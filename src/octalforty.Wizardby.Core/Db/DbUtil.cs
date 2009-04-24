using System.Data;

namespace octalforty.Wizardby.Core.Db
{
    public static class DbUtil
    {
        /// <summary>
        /// Executes <paramref name="action"/> within the transaction for the <paramref name="dbPlatform"/> 
        /// on the <paramref name="connectionString"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbPlatform"></param>
        /// <param name="connectionString"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T ExecuteInTransaction<T>(IDbPlatform dbPlatform, string connectionString, DbAction<IDbTransaction, T> action)
        {
            using(IDbConnection dbConnection = dbPlatform.ProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();

                using(IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    return action(dbTransaction);
            } // using
        }
    }
}
