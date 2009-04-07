namespace octalforty.Wizardby.Core.Db
{
    public interface IDbExceptionTranslator : IDbPlatformDependency
    {
        T Execute<T>(DbAction<T> dbAction);
    }
}
