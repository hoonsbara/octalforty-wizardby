using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SqlServer
{
    /// <summary>
    /// Microsoft SQL Server specific implementation of the <see cref="IDbCommandExecutive"/>.
    /// </summary>
    public class SqlServerCommandExecutive : IDbCommandExecutive
    {
        #region Private Fields
        private IDbPlatform platform;
        private Predicate<Exception> exceptionFilter;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a reference to the <see cref="Predicate{T}"/> which is used
        /// to filder exceptions.
        /// </summary>
        public Predicate<Exception> ExceptionFilter
        {
            [DebuggerStepThrough]
            get { return exceptionFilter; }
            [DebuggerStepThrough]
            set { exceptionFilter = value; }
        }
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCommandExecutive"/> class.
        /// </summary>
        public SqlServerCommandExecutive()
        {
            exceptionFilter = delegate(Exception exception)
                { return exception is SqlException; };
        }

        #region IDbCommandExecutive Members
        /// <summary>
        /// Gets or sets a reference to the <see cref="IDbPlatform"/>.
        /// </summary>
        public IDbPlatform Platform
        {
            [DebuggerStepThrough]
            get { return platform; }
            [DebuggerStepThrough]
            set { platform = value; }
        }

        /// <summary>
        /// Executes the given <paramref name="dbCommand"/> and returns a <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(IDbCommand dbCommand)
        {
            return Execute<IDataReader>(
                delegate { return dbCommand.ExecuteReader(); });
        }

        /// <summary>
        /// Executes the given <paramref name="dbCommand"/> and returns the number of rows affected.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(IDbCommand dbCommand)
        {
            return Execute<int>(
                delegate { return dbCommand.ExecuteNonQuery(); });
        }
        #endregion

        private T Execute<T>(Impl.Action<T> action)
        {
            try
            {
                return action();
            } // try

            catch(Exception e)
            {
                if(exceptionFilter(e))
                    throw new DbPlatformException(e.Message, e);

                throw;
            } // catch
        }
    }
}
