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
using System.Diagnostics;

namespace octalforty.Wizardby.Core.SemanticModel
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("index {Name} on {Table}")]
    public class IndexDefinition : SchemaElementDefinitionBase, IIndexDefinition
    {
        #region Private Fields
        private readonly IList<IIndexColumnDefinition> columns = new List<IIndexColumnDefinition>();
        private bool? unique;
        private bool? clustered;
        private string table;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexDefinition"/> class.
        /// </summary>
        public IndexDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexDefinition"/> class.
        /// </summary>
        /// <param name="name"></param>
        public IndexDefinition(string name) : 
            base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexDefinition"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        public IndexDefinition(string name, params IIndexColumnDefinition[] columns) : 
            base(name)
        {
            foreach(IIndexColumnDefinition column in columns)
                this.columns.Add(column);
        }

        #region IIndexDefinition Members
        /// <summary>
        /// Get or sets a string which contains the name of the table of index.
        /// </summary>
        public virtual string Table
        {
            get { return table; }
            set { table = value; }
        }

        /// <summary>
        /// Gets a reference to the collection of <see cref="IIndexColumnDefinition"/> objects
        /// which represent the columns within this index.
        /// </summary>
        public virtual IList<IIndexColumnDefinition> Columns
        {
            get { return columns; }
        }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current index or unique or not.
        /// </summary>
        public virtual bool? Unique
        {
            get { return unique; }
            set { unique = value; }
        }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current index or clustered or not.
        /// </summary>
        public virtual bool? Clustered
        {
            get { return clustered; }
            set { clustered = value; }
        }

        public virtual string Where { get; set; }

        #endregion
    }
}