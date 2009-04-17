using FirebirdSql.Data.FirebirdClient;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.Firebird
{
    public class FirebirdDeploymentManager : DbPlatformDependencyBase, IDbDeploymentManager
    {
        public FirebirdDeploymentManager()
        {
        }

        public FirebirdDeploymentManager(IDbPlatform platform) : 
            base(platform)
        {
        }

        public void Deploy(string connectionString)
        {
            FbConnection.CreateDatabase(connectionString);
        }
    }
}
