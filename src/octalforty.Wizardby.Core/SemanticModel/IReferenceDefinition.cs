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
    /// Represents a foreign key reference.
    /// </summary>
    public interface IReferenceDefinition : ISchemaElementDefinition
    {
        /// <summary>
        /// Gets a reference to the collection of strings which represent names of
        /// primary key columns.
        /// </summary>
        IList<string> PkColumns 
        { get; }

        /// <summary>
        /// Gets a reference to the collection of strings which represent names of
        /// foreign key columns.
        /// </summary>
        IList<string> FkColumns 
        { get; }

        /// <summary>
        /// Gets or sets a string which contains the name of the primary key table.
        /// </summary>
        string PkTable 
        { get; set; }

        /// <summary>
        /// Gets or sets a string which contains the name of the primary key table schema.
        /// </summary>
        string PkTableSchema
        { get; set; }

        /// <summary>
        /// Gets or sets a string which contains the name of the foreign key table.
        /// </summary>
        string FkTable 
        { get; set; }

        /// <summary>
        /// Gets or sets a string which contains the name of the foreign key table schema.
        /// </summary>
        string FkTableSchema
        { get; set; }
    }
}