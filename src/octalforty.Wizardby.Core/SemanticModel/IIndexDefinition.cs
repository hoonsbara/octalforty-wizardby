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

namespace octalforty.Wizardby.Core.SemanticModel
{
    /// <summary>
    /// Represents an index.
    /// </summary>
    public interface IIndexDefinition : ISchemaElementDefinition
    {
        /// <summary>
        /// Gets a reference to the collection of <see cref="IIndexColumnDefinition"/> objects
        /// which represent the columns within this index.
        /// </summary>
        IList<IIndexColumnDefinition> Columns
        { get; }

        /// <summary>
        /// Get or sets a string which contains the name of the table of index.
        /// </summary>
        string Table
        { get; set; }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current index or unique or not.
        /// </summary>
        bool? Unique
        { get; set; }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current index or clustered or not.
        /// </summary>
        bool? Clustered
        { get; set; }

        string Where { get; set; }
    }
}
