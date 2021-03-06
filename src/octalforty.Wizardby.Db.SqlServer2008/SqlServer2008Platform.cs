﻿#region The MIT License
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
using octalforty.Wizardby.Db.SqlServer2005;

namespace octalforty.Wizardby.Db.SqlServer2008
{
    /// <summary>
    /// A <see cref="IDbPlatform"/> implementation for the Microsoft SQL Server 2008.
    /// </summary>
    [DbPlatform("Microsoft SQL Server 2008", "sqlserver2008")]
    public class SqlServer2008Platform : SqlServer2005Platform
    {
        public override IDbSchemaProvider SchemaProvider
        {
            get { return new SqlServer2008SchemaProvider(this); }
        }

        public override IDbTypeMapper TypeMapper
        {
            get { return new SqlServer2008TypeMapper(); }
        }

        public override IDbDialect Dialect
        {
            get
            {
                var sqlServer2008Dialect = new SqlServer2008Dialect();
                sqlServer2008Dialect.Platform = this;

                return sqlServer2008Dialect;
            }
        }

        
    }
}
