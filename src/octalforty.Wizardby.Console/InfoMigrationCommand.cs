using System;
using System.Collections.Generic;

using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;

namespace octalforty.Wizardby.Console
{
    /// <summary>
    /// Implements <see cref="MigrationCommand.Info"/> command logic.
    /// </summary>
    [MigrationCommand(MigrationCommand.Info)]
    public class InfoMigrationCommand : MigrationCommandBase
    {
        #region MigrationCommandBase Members
        /// <summary>
        /// Executes the current command.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dbPlatform"></param>
        protected override void InternalExecute(MigrationParameters parameters, IDbPlatform dbPlatform)
        {
            IMigrationVersionInfoManager migrationVersionInfoManager = 
                new DbMigrationVersionInfoManager(dbPlatform, "SchemaInfo");

            long? currentMigrationVersion = 
                migrationVersionInfoManager.GetCurrentMigrationVersion(parameters.ConnectionString);
            IList<long> registeredMigrationVersions = 
                migrationVersionInfoManager.GetAllRegisteredMigrationVersions(parameters.ConnectionString);

            if(!currentMigrationVersion.HasValue)
            {
                using(new ConsoleStylingScope(ConsoleColor.Yellow))
                    System.Console.WriteLine(Environment.NewLine + Resources.DatabaseIsNotVersioned);

                return;
            } // if

            using(new ConsoleStylingScope(ConsoleColor.Green))
            {
                System.Console.WriteLine(Environment.NewLine + Resources.CurrentDatabaseVersionInfo, currentMigrationVersion.Value);

                if(registeredMigrationVersions.Count == 0)
                    return;

                System.Console.WriteLine(Resources.RegisteredDatabaseVersionsInfo);
                foreach(long registeredVersion in registeredMigrationVersions)
                    System.Console.WriteLine(Resources.RegisteredDatabaseVersionInfo, registeredVersion);
            } // using
        }
        #endregion
    }
}
