using System;

using ADOX;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.Jet
{
    public class JetDeploymentManager : DbPlatformDependencyBase, IDbDeploymentManager
    {
        public void Deploy(string connectionString)
        {
            CatalogClass catalog = new CatalogClass();
            string adoxConnectionString = connectionString + "Jet OLEDB;Engine Type=5";

            Console.WriteLine(adoxConnectionString);

            catalog.Create(adoxConnectionString);

            catalog = null;
        }
    }
}
