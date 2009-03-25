#region The MIT License
// The MIT License
// 
// Copyright (c) 2009 octalforty studios
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SqlServer
{
    /// <summary>
    /// Microsoft SQL Server specific implementation of the <see cref="IDbExecutive"/>.
    /// </summary>
    public class SqlServerExecutive : IDbExecutive
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
        /// Initializes a new instance of the <see cref="SqlServerExecutive"/> class.
        /// </summary>
        public SqlServerExecutive()
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
        /// Executes the given <paramref name="dbOperation"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbOperation"></param>
        /// <returns></returns>
        public T Execute<T>(DbOperation<T> dbOperation)
        {
            return InternalExecute(dbOperation);
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

        private T InternalExecute<T>(DbOperation<T> dbOperation)
        {
            try
            {
                return dbOperation();
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
