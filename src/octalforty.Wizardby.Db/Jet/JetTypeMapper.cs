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

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.Jet
{
    /// <summary>
    /// A <see cref="IDbTypeMapper"/> implementation for the Microsoft Jet database.
    /// </summary>
    /// <remarks>
    /// See http://support.microsoft.com/kb/320435 (OleDbType Enumeration vs. Microsoft Access Data Types) for
    /// information on type mappings.
    /// </remarks>
    public class JetTypeMapper : DbTypeMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JetTypeMapper"/> class.
        /// </summary>
        public JetTypeMapper()
        {
            RegisterTypeMapping(DbType.Int32, "INTEGER");
            RegisterTypeMapping(DbType.Int64, "LONG");
            RegisterTypeMapping(DbType.String, "TEXT");
        }
    }
}
