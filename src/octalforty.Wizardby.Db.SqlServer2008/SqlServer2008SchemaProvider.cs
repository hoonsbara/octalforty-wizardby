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
using System.Data.SqlClient;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.SemanticModel;
using octalforty.Wizardby.Db.SqlServer2000;

namespace octalforty.Wizardby.Db.SqlServer2008
{
    /// <summary>
    /// A <see cref="IDbSchemaProvider"/> for Microsoft SQL Server 2008.
    /// </summary>
    public class SqlServer2008SchemaProvider : SqlServer2000SchemaProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2008SchemaProvider"/> class.
        /// </summary>
        public SqlServer2008SchemaProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2008SchemaProvider"/> class.
        /// </summary>
        /// <param name="platform"></param>
        public SqlServer2008SchemaProvider(IDbPlatform platform) :
            base(platform)
        {
        }

        #region InformationSchemaSchemaProvider Members
        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// references in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        protected override void GetReferenceDefinitions(string connectionString, Core.SemanticModel.Schema databaseSchema)
        {
            WithDatabase(connectionString, delegate(Database database)
            {
                database.PrefetchObjects(typeof(Table));

                foreach (Table table in database.Tables)
                {
                    if (table.IsSystemObject)
                        continue;

                    foreach (ForeignKey foreignKey in table.ForeignKeys)
                    {
                        if (foreignKey.IsSystemNamed)
                            continue;

                        IReferenceDefinition referenceDefinition = GetReferenceDefinition(table, foreignKey);

                        ITableDefinition fkTable =
                            databaseSchema.GetTable(referenceDefinition.FkTableSchema, referenceDefinition.FkTable);
                        fkTable.AddReference(referenceDefinition);
                    } // foreach
                } // foreach
            });
        }

        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// indexes in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        protected override void GetIndexDefinitions(string connectionString, Core.SemanticModel.Schema databaseSchema)
        {
            WithDatabase(connectionString, delegate(Database database)
            {
                database.PrefetchObjects(typeof(Table));

                foreach (Table table in database.Tables)
                {
                    if (table.IsSystemObject)
                        continue;

                    foreach (Index index in table.Indexes)
                    {
                        if (index.IsSystemNamed)
                            continue;

                        IIndexDefinition indexDefinition = GetIndexDefinition(table, index);
                        ITableDefinition indexTable = databaseSchema.GetTable(table.Schema, table.Name);

                        indexTable.AddIndex(indexDefinition);
                    } // foreach
                } // foreach
            });
        }
        #endregion

        private static IIndexDefinition GetIndexDefinition(Table table, Index index)
        {
            IIndexDefinition indexDefinition = new IndexDefinition(index.Name);
            indexDefinition.Clustered = index.IsClustered;
            indexDefinition.Unique = index.IsUnique;

            foreach (IndexedColumn indexedColumn in index.IndexedColumns)
            {
                indexDefinition.Columns.Add(new IndexColumnDefinition(indexedColumn.Name,
                    indexedColumn.Descending ?
                        SortDirection.Descending :
                        SortDirection.Ascending));
            } // foreach

            return indexDefinition;
        }

        private static IReferenceDefinition GetReferenceDefinition(Table table, ForeignKey foreignKey)
        {
            IReferenceDefinition referenceDefinition = new ReferenceDefinition(foreignKey.Name);
            referenceDefinition.PkTableSchema = foreignKey.ReferencedTableSchema;
            referenceDefinition.PkTable = foreignKey.ReferencedTable;
            referenceDefinition.FkTableSchema = table.Schema;
            referenceDefinition.FkTable = table.Name;

            foreach (ForeignKeyColumn foreignKeyColumn in foreignKey.Columns)
            {
                referenceDefinition.FkColumns.Add(foreignKeyColumn.Name);
                referenceDefinition.PkColumns.Add(foreignKeyColumn.ReferencedColumn);
            } // foreach

            return referenceDefinition;
        }

        private void WithDatabase(string connectionString, Action<Database> action)
        {
            using (SqlConnection sqlConnection = (SqlConnection)Platform.ProviderFactory.CreateConnection())
            {
                sqlConnection.ConnectionString = connectionString;
                sqlConnection.Open();

                SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

                Server server = new Server(new ServerConnection(sqlConnection));
                Database database = server.Databases[connectionStringBuilder.InitialCatalog];

                action(database);
            } // using
        }
    }
}
