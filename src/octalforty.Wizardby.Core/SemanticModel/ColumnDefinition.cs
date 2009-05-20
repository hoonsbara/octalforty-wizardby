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
using System.Diagnostics;

namespace octalforty.Wizardby.Core.SemanticModel
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{Name} {Type}({Length}), Identity = {Identity}, Primary Key = {PrimaryKey}")]
    public class ColumnDefinition : SchemaElementDefinitionBase, IColumnDefinition
    {
        #region Private Fields
        private DbType? type;
        private bool? nullable;
        private int? length;
        private int? scale;
        private int? precision;
        private bool? primaryKey;
        private bool? identity;
        private string table;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        public ColumnDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        /// <param name="columnDefinition"></param>
        public ColumnDefinition(IColumnDefinition columnDefinition) :
            this(columnDefinition.Name, columnDefinition.Table, columnDefinition.Type, columnDefinition.Nullable, columnDefinition.Length, 
                columnDefinition.Scale, columnDefinition.Precision, columnDefinition.PrimaryKey, columnDefinition.Identity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        /// <param name="name"></param>
        public ColumnDefinition(string name) : 
            base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="nullable"></param>
        /// <param name="length"></param>
        /// <param name="scale"></param>
        /// <param name="precision"></param>
        public ColumnDefinition(string name, DbType? type, bool? nullable, int? length, int? scale, int? precision) : 
            this(name)
        {
            this.type = type;
            this.nullable = nullable;
            this.length = length;
            this.scale = scale;
            this.precision = precision;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="table"></param>
        /// <param name="type"></param>
        /// <param name="nullable"></param>
        /// <param name="length"></param>
        /// <param name="scale"></param>
        /// <param name="precision"></param>
        /// <param name="primaryKey"></param>
        /// <param name="identity"></param>
        public ColumnDefinition(string name, string table, DbType? type, bool? nullable, int? length, int? scale, int? precision, 
            bool? primaryKey, bool? identity) : 
            this(name, type, nullable, length, scale, precision)
        {
            this.table = table;
            this.primaryKey = primaryKey;
            this.identity = identity;
        }

        #region IColumnDefinition Members
        /// <summary>
        /// Gets or sets a string which contains the name of the table this
        /// column belongs to.
        /// </summary>
        public virtual string Table
        {
            get { return table; }
            set { table = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="DbType"/> member which represents the type of the current column.
        /// </summary>
        public virtual DbType? Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current column is identity column.
        /// </summary>
        public virtual bool? Identity
        {
            get { return identity; }
            set { identity = value; }
        }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current column is nullable or not.
        /// </summary>
        public virtual bool? Nullable
        {
            get { return nullable; }
            set { nullable = value; }
        }

        /// <summary>
        /// Gets or sets a value which contains the length of the current column.
        /// </summary>
        public virtual int? Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// Gets or sets a value which contains the scale of the current column.
        /// </summary>
        public virtual int? Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets a value which contains the precision of the current column.
        /// </summary>
        public virtual int? Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        /// <summary>
        /// Gets or sets a flag which indicates whether the current column is a primary key column.
        /// </summary>
        public virtual bool? PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }
        #endregion
    }
}