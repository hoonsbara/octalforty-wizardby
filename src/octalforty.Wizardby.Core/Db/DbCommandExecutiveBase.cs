using System.Data;

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Standard implementation of the <see cref="IDbCommandExecutive"/>.
    /// </summary>
    public class DbCommandExecutiveBase : IDbCommandExecutive
    {
        #region Private Fields
        private IDbPlatform platform;
        #endregion

        #region IDbCommandExecutive Members
        /// <summary>
        /// Gets or sets a reference to the <see cref="IDbPlatform"/>.
        /// </summary>
        public IDbPlatform Platform
        {
            get { return platform; }
            set { platform = value; }
        }

        /// <summary>
        /// Executes the given <paramref name="dbCommand"/> and returns a <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(IDbCommand dbCommand)
        {
            return dbCommand.ExecuteReader();
        }

        /// <summary>
        /// Executes the given <paramref name="dbCommand"/> and returns the number of rows affected.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(IDbCommand dbCommand)
        {
            return dbCommand.ExecuteNonQuery();
        }
        #endregion
    }
}
