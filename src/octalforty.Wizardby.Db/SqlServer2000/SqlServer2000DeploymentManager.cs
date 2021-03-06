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
using System.Data.SqlClient;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SqlServer2000
{
    public class SqlServer2000DeploymentManager : DbPlatformDependencyBase, IDbDeploymentManager
    {
        public SqlServer2000DeploymentManager()
        {
        }

        public SqlServer2000DeploymentManager(IDbPlatform platform) : 
            base(platform)
        {
        }

        public void Deploy(string connectionString, DbDeploymentMode deploymentMode)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            
            string originalInitialCatalog = connectionStringBuilder.InitialCatalog;
            connectionStringBuilder.InitialCatalog = "master";

            DbUtil.Execute(Platform, connectionStringBuilder.ToString(),
                delegate(IDbConnection connection)
                    {
                        if(deploymentMode == DbDeploymentMode.Redeploy)
                        {
                            using(IDbCommand dbCommand = connection.CreateCommand())
                            {
                                dbCommand.CommandText = string.Format("if exists(select * from sys.databases where name = '{0}') drop database [{0}]", originalInitialCatalog);
                                dbCommand.ExecuteNonQuery();
                            } // using
                        } // if

                        using(IDbCommand dbCommand = connection.CreateCommand())
                        {
                            dbCommand.CommandText = string.Format("if not exists(select * from sys.databases where name = '{0}') create database [{0}]", originalInitialCatalog);
                            dbCommand.ExecuteNonQuery();
                        } // using
                    });
        }
    }
}