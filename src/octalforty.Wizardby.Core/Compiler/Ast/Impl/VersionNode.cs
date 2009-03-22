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
using System.Diagnostics;

namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    [DebuggerDisplay("version {Number}")]
    internal class VersionNode : AstNode, IVersionNode
    {
        #region Private Fields
        private readonly long number;
        #endregion

        public VersionNode(IAstNode parent, long number) : 
            base(parent)
        {
            this.number = number;
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

        /// <summary>
        /// Gets the number of the current version.
        /// </summary>
        public long Number
        {
            get { return number; }
        }
    }
}
