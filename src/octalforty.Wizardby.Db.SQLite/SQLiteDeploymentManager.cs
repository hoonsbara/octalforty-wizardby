using System.Data.SQLite;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SQLite
{
    public class SQLiteDeploymentManager : DbPlatformDependencyBase, IDbDeploymentManager
    {
        public SQLiteDeploymentManager()
        {
        }

        public SQLiteDeploymentManager(IDbPlatform platform) : 
            base(platform)
        {
        }

        public void Deploy(string connectionString)
        {
            System.Data.SQLite.SQLiteConnectionStringBuilder connectionStringBuilder =
                new System.Data.SQLite.SQLiteConnectionStringBuilder(connectionString);
            SQLiteConnection.CreateFile(connectionStringBuilder.DataSource);
        }
    }
}
