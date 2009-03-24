using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Console
{
    /// <summary>
    /// Represents a migration command.
    /// </summary>
    public interface IMigrationCommand
    {
        /// <summary>
        /// Executes the current command.
        /// </summary>
        /// <param name="platformRegistry"></param>
        /// <param name="parameters"></param>
        void Execute(DbPlatformRegistry platformRegistry, MigrationParameters parameters);
    }
}
