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

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    public class AstFlattenerCompilerStage : MdlCompilerStageBase
    {
        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            if(IsImmediateChildOf<IColumnNode>(addReferenceNode))
                MoveNodeTo(addReferenceNode, addReferenceNode.Parent.Parent.Parent);
            else if(IsImmediateChildOf<IAddTableNode>(addReferenceNode) || IsImmediateChildOf<IAlterTableNode>(addReferenceNode))
                MoveNodeTo(addReferenceNode, addReferenceNode.Parent.Parent);
        }

        /// <summary>
        /// Visits the given <paramref name="removeReferenceNode"/>.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
            if(IsImmediateChildOf<IColumnNode>(removeReferenceNode))
                MoveNodeTo(removeReferenceNode, removeReferenceNode.Parent.Parent.Parent);
            else if(IsImmediateChildOf<ITableNode>(removeReferenceNode))
                MoveNodeTo(removeReferenceNode, removeReferenceNode.Parent.Parent);
        }

        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            if(IsImmediateChildOf<IColumnNode>(addIndexNode))
                MoveNodeTo(addIndexNode, addIndexNode.Parent.Parent.Parent);
            else if(IsImmediateChildOf<IAddTableNode>(addIndexNode) || IsImmediateChildOf<IAlterTableNode>(addIndexNode))
                MoveNodeTo(addIndexNode, addIndexNode.Parent.Parent);
        }

        /// <summary>
        /// Visits the given <paramref name="removeIndexNode"/>.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            if(IsImmediateChildOf<IAlterColumnNode>(removeIndexNode))
                MoveNodeTo(removeIndexNode, removeIndexNode.Parent.Parent.Parent);
            else if(IsImmediateChildOf<IAlterTableNode>(removeIndexNode))
                MoveNodeTo(removeIndexNode, removeIndexNode.Parent.Parent);
        }

        /// <summary>
        /// Visits the given <paramref name="addConstraintNode"/>.
        /// </summary>
        /// <param name="addConstraintNode"></param>
        public override void Visit(IAddConstraintNode addConstraintNode)
        {
            if(IsImmediateChildOf<IColumnNode>(addConstraintNode))
                MoveNodeTo(addConstraintNode, addConstraintNode.Parent.Parent.Parent);
            else if(IsImmediateChildOf<IAddTableNode>(addConstraintNode) || IsImmediateChildOf<IAlterTableNode>(addConstraintNode))
                MoveNodeTo(addConstraintNode, addConstraintNode.Parent.Parent);
        }

        /// <summary>
        /// Visits the given <paramref name="removeConstraintNode"/>.
        /// </summary>
        /// <param name="removeConstraintNode"></param>
        public override void Visit(IRemoveConstraintNode removeConstraintNode)
        {
            if(IsImmediateChildOf<IAlterColumnNode>(removeConstraintNode))
                MoveNodeTo(removeConstraintNode, removeConstraintNode.Parent.Parent.Parent);
            else if(IsImmediateChildOf<IAlterTableNode>(removeConstraintNode.Parent))
                MoveNodeTo(removeConstraintNode, removeConstraintNode.Parent.Parent);
        }

        private static void MoveNodeTo(IAstNode astNode, IAstNode newParentNode)
        {
            astNode.Parent.ChildNodes.Remove(astNode);
            
            newParentNode.ChildNodes.Add(astNode);
            astNode.Parent = newParentNode;
        }
    }
}
