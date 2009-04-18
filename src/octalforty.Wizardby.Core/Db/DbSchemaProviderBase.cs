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
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Provides an abstract base class for <see cref="IDbSchemaProvider"/> implementors.
    /// </summary>
    public abstract class DbSchemaProviderBase : DbPlatformDependencyBase, IDbSchemaProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbSchemaProviderBase"/> class.
        /// </summary>
        protected DbSchemaProviderBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbSchemaProviderBase"/> class.
        /// </summary>
        /// <param name="platform"></param>
        protected DbSchemaProviderBase(IDbPlatform platform) : 
            base(platform)
        {
        }

        #region IDbSchemaProvider Members
        /// <summary>
        /// Introspects the database defined by the <see cref="connectionString"/> and 
        /// returns a <see cref="Schema"/> object, which contains all the schema elements.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual Schema GetSchema(string connectionString)
        {
            Schema databaseSchema = new Schema();

            GetSchemaDefinitions(connectionString, databaseSchema);
            GetTableDefinitions(connectionString, databaseSchema);
            GetReferenceDefinitions(connectionString, databaseSchema);
            GetIndexDefinitions(connectionString, databaseSchema);

            return databaseSchema;
        }
        #endregion

        #region Overridable Members
        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// references in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        protected abstract void GetReferenceDefinitions(string connectionString, Schema databaseSchema);

        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// indexes in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        protected abstract void GetIndexDefinitions(string connectionString, Schema databaseSchema);

        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// schemas in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        /// <returns></returns>
        protected abstract void GetSchemaDefinitions(string connectionString, Schema databaseSchema);

        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// tables, their columns and primary keys in the database defined by the 
        /// <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        /// <returns></returns>
        protected abstract void GetTableDefinitions(string connectionString, Schema databaseSchema);
        #endregion
    }
}
