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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace octalforty.Wizardby.Core.SemanticModel
{
    public class Schema
    {
        private readonly IDictionary<string, ITableDefinition> tables = new Dictionary<string, ITableDefinition>();
        private readonly IDictionary<string, ISchemaDefinition> schemas = new Dictionary<string, ISchemaDefinition>();

        public ReadOnlyCollection<ITableDefinition> Tables
        {
            get { return new ReadOnlyCollection<ITableDefinition>(new List<ITableDefinition>(tables.Values)); }
        }

        public ReadOnlyCollection<ISchemaDefinition> Schemas
        {
            get { return new ReadOnlyCollection<ISchemaDefinition>(new List<ISchemaDefinition>(schemas.Values)); }
        }

        public void AddSchema(ISchemaDefinition schema)
        {
            schemas[GetInvariantName(schema.Name)] = schema;
        }

        public void AddTable(ITableDefinition table)
        {
            tables[GetInvariantName(table)] = table;
        }

        public ITableDefinition GetTable(string name)
        {
            string invariantName = GetInvariantName(name);
            return InternalGetTable(invariantName);
        }

        public ISchemaDefinition GetSchema(string name)
        {
            string invariantName = GetInvariantName(name);
            return InternalGetSchema(invariantName);
        }

        private ISchemaDefinition InternalGetSchema(string name)
        {
            return GetSchemaElement(schemas, name);
        }

        private ITableDefinition InternalGetTable(string invariantName)
        {
            return GetSchemaElement(tables, invariantName);
        }

        private static T GetSchemaElement<T>(IDictionary<string, T> elements, string invariantName)
            where T : class
        {
            return elements.ContainsKey(invariantName) ?
                elements[invariantName] :
                null;
        }

        public void RemoveTable(string name)
        {
            tables.Remove(GetInvariantName(name));
        }

        private static string GetInvariantName(ITableDefinition table)
        {
            return GetSchemaQualifiedTableName((table.Schema ?? new SchemaDefinition()).Name, table.Name);
        }

        private static string GetInvariantName(string name)
        {
            return name.ToLowerInvariant();
        }

        private static string GetSchemaQualifiedTableName(string schemaName, string tableName)
        {
            return string.IsNullOrEmpty(schemaName) ? 
                GetInvariantName(tableName) : 
                string.Format("{0}.{1}", GetInvariantName(schemaName), GetInvariantName(tableName));
        }

        public ITableDefinition GetTable(string schemaName, string tableName)
        {
            return InternalGetTable(GetSchemaQualifiedTableName(schemaName, tableName));
        }

        public ITableDefinition GetTable(ISchemaDefinition schema, string tableName)
        {
            return GetTable(schema == null ? null : schema.Name, tableName);
        }
    }
}