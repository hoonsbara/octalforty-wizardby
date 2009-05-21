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
    public class NamingCompilerStage : MdlCompilerStageBase
    {
        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            if(string.IsNullOrEmpty(addIndexNode.Name))
                addIndexNode.Name = Environment.GetAnonymousIdentifier();

            base.Visit(addIndexNode);
        }

        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            if(string.IsNullOrEmpty(addReferenceNode.Name))
                addReferenceNode.Name = Environment.GetAnonymousIdentifier();

            base.Visit(addReferenceNode);
        }

        /// <summary>
        /// Visits the given <paramref name="addConstraintNode"/>.
        /// </summary>
        /// <param name="addConstraintNode"></param>
        public override void Visit(IAddConstraintNode addConstraintNode)
        {
            if(string.IsNullOrEmpty(addConstraintNode.Name))
                addConstraintNode.Name = Environment.GetAnonymousIdentifier();

            base.Visit(addConstraintNode);
        }
    }
}
