using System;
using System.Data.Common;
using System.Data.SQLite;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SQLite
{
    [DbPlatform("SQLite", "sqlite")]
    public class SQLitePlatform : DbPlatformBase<SQLiteDialect, SQLiteConnectionStringBuilder, DefaultDbNamingStrategy, SQLiteTypeMapper>
    {
        public SQLitePlatform() : 
            base(true)
        {
        }

        public override DbProviderFactory ProviderFactory
        {
            get { return SQLiteFactory.Instance; }
        }

        public override IDbDeploymentManager DeploymentManager
        {
            get { return new SQLiteDeploymentManager(this); }
        }
    }
}
