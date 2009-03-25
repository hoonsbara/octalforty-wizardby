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
using System.Data;

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Standard implementation of the <see cref="IDbExecutive"/>.
    /// </summary>
    public class DbExecutiveBase : IDbExecutive
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
        /// Executes the given <paramref name="dbOperation"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbOperation"></param>
        /// <returns></returns>
        public virtual T Execute<T>(DbOperation<T> dbOperation)
        {
            return dbOperation();
        }

        /// <summary>
        /// Executes the given <paramref name="dbCommand"/> and returns a <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(IDbCommand dbCommand)
        {
            return dbCommand.ExecuteReader();
        }

        /// <summary>
        /// Executes the given <paramref name="dbCommand"/> and returns the number of rows affected.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(IDbCommand dbCommand)
        {
            return dbCommand.ExecuteNonQuery();
        }
        #endregion
    }
}
