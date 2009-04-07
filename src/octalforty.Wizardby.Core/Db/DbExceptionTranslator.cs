namespace octalforty.Wizardby.Core.Db
{
    public class DbExceptionTranslator : DbPlatformDependencyBase, IDbExceptionTranslator
    {
        public T Execute<T>(DbAction<T> dbAction)
        {
            return dbAction();
        }
    }
}
