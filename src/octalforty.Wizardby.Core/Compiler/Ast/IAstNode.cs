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

namespace octalforty.Wizardby.Core.Compiler.Ast
{
    /// <summary>
    /// Represents a single node of an Abstract Syntax Tree.
    /// </summary>
    public interface IAstNode
    {
        /// <summary>
        /// Gets or sets a reference to the <see cref="Compiler.Location"/> of this node
        /// in a source file.
        /// </summary>
        Location Location
        { get; set; }

        /// <summary>
        /// Gets or sets a reference to the parent <see cref="IAstNode"/> of this node or <c>null</c> this
        /// is a top-level node or no parent exist.
        /// </summary>
        IAstNode Parent
        { get; set; }

        /// <summary>
        /// Gets a reference to the collection of <see cref="IAstNodeProperty"/> objects, which
        /// represent properties of the current AST node.
        /// </summary>
        IAstNodePropertyCollection Properties
        { get; }

        IList<IAstNode> ChildNodes
        { get; }

        /// <summary>
        /// Accepts a given <paramref name="visitor"/>.
        /// </summary>
        /// <param name="visitor"></param>
        void Accept(IAstVisitor visitor);
    }
}
