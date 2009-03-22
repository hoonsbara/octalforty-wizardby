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
using System.Data;

namespace octalforty.Wizardby.Core.SemanticModel
{
    /// <summary>
    /// Represents a column within a table.
    /// </summary>
    public interface IColumnDefinition : ISchemaElementDefinition
    {
        /// <summary>
        /// Gets or sets a string which contains the name of the table this
        /// column belongs to.
        /// </summary>
        string Table
        { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DbType"/> member which represents the type of the current column.
        /// </summary>
        DbType? Type 
        { get; set; }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current column is a primary key column.
        /// </summary>
        bool? PrimaryKey 
        { get; set; }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current column is identity column.
        /// </summary>
        bool? Identity 
        { get; set; }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current column is nullable or not.
        /// </summary>
        bool? Nullable 
        { get; set; }

        /// <summary>
        /// Gets or sets a value which contains the length of the current column.
        /// </summary>
        int? Length 
        { get; set; }

        /// <summary>
        /// Gets or sets a value which contains the scale of the current column.
        /// </summary>
        int? Scale 
        { get; set; }

        /// <summary>
        /// Gets or sets a value which contains the precision of the current column.
        /// </summary>
        int? Precision 
        { get; set; }

        /// <summary>
        /// Gets or sets a default value for the current column.
        /// </summary>
        string Default
        { get; set; }
    }
}