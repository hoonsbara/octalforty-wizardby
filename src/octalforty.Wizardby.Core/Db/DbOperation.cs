namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Represents the method that will perform database operation within the <see cref="IDbExecutive"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public delegate T DbOperation<T>();
}
