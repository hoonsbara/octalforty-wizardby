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

namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    [DebuggerDisplay("add constraint {Name}")]
    public class AddConstraintNode : AstNode, IAddConstraintNode
    {
        #region Private Fields
        private readonly IList<string> columns = new List<string>();
        private string table;
        private string name;
        #endregion

        public AddConstraintNode(IAstNode parent, string name) : base(parent)
        {
            this.name = name;
        }

        #region AstNode Members
        /// <summary>
        /// Accepts a given <paramref name="visitor"/>.
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
        #endregion

        #region IAddConstraintNode Members
        /// <summary>
        /// Gets or sets a string which contains the name of the current schema element.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Get or sets a string which contains the name of the table of constraint.
        /// </summary>
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        /// <summary>
        /// Gets a reference to the collection of column names for the constraint.
        /// </summary>
        public IList<string> Columns
        {
            get { return columns; }
        }
        #endregion
    }
}