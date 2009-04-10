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
using System.Data.Common;

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Provides base implementation of a <see cref="IDbPlatform"/> interface.
    /// </summary>
    /// <typeparam name="TDbConnectionStringBuilder"></typeparam>
    /// <typeparam name="TDbDialect"></typeparam>
    /// <typeparam name="TDbNamingStrategy"></typeparam>
    /// <typeparam name="TDbTypeMapper"></typeparam>
    public abstract class DbPlatformBase<TDbDialect, TDbConnectionStringBuilder, TDbNamingStrategy, TDbTypeMapper> : IDbPlatform
        where TDbDialect : IDbDialect, new()
        where TDbConnectionStringBuilder : IDbConnectionStringBuilder, new()
        where TDbNamingStrategy : IDbNamingStrategy, new()
        where TDbTypeMapper : IDbTypeMapper, new()
    {
        #region Private Fields
        private readonly bool supportsTransactionalDdl;
        private readonly IDbTypeMapper typeMapper;
        private readonly IDbDialect dialect;
        private readonly IDbNamingStrategy namingStrategy;
        #endregion

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="DbPlatformBase{TDbDialect, IDbConnectionStringBuilder, TDbNamingStrategy, TDbTypeMapper}"/> class.
        /// </summary>
        /// <param name="supportsTransactionalDdl"></param>
        protected DbPlatformBase(bool supportsTransactionalDdl)
        {
            this.supportsTransactionalDdl = supportsTransactionalDdl;

            typeMapper = new TDbTypeMapper();
            typeMapper.Platform = this;

            dialect = new TDbDialect();
            dialect.Platform = this;

            namingStrategy = new TDbNamingStrategy();
            namingStrategy.Platform = this;
        }

        #region IDbPlatform Members
        /// <summary>
        /// Gets a value which indicates whether the current platform supports transactional
        /// DDL statements.
        /// </summary>
        public virtual bool SupportsTransactionalDdl
        {
            get { return supportsTransactionalDdl; }
        }
        
        public virtual IDbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            IDbConnectionStringBuilder connectionStringBuilder = new TDbConnectionStringBuilder();
            connectionStringBuilder.Platform = this;

            return connectionStringBuilder;
        }

        public virtual IDbDeploymentManager DeploymentManager
        {
            get { return null; }
        }

        public virtual IDbExceptionTranslator ExceptionTranslator
        {
            get
            {
                DbExceptionTranslator exceptionTranslator = new DbExceptionTranslator();
                exceptionTranslator.Platform = this;

                return exceptionTranslator;
            }
        }

        public virtual IDbTypeMapper TypeMapper
        {
            get { return typeMapper; }
        }

        public virtual IDbDialect Dialect
        {
            get { return dialect; }
        }

        public virtual IDbNamingStrategy NamingStrategy
        {
            get { return namingStrategy; }
        }

        public abstract DbProviderFactory ProviderFactory
        { get; }

        public virtual IDbSchemaProvider SchemaProvider
        {
            get { return null; }
        }
        #endregion
    }
}
