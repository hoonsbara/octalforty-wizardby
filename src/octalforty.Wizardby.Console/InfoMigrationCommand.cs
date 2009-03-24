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
        /// <param name="platformRegistry"></param>
        /// <param name="parameters"></param>
        protected override void InternalExecute(DbPlatformRegistry platformRegistry, MigrationParameters parameters)
        {
            IDbPlatform platform = ResolveDbPlatform(platformRegistry, parameters);

            IMigrationVersionInfoManager migrationVersionInfoManager = 
                new DbMigrationVersionInfoManager(platform, "SchemaInfo");

            long? currentMigrationVersion = 
                migrationVersionInfoManager.GetCurrentMigrationVersion(parameters.ConnectionString);
            IList<long> registeredMigrationVersions = 
                migrationVersionInfoManager.GetAllRegisteredMigrationVersions(parameters.ConnectionString);

            if(!currentMigrationVersion.HasValue)
                System.Console.WriteLine(Resources.DatabaseIsNotVersioned);
        }
        #endregion
        
        private static IDbPlatform ResolveDbPlatform(DbPlatformRegistry platformRegistry, MigrationParameters parameters)
        {
            IDbPlatform platform = platformRegistry.ResolvePlatform(parameters.PlatformAlias);
            if(platform == null)
                throw new MigrationException(
                    string.Format(Resources.CouldNotResolvePlatformAlias, parameters.PlatformAlias));
            
            return platform;
        }
    }
}
