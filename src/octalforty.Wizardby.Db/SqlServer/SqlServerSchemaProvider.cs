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

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Db.SqlServer
{
    public class SqlServerSchemaProvider : DbPlatformDependencyBase, IDbSchemaProvider
    {
        public SqlServerSchemaProvider()
        {
        }

        public SqlServerSchemaProvider(IDbPlatform platform) : 
            base(platform)
        {
        }

        public Schema GetSchema(string connectionString)
        {
            Schema schema = new Schema();

            //
            // Retrieving a list of tables & their columns.
            ExecuteReader(connectionString, @"select c.* from information_schema.tables t 
inner join information_schema.columns c
on t.table_catalog = c.table_catalog and t.table_schema = c.table_schema and t.table_name = c.table_name
order by t.table_name, c.ordinal_position", 
                delegate(IDataReader dr)
                    {
                        string tableName = string.Format("{0}.{1}", As<string>(dr, "table_schema"), As<string>(dr, "table_name")); 

                        if(schema.GetTable(tableName) == null)
                            schema.AddTable(new TableDefinition(tableName));

                        IColumnDefinition columnDefinition = new ColumnDefinition((string)dr["column_name"]);
                        columnDefinition.Default = As<string>(dr, "column_default");
                        columnDefinition.Length = As<int?>(dr, "character_maximum_length");

                        //
                        // SQL Server returns -1 as length for (max) values, so fix that
                        if(columnDefinition.Length.HasValue && columnDefinition.Length.Value == -1)
                            columnDefinition.Length = null;

                        columnDefinition.Nullable = (As<string>(dr, "is_nullable") ?? "NO") == "YES";
                        columnDefinition.Precision = As<byte?>(dr, "numeric_precision");
                        columnDefinition.Scale = As<int?>(dr, "numeric_scale");
                        columnDefinition.Type = Platform.TypeMapper.MapToDbType(As<string>(dr, "data_type"), columnDefinition.Length);

                        schema.GetTable(tableName).AddColumn(columnDefinition);
                    });

            return schema;
            
        }

        private void ExecuteReader(string connectionString, string sqlStatement, Action<IDataReader> action)
        {
            using(IDbConnection dbConnection = Platform.ProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();
                
                using(IDbCommand dbCommand = dbConnection.CreateCommand())
                {
                    dbCommand.CommandText = sqlStatement;

                    using(IDataReader dataReader = dbCommand.ExecuteReader())
                    {
                        while(dataReader.Read())
                            action(dataReader);
                    } // using
                } // using
            } // using
        }

        private static T As<T>(IDataRecord dataReader, string columnName)
        {
            if(dataReader.IsDBNull(dataReader.GetOrdinal(columnName)))
                return default(T);

            return (T)dataReader[columnName];
        }
    }
}
