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
