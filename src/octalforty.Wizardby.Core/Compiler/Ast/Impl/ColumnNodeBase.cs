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

namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    internal abstract class ColumnNodeBase : AstNode, IColumnNode
    {
        private DbType? type;
        private bool? nullable;
        private int? length;
        private int? scale;
        private int? precision;
        private bool? primaryKey;
        private string name;
        private bool? identity;
        private string table;
        private string @default;

        protected ColumnNodeBase(IAstNode parent) : 
            base(parent)
        {
        }

        protected ColumnNodeBase(IAstNode parent, string name) : 
            this(parent)
        {
            this.name = name;
        }

        #region IColumnDefinition Members
        /// <summary>
        /// Gets or sets a string which contains the name of the table this
        /// column belongs to.
        /// </summary>
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        public DbType? Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool? Identity
        {
            get { return identity; }
            set { identity = value; }
        }

        public bool? Nullable
        {
            get { return nullable; }
            set { nullable = value; }
        }

        public int? Length
        {
            get { return length; }
            set { length = value; }
        }

        public int? Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public int? Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public bool? PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets a default value for the current column.
        /// </summary>
        public string Default
        {
            get { return @default; }
            set { @default = value; }
        }
        #endregion
    }
}