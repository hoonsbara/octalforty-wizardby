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

using Rhino.Mocks;

namespace octalforty.Wizardby.Tests.Db.SqlServer
{
    [TestFixture()]
    public class SqlServerCommandExecutiveTestFixture
    {
        [Test()]
        [ExpectedException(typeof(DbPlatformException))]
        public void ExecuteReader()
        {
            SqlServerExecutive dbCommandExecutive = new SqlServerExecutive();
            dbCommandExecutive.ExceptionFilter = delegate(Exception exception)
                { return exception is ApplicationException; };

            MockRepository mockRepository = new MockRepository();

            IDbCommand dbCommand = mockRepository.StrictMock<IDbCommand>();
            using(mockRepository.Record())
            {
                Expect.Call(dbCommand.ExecuteReader()).Throw(new ApplicationException());
            }

            mockRepository.ReplayAll();

            dbCommandExecutive.ExecuteReader(dbCommand);

            mockRepository.VerifyAll();
        }

        [Test()]
        [ExpectedException(typeof(DbPlatformException))]
        public void ExecuteNonQuery()
        {
            SqlServerExecutive dbCommandExecutive = new SqlServerExecutive();
            dbCommandExecutive.ExceptionFilter = delegate(Exception exception)
                { return exception is ApplicationException; };

            MockRepository mockRepository = new MockRepository();

            IDbCommand dbCommand = mockRepository.StrictMock<IDbCommand>();
            using(mockRepository.Record())
            {
                Expect.Call(dbCommand.ExecuteNonQuery()).Throw(new ApplicationException());
            }

            mockRepository.ReplayAll();

            dbCommandExecutive.ExecuteNonQuery(dbCommand);

            mockRepository.VerifyAll();
        }
    }
}
