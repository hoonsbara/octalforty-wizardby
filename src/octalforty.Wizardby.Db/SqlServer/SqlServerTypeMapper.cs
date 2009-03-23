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

namespace octalforty.Wizardby.Db.SqlServer
{
    public class SqlServerTypeMapper : DbTypeMapperBase
    {
        public SqlServerTypeMapper()
        {
            RegisterTypeMapping(DbType.Boolean, "bit");
            RegisterTypeMapping(DbType.Byte, "tinyint");
            RegisterTypeMapping(DbType.Int32, "int");
            RegisterTypeMapping(DbType.Int64, "bigint");
            RegisterTypeMapping(DbType.Guid, "uniqueidentifier");
            RegisterTypeMapping(DbType.Time, "rowversion");
        }

        #region DbTypeMapperBase Members
        protected override string MapToNativeTypeCore(DbType logicalType, int? length)
        {
            switch(logicalType)
            {
                case DbType.AnsiString:
                    return "varchar";
                case DbType.AnsiStringFixedLength:
                    return "char";
                case DbType.Binary:
                    return "varbinary";
                case DbType.String:
                    return "nvarchar";
                case DbType.StringFixedLength:
                    return "nchar";
                default:
                    return base.MapToNativeTypeCore(logicalType, length);
            } // switch
        }

        protected override string FormatBareNativeType(string bareNativeType, int? length)
        {
            if(Array.IndexOf(new string[] { "varchar", "char", "varbinary", "nvarchar", "nchar" }, bareNativeType) != -1 && !length.HasValue)
                return string.Format("{0}(max)", bareNativeType);

            return base.FormatBareNativeType(bareNativeType, length);
        }
        #endregion
    }
}