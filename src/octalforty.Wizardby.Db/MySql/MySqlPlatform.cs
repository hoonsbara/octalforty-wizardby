using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.MySql
{
    /// <summary>
    /// A <see cref="IDbPlatform"/> implementation for the MySQL database.
    /// </summary>
    public class MySqlPlatform : DbPlatformBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlPlatform"/> class.
        /// </summary>
        public MySqlPlatform() :
            base(false, null, null)
        {
        }
    }
}
