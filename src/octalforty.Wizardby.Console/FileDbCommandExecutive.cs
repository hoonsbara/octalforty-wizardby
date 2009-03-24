using System.Data;
using System.IO;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Console
{
    public class FileDbCommandExecutive : IDbExecutive
    {
        private IDbPlatform platform;
        private readonly IDbExecutive innerDbCommandExecutive;
        private readonly TextWriter textWriter;

        public FileDbCommandExecutive(TextWriter textWriter, IDbExecutive innerDbCommandExecutive)
        {
            this.textWriter = textWriter;
            this.innerDbCommandExecutive = innerDbCommandExecutive;
        }

        public IDbPlatform Platform
        {
            get { return platform; }
            set { platform = value; }
        }

        /// <summary>
        /// Executes the given <paramref name="dbOperation"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbOperation"></param>
        /// <returns></returns>
        public T Execute<T>(DbOperation<T> dbOperation)
        {
            //TODO: Is this really no-op?
            return default(T);
        }

        public IDataReader ExecuteReader(IDbCommand dbCommand)
        {
            return innerDbCommandExecutive.ExecuteReader(dbCommand);
        }

        public int ExecuteNonQuery(IDbCommand dbCommand)
        {
            textWriter.WriteLine(dbCommand.CommandText);
            return 0;
        }
    }
}
