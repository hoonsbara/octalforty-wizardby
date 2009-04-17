using System.Data.Common;

using FirebirdSql.Data.FirebirdClient;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.Firebird
{
    [DbPlatform("Firebird Embedded", "fbembedded")]
    public class FirebirdEmbeddedPlatform : DbPlatformBase<DbDialectBase, FirebirdEmbeddedConnectionStringBuilder,
        DefaultDbNamingStrategy, FirebirdTypeMapper>
    {
        public FirebirdEmbeddedPlatform() : 
            base(true)
        {
        }

        public override DbProviderFactory ProviderFactory
        {
            get { return FirebirdClientFactory.Instance; }
        }

        public override IDbDeploymentManager DeploymentManager
        {
            get { return new FirebirdDeploymentManager(this); }
        }
    }
}
