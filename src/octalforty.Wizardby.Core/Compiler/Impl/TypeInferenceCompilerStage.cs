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
using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    /// <summary>
    /// A MDL Compiler Stage which inferes column types where no types specified.
    /// </summary>
    public class TypeInferenceCompilerStage : MdlCompilerStageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInferenceCompilerStage"/> class.
        /// </summary>
        public TypeInferenceCompilerStage()
        {
        }

        #region MdlCompilerStageBase Members
        /// <summary>
        /// Visits the given <paramref name="addColumnNode"/>.
        /// </summary>
        /// <param name="addColumnNode"></param>
        public override void Visit(IAddColumnNode addColumnNode)
        {
            /*IColumnDef
            IColumnBinding columnBinding = Environment.Resolve<IColumnBinding>(addColumnNode);
            InferColumnType(addColumnNode, columnBinding);*/
        }
        #endregion

        /*private void InferColumnType(IAstNode columnNode, IColumnBinding columnBinding)
        {
            //
            // Type is already resolved, nothing to do here
            if(columnBinding.Type != null)
                return;

            //
            // We can infer column type from child IAddReferenceNode, if there's any
            IAddReferenceNode addReferenceNode = GetFirst<IAddReferenceNode>(columnNode.ChildNodes);
            if(addReferenceNode != null)
            {
                //
                // Reference binding must contain only one primary key column
                IReferenceBinding referenceBinding = Environment.Resolve<IReferenceBinding>(addReferenceNode);
                if(referenceBinding != null && referenceBinding.PrimaryKeyColumns.GetLength(0) == 1)
                {
                    IColumnBinding primaryKeyColumnBinding = referenceBinding.PrimaryKeyColumns[0];
                    columnBinding.Type = primaryKeyColumnBinding.Type;
                } // if
            } // if
        }*/
    }
}
