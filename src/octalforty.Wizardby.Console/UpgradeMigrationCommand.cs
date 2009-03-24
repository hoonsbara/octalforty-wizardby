using System;
using System.Diagnostics;
using System.IO;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;

namespace octalforty.Wizardby.Console
{
    [MigrationCommand(MigrationCommand.Upgrade)]
    public class UpgradeMigrationCommand : MigrationCommandBase
    {
        protected override void InternalExecute(MigrationParameters parameters, IDbPlatform dbPlatform)
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                new DbMigrationVersionInfoManager(dbPlatform, "SchemaInfo");
            IMigrationScriptExecutive migrationScriptExecutive = new DbMigrationScriptExecutive();

            Stopwatch stopwatch = null;

            migrationScriptExecutive.Migrating += delegate(object sender, MigrationScriptExecutionEventArgs args1)
                {
                    using(new ConsoleStylingScope(ConsoleColor.Green))
                        System.Console.WriteLine("Upgrading to version {0}", args1.Version);

                    stopwatch = Stopwatch.StartNew();
                };

            migrationScriptExecutive.Migrated += delegate(object sender, MigrationScriptExecutionEventArgs args1)
                {
                    using(new ConsoleStylingScope(ConsoleColor.Green))
                        System.Console.WriteLine("Upgraded to version {0} ({1:N2} sec.)", args1.Version, stopwatch.Elapsed.TotalSeconds);
                };

            IMigrationService migrationService = new MigrationService();

            System.Console.WriteLine();
            using(StreamReader streamReader = new StreamReader(parameters.MdlFileName, true))
                migrationService.Migrate(dbPlatform, parameters.ConnectionString, null, streamReader, 
                    migrationVersionInfoManager, migrationScriptExecutive);
        }
    }
}
