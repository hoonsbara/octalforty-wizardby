using System;

namespace octalforty.Wizardby.Core.Db
{
    public class DbPlatformExceptionTranslator : DbPlatformDependencyBase, IDbExceptionTranslator
    {
        public T Execute<T>(DbAction<T> dbAction)
        {
            try
            {

            }
            catch(Exception)
            {
                
                throw;
            }
        }
    }
}
