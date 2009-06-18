using System;
using System.Data;

using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;

namespace octalforty.Wizardby.Console
{
    /// <summary>
    /// Implements <see cref="MigrationCommand.Register"/> command logic.
    /// </summary>
    [MigrationCommand(MigrationCommand.Register)]
    public class RegisterMigrationCommand : MigrationCommandBase
    {
        public RegisterMigrationCommand() :
            base(true, true, true, true)
        {
        }

        #region MigrationCommandBase Members
        /// <summary>
        /// Executes the current command.
        /// </summary>
        /// <param name="parameters"></param>
        protected override void InternalExecute(MigrationParameters parameters)
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                ServiceProvider.GetService<IMigrationVersionInfoManager>();

            DbUtil.ExecuteInTransaction(ServiceProvider.GetService<IDbPlatform>(),
                parameters.ConnectionString,
                delegate(IDbTransaction transaction)
                    {
                        migrationVersionInfoManager.RegisterMigrationVersion(transaction, MigrationMode.Upgrade, 
                            parameters.VersionOrStep.Value);
                    });

            System.Console.WriteLine();
            using(new ConsoleStylingScope(ConsoleColor.Green))
                System.Console.WriteLine(Resources.RegisteredVersion, parameters.VersionOrStep.Value);
        }
        #endregion
    }
}
