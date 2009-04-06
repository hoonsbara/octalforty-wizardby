using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SqlServer
{
    public class SqlServerDeploymentManager : DbPlatformDependencyBase, IDbDeploymentManager
    {
        public void Deploy(string connectionString)
        {
            throw new System.NotImplementedException();
        }
    }
}
