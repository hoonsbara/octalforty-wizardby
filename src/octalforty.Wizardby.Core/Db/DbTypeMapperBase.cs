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
using System.Collections.Generic;
using System.Data;

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Provides base implementation of a <see cref="IDbTypeMapper"/> interface.
    /// </summary>
    public class DbTypeMapperBase : DbPlatformDependencyBase, IDbTypeMapper
    {
        #region Private Fields
        private readonly IDictionary<DbType, string> typeMappings = new Dictionary<DbType, string>();
        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="DbTypeMapperBase"/> class.
        /// </summary>
        public DbTypeMapperBase()
        {
        }

        /// <summary>
        /// Registers a type mapping from <paramref name="logicalType"/> to <paramref name="nativeType"/>.
        /// </summary>
        /// <param name="logicalType"></param>
        /// <param name="nativeType"></param>
        public void RegisterTypeMapping(DbType logicalType, string nativeType)
        {
            typeMappings[logicalType] = nativeType;
        }

        #region IDbTypeMapper Members
        /// <summary>
        /// Maps <paramref name="logicalType"/> to an appropriate physical type name.
        /// </summary>
        /// <param name="logicalType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <remarks>
        /// The <see cref="IDbTypeMapper.MapToNativeType(DbType,int?)"/> implementation must take <paramref name="length"/> into account and produce
        /// a valid type name. For example, a SQL Server Type Mapper when asked to map a <see cref="DbType.String"/> of length
        /// 500 must return <c>nvarchar(500)</c>.
        /// </remarks>
        public virtual string MapToNativeType(DbType logicalType, int? length)
        {
            string bareNativeType = MapToNativeTypeCore(logicalType, length);
            return FormatBareNativeType(bareNativeType, length); 
        }

        /// <summary>
        /// Maps <paramref name="logicalType"/> to an appropriate native type name.
        /// </summary>
        /// <param name="logicalType"></param>
        /// <param name="scale"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        /// <remarks>
        /// The <see cref="IDbTypeMapper.MapToNativeType(DbType,int?,int?)"/> implementation must take <paramref name="scale"/> and <paramref name="precision"/>
        /// into account and produce a valid type name. For example, a SQL Server Type Mapper when asked to map a <see cref="DbType.Decimal"/> of 
        /// with scale 30 and precision 10 must return <c>decimal(30,10)</c>.
        /// </remarks>
        public virtual string MapToNativeType(DbType logicalType, int? scale, int? precision)
        {
            string bareNativeType = MapToNativeTypeCore(logicalType, scale, precision);
            return FormatBareNativeType(bareNativeType, scale, precision);
        }
        #endregion

        protected virtual string FormatBareNativeType(string bareNativeType, int? length)
        {
            return length.HasValue ?
                string.Format("{0}({1})", bareNativeType, length.Value) :
                bareNativeType;
        }

        protected virtual string FormatBareNativeType(string bareNativeType, int? scale, int? precision)
        {
            return scale.HasValue && precision.HasValue ?
                string.Format("{0}({1}, {2})", bareNativeType, scale.Value, precision.Value) :
                bareNativeType;
        }

        protected virtual string MapToNativeTypeCore(DbType logicalType, int? length)
        {
            return typeMappings.ContainsKey(logicalType) ?
                MapToRegisteredNativeType(logicalType) :
                MapToNativeType(logicalType);
        }

        protected virtual string MapToNativeTypeCore(DbType logicalType, int? scale, int? precision)
        {
            return typeMappings.ContainsKey(logicalType) ?
                MapToRegisteredNativeType(logicalType) :
                MapToNativeType(logicalType);
        }

        protected virtual string MapToRegisteredNativeType(DbType logicalType)
        {
            return typeMappings[logicalType];
        }

        protected virtual string MapToNativeType(DbType logicalType)
        {
            return logicalType.ToString().ToLower();
        }
    }
}
