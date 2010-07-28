#region The MIT License
// The MIT License
// 
// Copyright (c) 2009 octalforty studios
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
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
