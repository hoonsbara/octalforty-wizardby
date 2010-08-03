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

using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// A <see cref="IDbSchemaProvider"/> which uses <c>INFORMATION_SCHEMA</c> view
    /// to retrieve schema information.
    /// </summary>
    public class InformationSchemaSchemaProvider : DbSchemaProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformationSchemaSchemaProvider"/> class.
        /// </summary>
        public InformationSchemaSchemaProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationSchemaSchemaProvider"/> class.
        /// </summary>
        /// <param name="platform"></param>
        public InformationSchemaSchemaProvider(IDbPlatform platform) : 
            base(platform)
        {
        }

        #region DbSchemaProviderBase Members
        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// schemas in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        /// <returns></returns>
        protected override void GetSchemaDefinitions(string connectionString, Schema databaseSchema)
        {
            ExecuteReader(connectionString, "select schema_name from information_schema.schemata",
                delegate(IDataReader dr)
                    { databaseSchema.AddSchema(new SchemaDefinition(As<string>(dr, "schema_name"))); });
        }

        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// tables, their columns and primary keys in the database defined by the 
        /// <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        /// <returns></returns>
        protected override void GetTableDefinitions(string connectionString, Schema databaseSchema)
        {
            //
            // Retrieving a list of tables & their columns.
            ExecuteReader(connectionString, @"select c.* 
from information_schema.tables t 
    inner join information_schema.columns c
        on t.table_catalog = c.table_catalog and t.table_schema = c.table_schema and t.table_name = c.table_name
order by t.table_name, c.ordinal_position", 
                delegate(IDataReader dr)
                    {
                        string schemaName = As<string>(dr, "table_schema");
                        ISchemaDefinition schema = databaseSchema.GetSchema(schemaName);

                        string tableName = As<string>(dr, "table_name");
                        ITableDefinition table = GetTableDefinition(databaseSchema, schema, tableName);

                        IColumnDefinition columnDefinition = new ColumnDefinition((string)dr["column_name"]);
                        
                        string @default = As<string>(dr, "column_default");
                        if(!string.IsNullOrEmpty(@default))
                            table.AddConstraint(new DefaultConstraintDefinition("", table.Name, @default));

                        columnDefinition.Length = As<int?>(dr, "character_maximum_length");

                        //
                        // SQL Server returns -1 as length for (max) values, so fix that
                        /*if(columnDefinition.Length.HasValue && columnDefinition.Length.Value == -1)
                            columnDefinition.Length = null;*/

                        columnDefinition.Nullable = (As<string>(dr, "is_nullable") ?? "NO") == "YES";
                        columnDefinition.Precision = As<byte?>(dr, "numeric_precision");
                        columnDefinition.Scale = As<int?>(dr, "numeric_scale");
                        columnDefinition.Type = 
                            Platform.TypeMapper.MapToDbType(As<string>(dr, "data_type"), 
                                columnDefinition.Length);

                        table.AddColumn(columnDefinition);
                    });

            //
            // Primary keys
            ExecuteReader(connectionString, @"select t.table_schema, t.table_name, k.column_name, k.ordinal_position
	from information_schema.table_constraints t
		inner join information_schema.key_column_usage k on t.constraint_name = k.constraint_name
where t.constraint_type = 'PRIMARY KEY'
order by t.table_schema, t.table_name, k.ordinal_position",
                delegate(IDataReader dr)
                    {
                        string schemaName = As<string>(dr, "table_schema");
                        string tableName = As<string>(dr, "table_name");

                        ITableDefinition table = databaseSchema.GetTable(schemaName, tableName);
                        if(table == null)
                            return;
                        
                        IColumnDefinition column = table.GetColumn(As<string>(dr, "column_name"));
                        column.PrimaryKey = true;
                    });
        }

        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// references in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        protected override void GetReferenceDefinitions(string connectionString, Schema databaseSchema)
        {
        }

        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// indexes in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        protected override void GetIndexDefinitions(string connectionString, Schema databaseSchema)
        {
        }
        #endregion

        private ITableDefinition GetTableDefinition(Schema databaseSchema, ISchemaDefinition schema, string tableName)
        {
            ITableDefinition table = databaseSchema.GetTable(schema, tableName);
            if (table == null)
            {
                table = new TableDefinition(tableName, schema);
                databaseSchema.AddTable(table);
            } // if
            return table;
        }

        protected void ExecuteReader(string connectionString, string sqlStatement, Action<IDataReader> action)
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

        protected static T As<T>(IDataRecord dataReader, string columnName)
        {
            if(dataReader.IsDBNull(dataReader.GetOrdinal(columnName)))
                return default(T);

            return (T)dataReader[columnName];
        }
    }
}
