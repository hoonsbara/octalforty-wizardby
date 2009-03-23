using System.Data;
using System.IO;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Console
{
    public class FileDbCommandExecutive : IDbCommandExecutive
    {
        private IDbPlatform platform;
        private readonly IDbCommandExecutive innerDbCommandExecutive;
        private readonly TextWriter textWriter;

        public FileDbCommandExecutive(TextWriter textWriter, IDbCommandExecutive innerDbCommandExecutive)
        {
            this.textWriter = textWriter;
            this.innerDbCommandExecutive = innerDbCommandExecutive;
        }

        public IDbPlatform Platform
        {
            get { return platform; }
            set { platform = value; }
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
