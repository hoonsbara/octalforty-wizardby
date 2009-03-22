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
namespace octalforty.Wizardby.Core.SemanticModel
{
    /// <summary>
    /// Represents a table within a database schema.
    /// </summary>
    public interface ITableDefinition : ISchemaElementDefinition
    {
        /// <summary>
        /// Gets a reference to the collection if <see cref="IColumnDefinition"/> objects
        /// for the current table.
        /// </summary>
        ISchemaElementDefinitionCollection<IColumnDefinition> Columns
        { get; }

        /// <summary>
        /// Gets a reference to the collection if <see cref="IIndexDefinition"/> objects
        /// for the current table.
        /// </summary>
        ISchemaElementDefinitionCollection<IIndexDefinition> Indexes
        { get; }

        /// <summary>
        /// Gets a reference to the collection if <see cref="IReferenceDefinition"/> objects
        /// for the current table.
        /// </summary>
        ISchemaElementDefinitionCollection<IReferenceDefinition> References
        { get; }

        /// <summary>
        /// Returns a column named <paramref name="name"/> or <c>null</c> if no column
        /// with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IColumnDefinition GetColumn(string name);
       
        /// <summary>
        /// Adds <paramref name="column"/> to the list of columns.
        /// </summary>
        /// <param name="column"></param>
        void AddColumn(IColumnDefinition column);

        /// <summary>
        /// Removes column named <paramref name="name"/> from the list of columns.
        /// </summary>
        /// <param name="name"></param>
        void RemoveColumn(string name);

        /// <summary>
        /// Returns an index named <paramref name="name"/> or <c>null</c> if no index
        /// with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IIndexDefinition GetIndex(string name);

        /// <summary>
        /// Adds <paramref name="index"/> to the list of indexes.
        /// </summary>
        /// <param name="index"></param>
        void AddIndex(IIndexDefinition index);

        /// <summary>
        /// Removes index named <paramref name="name"/> from the list of indexes.
        /// </summary>
        /// <param name="name"></param>
        void RemoveIndex(string name);

        /// <summary>
        /// Returns a reference named <paramref name="name"/> or <c>null</c> if no reference
        /// with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IReferenceDefinition GetReference(string name);

        /// <summary>
        /// Adds <paramref name="reference"/> to the list of references.
        /// </summary>
        /// <param name="reference"></param>
        void AddReference(IReferenceDefinition reference);

        /// <summary>
        /// Removes reference named <paramref name="name"/> from the list of references.
        /// </summary>
        /// <param name="name"></param>
        void RemoveReference(string name);

        /// <summary>
        /// Gets a reference to the <see cref="IColumnDefinition"/> which represents the 
        /// primary key column of the current table.
        /// </summary>
        /// <returns></returns>
        IColumnDefinition GetPrimaryKeyColumn();
    }
}
