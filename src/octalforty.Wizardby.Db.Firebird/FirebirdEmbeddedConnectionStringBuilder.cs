using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Db.Firebird
{
    public class FirebirdEmbeddedConnectionStringBuilder : DbConnectionStringBuilderBase
    {
        /// <summary>
        /// Appends a <paramref name="key"/>-<paramref name="value"/> pair to the connection string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void AppendKeyValuePair(string key, string value)
        {
            if(key.ToLowerInvariant() != "database")
                return;

            base.AppendKeyValuePair(key, value);
        }

        /// <summary>
        /// Returns the connection string associated with this <see cref="IDbConnectionStringBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString() + "ServerType=1;";
        }
    }
}
