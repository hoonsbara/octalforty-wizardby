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
using System.Diagnostics;

namespace octalforty.Wizardby.Core.SemanticModel
{
    /// <summary>
    /// Standard <see cref="ITableDefinition"/> implementation.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class TableDefinition : SchemaElementDefinitionBase, ITableDefinition
    {
        #region Private Fields
        private readonly SchemaElementCollection<IColumnDefinition> columns = new SchemaElementCollection<IColumnDefinition>();
        private readonly SchemaElementCollection<IIndexDefinition> indexes = new SchemaElementCollection<IIndexDefinition>();
        private readonly SchemaElementCollection<IReferenceDefinition> references = new SchemaElementCollection<IReferenceDefinition>();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDefinition"/> class.
        /// </summary>
        public TableDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDefinition"/> class.
        /// </summary>
        /// <param name="name"></param>
        public TableDefinition(string name) : 
            base(name)
        {
        }

        #region ITableDefinition Members
        /// <summary>
        /// Gets a reference to the collection if <see cref="IColumnDefinition"/> objects
        /// for the current table.
        /// </summary>
        public ISchemaElementDefinitionCollection<IColumnDefinition> Columns
        {
            get { return columns; }
        }

        /// <summary>
        /// Gets a reference to the collection if <see cref="IIndexDefinition"/> objects
        /// for the current table.
        /// </summary>
        public ISchemaElementDefinitionCollection<IIndexDefinition> Indexes
        {
            get { return indexes; }
        }

        /// <summary>
        /// Gets a reference to the collection if <see cref="IReferenceDefinition"/> objects
        /// for the current table.
        /// </summary>
        public ISchemaElementDefinitionCollection<IReferenceDefinition> References
        {
            get { return references; }
        }

        /// <summary>
        /// Returns a column named <paramref name="name"/> or <c>null</c> if no column
        /// with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IColumnDefinition GetColumn(string name)
        {
            return GetSchemaElementDefinition(columns, name);
        }

        /// <summary>
        /// Adds <paramref name="column"/> to the list of columns.
        /// </summary>
        /// <param name="column"></param>
        public void AddColumn(IColumnDefinition column)
        {
            column.Table = Name;
            columns.Add(column);
        }

        /// <summary>
        /// Removes column named <paramref name="name"/> from the list of columns.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveColumn(string name)
        {
            RemoveSchemaElementDefinition(columns, name);
        }

        /// <summary>
        /// Returns an index named <paramref name="name"/> or <c>null</c> if no index
        /// with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IIndexDefinition GetIndex(string name)
        {
            return GetSchemaElementDefinition(indexes, name);
        }

        /// <summary>
        /// Adds <paramref name="index"/> to the list of indexes.
        /// </summary>
        /// <param name="index"></param>
        public void AddIndex(IIndexDefinition index)
        {
            index.Table = Name;
            indexes.Add(index);
        }

        /// <summary>
        /// Removes index named <paramref name="name"/> from the list of indexes.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveIndex(string name)
        {
            RemoveSchemaElementDefinition(indexes, name);
        }

        /// <summary>
        /// Returns a reference named <paramref name="name"/> or <c>null</c> if no reference
        /// with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IReferenceDefinition GetReference(string name)
        {
            return GetSchemaElementDefinition(references, name);
        }

        /// <summary>
        /// Adds <paramref name="reference"/> to the list of references.
        /// </summary>
        /// <param name="reference"></param>
        public void AddReference(IReferenceDefinition reference)
        {
            reference.FkTable = Name;
            references.Add(reference);
        }

        /// <summary>
        /// Removes reference named <paramref name="name"/> from the list of references.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveReference(string name)
        {
            RemoveSchemaElementDefinition(references, name);
        }

        /// <summary>
        /// Gets a reference to the <see cref="IColumnDefinition"/> which represents the 
        /// primary key column of the current table.
        /// </summary>
        /// <returns></returns>
        public IColumnDefinition GetPrimaryKeyColumn()
        {
            foreach(IColumnDefinition column in columns)
                if(column.PrimaryKey ?? false)
                    return column;

            return null;
        }
        #endregion

        private static T GetSchemaElementDefinition<T>(IEnumerable<T> schemaElements, string name)
            where T : ISchemaElementDefinition
        {
            foreach(T schemaElement in schemaElements)
                if(string.Equals(schemaElement.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    return schemaElement;

            return default(T);
        }

        private static void RemoveSchemaElementDefinition<T>(ICollection<T> schemaElements, string name)
            where T : ISchemaElementDefinition
        {
            schemaElements.Remove(GetSchemaElementDefinition(schemaElements, name));
        }
    }
}