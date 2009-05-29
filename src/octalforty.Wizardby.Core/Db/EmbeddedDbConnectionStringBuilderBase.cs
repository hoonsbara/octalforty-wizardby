using System.IO;

namespace octalforty.Wizardby.Core.Db
{
    public abstract class EmbeddedDbConnectionStringBuilderBase : DbConnectionStringBuilderBase
    {
        private readonly string databaseFileExtension;
        private readonly bool extensionRequired;
        private readonly bool allowArbitraryExtension;

        protected EmbeddedDbConnectionStringBuilderBase(string databaseFileExtension, bool extensionRequired, bool allowArbitraryExtension)
        {
            this.databaseFileExtension = databaseFileExtension;
            this.allowArbitraryExtension = allowArbitraryExtension;
            this.extensionRequired = extensionRequired;
        }

        protected EmbeddedDbConnectionStringBuilderBase(bool ignoreUnmappedKeys, string databaseFileExtension, 
            bool extensionRequired, bool allowArbitraryExtension) : 
            base(ignoreUnmappedKeys)
        {
            this.databaseFileExtension = databaseFileExtension;
            this.allowArbitraryExtension = allowArbitraryExtension;
            this.extensionRequired = extensionRequired;
        }

        public override void AppendKeyValuePair(string key, string value)
        {
            //
            // If we have a "database" key, append extension to "value" 
            // if it's not already there.
            if(key.ToLowerInvariant() == "database")
            {
                string extension = Path.GetExtension(value);
                if((extensionRequired && string.IsNullOrEmpty(extension)) || !allowArbitraryExtension)
                {
                    base.AppendKeyValuePair(key, value + databaseFileExtension);
                    return;
                } // if
            } // if

            base.AppendKeyValuePair(key, value);
        }
    }
}
