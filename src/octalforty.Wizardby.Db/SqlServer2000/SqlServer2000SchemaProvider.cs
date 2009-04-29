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

namespace octalforty.Wizardby.Db.SqlServer2000
{
    /// <summary>
    /// A <see cref="IDbSchemaProvider"/> for Microsoft SQL Server 2000.
    /// </summary>
    public class SqlServer2000SchemaProvider : InformationSchemaSchemaProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000SchemaProvider"/> class.
        /// </summary>
        public SqlServer2000SchemaProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000SchemaProvider"/> class.
        /// </summary>
        /// <param name="platform"></param>
        public SqlServer2000SchemaProvider(IDbPlatform platform) : 
            base(platform)
        {
        }

        #region InformationSchemaSchemaProvider Members
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
            base.GetTableDefinitions(connectionString, databaseSchema);
            FixupColumnLengths(databaseSchema);
            FixupColumnScaleAndPrecision(databaseSchema);
            
            //
            // Identities
            ExecuteReader(connectionString, @"select table_schema, table_name, column_name
	from information_schema.columns
where columnproperty(object_id(quotename(table_schema) + '.' + quotename(table_name)), column_name, 'IsIdentity') = 1", 
                delegate(IDataReader dr)
                    {
                        string schemaName = As<string>(dr, "table_schema");
                        string tableName = As<string>(dr, "table_name");

                        ITableDefinition table = databaseSchema.GetTable(schemaName, tableName);
                        if(table == null)
                            return;

                        IColumnDefinition identityColumn = table.GetColumn(As<string>(dr, "column_name"));
                        identityColumn.Identity = true;
                    });
        }
        #endregion

        private static void FixupColumnScaleAndPrecision(Schema databaseSchema)
        {
            //
            // SQL Server returns scale and precision for all data types without exception. However,
            // docs say that "The precision and scale of the numeric data types besides decimal are fixed".
            foreach(ITableDefinition table in databaseSchema.Tables)
                foreach(IColumnDefinition column in table.Columns)
                    if(column.Type != DbType.Decimal)
                    {
                        column.Scale = null;
                        column.Precision = null;
                    } // if
        }

        private static void FixupColumnLengths(Schema databaseSchema)
        {
            //
            // Fix lengths. SQL Server returns -1 as length for (max) columns.
            foreach(ITableDefinition table in databaseSchema.Tables)
                foreach(IColumnDefinition column in table.Columns)
                    if(column.Length != null && column.Length.Value == -1)
                        column.Length = null;
        }
    }
}
