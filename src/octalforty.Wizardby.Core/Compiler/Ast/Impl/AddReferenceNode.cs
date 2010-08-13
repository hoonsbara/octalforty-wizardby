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
    [DebuggerDisplay("add reference {Name} ({FkTable} to {PkTable}) ({Location})")]
    internal class AddReferenceNode : AstNode, IAddReferenceNode
    {
        #region Private Fields
        private string name;
        private readonly IList<string> pkColumns = new List<string>();
        private readonly IList<string> fkColumns = new List<string>();
        private string pkTable;
        private string fkTable;
        #endregion

        private string pkTableSchema;

        private string fkTableSchema;

        public AddReferenceNode(IAstNode parent, string name) : 
            base(parent)
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

        #region IAddReferenceNode Members
        public IList<string> PkColumns
        {
            get { return pkColumns; }
        }

        public IList<string> FkColumns
        {
            get { return fkColumns; }
        }

        public string PkTable
        {
            get { return pkTable; }
            set { pkTable = value; }
        }

        public string FkTable
        {
            get { return fkTable; }
            set { fkTable = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string PkTableSchema
        {
            get { return pkTableSchema; }
            set { pkTableSchema = value; }
        }

        public string FkTableSchema
        {
            get { return fkTableSchema; }
            set { fkTableSchema = value; }
        }
        #endregion
    }
}
