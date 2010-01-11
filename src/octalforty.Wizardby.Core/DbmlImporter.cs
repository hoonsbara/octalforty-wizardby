using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core
{
    public class DbmlImporter
    {
        private readonly XmlNamespaceManager namespaceManager;

        public DbmlImporter()
        {
            namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("dbml", "http://schemas.microsoft.com/linqtosql/dbml/2007");
        }

        public Schema ImportDbml(Stream stream)
        {
            if(stream == null) 
                throw new ArgumentNullException("stream");

            var typeToTableMapping = new Dictionary<string, ITableDefinition>();

            var document = new XmlDocument();
            document.Load(stream);

            var schema = new Schema();

            foreach(XmlNode tableNode in document.SelectNodes("dbml:Database/dbml:Table", namespaceManager))
            {
                var name = tableNode.Attributes["Name"].Value.Split('.');

                ISchemaDefinition schemaDefinition = schema.GetSchema(name[0]);// new SchemaDefinition(name[0]);
                var tableDefinition = new TableDefinition(name[1], schemaDefinition);

                schema.AddSchema(schemaDefinition);
                //schemaDefinition.

                //Console.WriteLine(tableNode.Name);
            } // foreach

            return schema;
        }
    }
}
