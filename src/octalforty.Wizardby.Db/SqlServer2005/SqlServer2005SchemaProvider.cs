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
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.SemanticModel;
using octalforty.Wizardby.Db.SqlServer2000;

namespace octalforty.Wizardby.Db.SqlServer2005
{
    /// <summary>
    /// A <see cref="IDbSchemaProvider"/> for Microsoft SQL Server 2005.
    /// </summary>
    public class SqlServer2005SchemaProvider : SqlServer2000SchemaProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2005SchemaProvider"/> class.
        /// </summary>
        public SqlServer2005SchemaProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2005SchemaProvider"/> class.
        /// </summary>
        /// <param name="platform"></param>
        public SqlServer2005SchemaProvider(IDbPlatform platform) : 
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
        protected override void GetReferenceDefinitions(string connectionString, Schema databaseSchema)
        {
            base.GetReferenceDefinitions(connectionString, databaseSchema);
        }

        /// <summary>
        /// When overriden in derived class, retrieves a collection of all
        /// indexes in the database defined by the <see cref="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseSchema"></param>
        protected override void GetIndexDefinitions(string connectionString, Schema databaseSchema)
        {
            base.GetIndexDefinitions(connectionString, databaseSchema);
        }
        #endregion

    }
}
