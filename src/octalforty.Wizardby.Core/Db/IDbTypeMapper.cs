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

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Provides functionality for mapping between logical database types (<see cref="DbType"/>) to native database types.
    /// </summary>
    public interface IDbTypeMapper : IDbPlatformDependency
    {
        /// <summary>
        /// Maps <paramref name="logicalType"/> to an appropriate native type name.
        /// </summary>
        /// <param name="logicalType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <remarks>
        /// The <see cref="MapToNativeType(DbType,int?)"/> implementation must take <paramref name="length"/> into account and produce
        /// a valid type name. For example, a SQL Server Type Mapper when asked to map a <see cref="DbType.String"/> of length
        /// 500 must return <c>nvarchar(500)</c>.
        /// </remarks>
        string MapToNativeType(DbType logicalType, int? length);

        /// <summary>
        /// Maps <paramref name="logicalType"/> to an appropriate native type name.
        /// </summary>
        /// <param name="logicalType"></param>
        /// <param name="scale"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        /// <remarks>
        /// The <see cref="MapToNativeType(DbType,int?,int?)"/> implementation must take <paramref name="scale"/> and <paramref name="precision"/>
        /// into account and produce a valid type name. For example, a SQL Server Type Mapper when asked to map a <see cref="DbType.Decimal"/> of 
        /// with scale 30 and precision 10 must return <c>decimal(30,10)</c>.
        /// </remarks>
        string MapToNativeType(DbType logicalType, int? scale, int? precision);

        /// <summary>
        /// Maps <paramref name="nativeType"/> to an appropriate <see cref="DbType"/> value.
        /// </summary>
        /// <param name="nativeType"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        DbType? MapToDbType(string nativeType, int? length);
    }
}
