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

using NUnit.Framework;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Db.SqlServer;

namespace octalforty.Wizardby.Tests.Db.SqlServer
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/cc716729.aspx
    /// </summary>
    [TestFixture()]
    public class SqlServerTypeMapperTestFixture
    {
        private IDbTypeMapper typeMapper;

        [TestFixtureSetUp()]
        public void TextFixtureSetUp()
        {
            typeMapper = new SqlServerTypeMapper();
        }

        [Test()]
        public void MapToNativeType()
        {
            Assert.AreEqual("varchar(100)", typeMapper.MapToNativeType(DbType.AnsiString, 100));
            Assert.AreEqual("char(100)", typeMapper.MapToNativeType(DbType.AnsiStringFixedLength, 100));
            Assert.AreEqual("varbinary(400)", typeMapper.MapToNativeType(DbType.Binary, 400));
            Assert.AreEqual("bit", typeMapper.MapToNativeType(DbType.Boolean, null));
            Assert.AreEqual("tinyint", typeMapper.MapToNativeType(DbType.Byte, null));
            Assert.AreEqual("money", typeMapper.MapToNativeType(DbType.Currency, null));
            Assert.AreEqual("datetime", typeMapper.MapToNativeType(DbType.Date, null));
            Assert.AreEqual("datetime", typeMapper.MapToNativeType(DbType.DateTime, null));
            Assert.AreEqual("decimal", typeMapper.MapToNativeType(DbType.Decimal, null));
            Assert.AreEqual("float", typeMapper.MapToNativeType(DbType.Double, null));
            Assert.AreEqual("uniqueidentifier", typeMapper.MapToNativeType(DbType.Guid, null));
            Assert.AreEqual("smallint", typeMapper.MapToNativeType(DbType.Int16, null));
            Assert.AreEqual("int", typeMapper.MapToNativeType(DbType.Int32, null));
            Assert.AreEqual("bigint", typeMapper.MapToNativeType(DbType.Int64, null));
            Assert.AreEqual("sql_variant", typeMapper.MapToNativeType(DbType.Object, null));
            Assert.AreEqual("real", typeMapper.MapToNativeType(DbType.Single, null));
            Assert.AreEqual("nvarchar(123)", typeMapper.MapToNativeType(DbType.String, 123));
            Assert.AreEqual("nchar(456)", typeMapper.MapToNativeType(DbType.StringFixedLength, 456));
            Assert.AreEqual("rowversion", typeMapper.MapToNativeType(DbType.Time, null));
            Assert.AreEqual("xml", typeMapper.MapToNativeType(DbType.Xml, null));
        }

        [Test()]
        [ExpectedException(typeof(DbPlatformException), ExpectedMessage = "Unknown data type: 'DbType.DateTime2'")]
        public void MapDateTime2()
        {
            typeMapper.MapToNativeType(DbType.DateTime2, null);
        }

        [Test()]
        [ExpectedException(typeof(DbPlatformException), ExpectedMessage = "Unknown data type: 'DbType.DateTimeOffset'")]
        public void MapDateTimeOffset()
        {
            typeMapper.MapToNativeType(DbType.DateTimeOffset, null);
        }

        [Test()]
        [ExpectedException(typeof(DbPlatformException), ExpectedMessage = "Unknown data type: 'DbType.SByte'")]
        public void MapSByte()
        {
            typeMapper.MapToNativeType(DbType.SByte, null);
        }

        [Test()]
        [ExpectedException(typeof(DbPlatformException), ExpectedMessage = "Unknown data type: 'DbType.UInt16'")]
        public void MapUInt16()
        {
            typeMapper.MapToNativeType(DbType.UInt16, null);
        }

        [Test()]
        [ExpectedException(typeof(DbPlatformException), ExpectedMessage = "Unknown data type: 'DbType.UInt32'")]
        public void MapUInt32()
        {
            typeMapper.MapToNativeType(DbType.UInt32, null);
        }

        [Test()]
        [ExpectedException(typeof(DbPlatformException), ExpectedMessage = "Unknown data type: 'DbType.UInt64'")]
        public void MapUInt64()
        {
            typeMapper.MapToNativeType(DbType.UInt64, null);
        }

        [Test()]
        [ExpectedException(typeof(DbPlatformException), ExpectedMessage = "Unknown data type: 'DbType.VarNumeric'")]
        public void MapVarNumeric()
        {
            typeMapper.MapToNativeType(DbType.VarNumeric, null);
        }
    }
}
