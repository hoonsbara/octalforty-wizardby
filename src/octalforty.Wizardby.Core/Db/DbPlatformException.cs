using System;

namespace octalforty.Wizardby.Core.Db
{
    public class DbPlatformException : ApplicationException
    {
        public DbPlatformException(string message) : 
            base(message)
        {
        }

        public DbPlatformException(string message, Exception innerException) : 
            base(message, innerException)
        {
        }
    }
}
