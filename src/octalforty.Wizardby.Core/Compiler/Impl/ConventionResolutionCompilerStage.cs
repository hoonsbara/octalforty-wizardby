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
using System;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    /// <summary>
    /// Performs resolution of conventions in an AST.
    /// </summary>
    /// <remarks>
    /// This stage resolves all conventions and expands the AST appropriately. For instance, this piece:
    /// <code>
    /// add table Bar ... :
    ///     add column Foo ... :
    ///         index
    /// </code>
    /// will get expanded to
    /// <code>
    /// add table Bar ... :
    ///     add column Foo ... :
    ///         index table => Bar, column => Foo
    /// </code>
    /// </remarks>
    public class ConventionResolutionCompilerStage : MdlCompilerStageBase
    {
        #region MdlCompilerStageBase Members
        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            if(addIndexNode.Properties[MdlSyntax.Column] == null && addIndexNode.Properties[MdlSyntax.Columns] == null)
                ResolveAddIndexColumn(addIndexNode);

            if(addIndexNode.Properties[MdlSyntax.Table] == null)
                ResolveAddIndexTable(addIndexNode);
        }
        #endregion

        private static void ResolveAddIndexTable(IAddIndexNode addIndexNode)
        {
            ITableNode tableNode = null;
            if(addIndexNode.Parent is IAddColumnNode)
                tableNode = (ITableNode)addIndexNode.Parent.Parent;
            else if(addIndexNode.Parent is ITableNode)
                tableNode = (ITableNode)addIndexNode.Parent;
            else
                throw new MdlParserException();

            addIndexNode.Properties.AddProperty(AstNodePropertyUtil.AsString(MdlSyntax.Table, tableNode.Name));

            
        }

        private static void ResolveAddIndexColumn(IAddIndexNode addIndexNode)
        {
            if(!(addIndexNode.Parent is IAddColumnNode))
                throw new MdlParserException();

            IAddColumnNode addColumnNode = (IAddColumnNode)addIndexNode.Parent;
            addIndexNode.Properties.AddProperty(AstNodePropertyUtil.AsString(MdlSyntax.Column, addColumnNode.Name));
        }

    }
}
