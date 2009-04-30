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
    public static class DbUtil
    {
        /// <summary>
        /// Executes <paramref name="action"/> within the transaction for the <paramref name="dbPlatform"/> 
        /// on the <paramref name="connectionString"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbPlatform"></param>
        /// <param name="connectionString"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T ExecuteInTransaction<T>(IDbPlatform dbPlatform, 
            string connectionString, DbAction<IDbTransaction, T> action)
        {
            return Execute<T>(dbPlatform, connectionString, 
                delegate(IDbConnection dbConnection)
                    {
                        using(IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                        {
                            T result = action(dbTransaction);

                            dbTransaction.Commit();

                            return result;
                        } // using
                    });
        }

        /// <summary>
        /// Executes <paramref name="action"/> within the connection open for the <paramref name="dbPlatform"/>
        /// on the <paramref name="connectionString"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbPlatform"></param>
        /// <param name="connectionString"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T Execute<T>(IDbPlatform dbPlatform, string connectionString, DbAction<IDbConnection, T> action)
        {
            using(IDbConnection dbConnection = dbPlatform.ProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();

                return action(dbConnection);
            } // using
        }
    }
}
