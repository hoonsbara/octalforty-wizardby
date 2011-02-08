using System;
using System.Data.Common;
using System.Data.SqlClient;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.SqlServer2000
{
    public class SqlServer2000ExceptionTranslator : IDbExceptionTranslator
    {
        public virtual IDbPlatform Platform 
        { get; set; }

        public SqlServer2000ExceptionTranslator(IDbPlatform platform)
        {
            Platform = platform;
        }

        public virtual T Execute<T>(DbAction<T> dbAction)
        {
            try
            {
                return dbAction();
            } // try
            catch(DbException e)
            {
                throw new DbPlatformException(string.Format("An error occured at at line {0}:{1}{2}", 
                    e is SqlException ? ((SqlException)e).LineNumber.ToString() : "(unknown)",
                    e.Message,
                    Environment.NewLine), e);
            } // catch
        }
    }
}