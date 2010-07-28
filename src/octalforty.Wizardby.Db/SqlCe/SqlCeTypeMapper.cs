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

using octalforty.Wizardby.Db.SqlServer2000;

namespace octalforty.Wizardby.Db.SqlCe
{
    /// <summary>
    /// Maps logical database types (<see cref="DbType"/>) to SQL CE-specific types.
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/en-us/library/ms172424(v=SQL.100).aspx
    /// </remarks>
    public class SqlCeTypeMapper : SqlServer2000TypeMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCeTypeMapper"/> class.
        /// </summary>
        public SqlCeTypeMapper()
        {
        }

        protected override string MapToNativeTypeCore(DbType logicalType, int? length)
        {
            if(logicalType == DbType.Binary)
                return length.HasValue ? "binary" : "image";

            if(logicalType == DbType.String || logicalType == DbType.AnsiString)
                return length.HasValue ? "nvarchar" : "ntext";

            return base.MapToNativeTypeCore(logicalType, length);
        }
    }
}
