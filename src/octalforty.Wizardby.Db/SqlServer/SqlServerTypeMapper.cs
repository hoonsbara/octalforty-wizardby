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
using octalforty.Wizardby.Db.Properties;

namespace octalforty.Wizardby.Db.SqlServer
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/en-us/library/cc716729.aspx for information on mapping.
    /// </remarks>
    public class SqlServerTypeMapper : DbTypeMapperBase
    {
        private const string Bit = "bit";
        private const string Tinyint = "tinyint";
        private const string Varchar = "varchar";
        private const string Char = "char";
        private const string Varbinary = "varbinary";
        private const string Nvarchar = "nvarchar";
        private const string Nchar = "nchar";

        public SqlServerTypeMapper()
        {
            RegisterTypeMapping(DbType.Boolean, Bit);
            RegisterTypeMapping(DbType.Byte, Tinyint);
            RegisterTypeMapping(DbType.Currency, "money");
            RegisterTypeMapping(DbType.DateTime, "datetime");
            RegisterTypeMapping(DbType.Date, "datetime");
            RegisterTypeMapping(DbType.Decimal, "decimal");
            RegisterTypeMapping(DbType.Double, "float");
            RegisterTypeMapping(DbType.Guid, "uniqueidentifier");
            RegisterTypeMapping(DbType.Int16, "smallint");
            RegisterTypeMapping(DbType.Int32, "int");
            RegisterTypeMapping(DbType.Int64, "bigint");
            RegisterTypeMapping(DbType.Object, "sql_variant");
            RegisterTypeMapping(DbType.Single, "real");
            RegisterTypeMapping(DbType.Time, "rowversion");
            RegisterTypeMapping(DbType.Xml, "xml");
        }

        #region DbTypeMapperBase Members
        protected override string MapToNativeTypeCore(DbType logicalType, int? length)
        {
            switch(logicalType)
            {
                case DbType.AnsiString:
                    return Varchar;
                case DbType.AnsiStringFixedLength:
                    return Char;
                case DbType.Binary:
                    return Varbinary;
                case DbType.String:
                    return Nvarchar;
                case DbType.StringFixedLength:
                    return Nchar;
                default:
                    return base.MapToNativeTypeCore(logicalType, length);
            } // switch
        }

        protected override string FormatBareNativeType(string bareNativeType, int? length)
        {
            if(Array.IndexOf(new string[] { Varchar, Char, Varbinary, Nvarchar, Nchar }, bareNativeType) != -1 && !length.HasValue)
                return string.Format("{0}(max)", bareNativeType);

            return base.FormatBareNativeType(bareNativeType, length);
        }

        protected override string MapToNativeType(DbType logicalType)
        {
            throw new DbPlatformException(string.Format(Resources.UnknownDataType, logicalType));
        }

        /// <summary>
        /// Maps <paramref name="nativeType"/> to an appropriate <see cref="DbType"/> value.
        /// </summary>
        /// <param name="nativeType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override DbType? MapToDbType(string nativeType, int? length)
        {
            switch(nativeType.ToLowerInvariant())
            {
                case Varchar:
                    return DbType.AnsiString;
                case Char:
                    return DbType.AnsiStringFixedLength;
                case Varbinary:
                    return DbType.Binary;
                case Nvarchar:
                    return DbType.String;
                case Nchar:
                    return DbType.StringFixedLength;
            } // switch

            return base.MapToDbType(nativeType, length);
        }
        #endregion
    }
}