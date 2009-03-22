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
using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    public class ShortcutResolutionCompilerStage : MdlCompilerStageBase
    {
        /// <summary>
        ///  Visits the given <paramref name="templatesNode" />.
        /// </summary>
        /// <param name="templatesNode"></param>
        public override void Visit(ITemplatesNode templatesNode)
        {
            //
            // Skip templates
        }

        /// <summary>
        /// Visits the given <paramref name="addColumnNode"/>.
        /// </summary>
        /// <param name="addColumnNode"></param>
        public override void Visit(IAddColumnNode addColumnNode)
        {
            //
            // Create an "IAddReferenceNode" for "references" property
            if(addColumnNode.Properties.ContainsProperty(MdlSyntax.References))
            {
                IAddReferenceNode addReferenceNode = new AddReferenceNode(addColumnNode, "");
                string references = addColumnNode.Properties[MdlSyntax.References].Value.ToString();

                addReferenceNode.Properties.AddProperty(new AstNodeProperty(MdlSyntax.PkTable, 
                    references));

                addColumnNode.ChildNodes.Add(addReferenceNode);
            } // if

            //
            // Create an 'IAddIndexNode" for "unique" property
            if(addColumnNode.Properties.ContainsProperty(MdlSyntax.Unique))
            {
                IAddIndexNode addIndexNode = new AddIndexNode(addColumnNode, "");
                addIndexNode.Properties.AddProperty(new AstNodeProperty(MdlSyntax.Unique, "true"));

                addColumnNode.ChildNodes.Add(addIndexNode);
            } // if
        }
    }
}
