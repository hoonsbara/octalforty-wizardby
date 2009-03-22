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

using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    public class TemplateSubstitutionCompilerStage : MdlCompilerStageBase
    {
        #region Private Fields
        private readonly IDictionary<string, ITableTemplateNode> tableTemplateNodes = 
            new Dictionary<string, ITableTemplateNode>();
        #endregion

        public TemplateSubstitutionCompilerStage()
        {
        }

        #region MdlCompilerStageBase Members
        /// <summary>
        ///  Visits the given <paramref name="tableTemplateNode" />.
        /// </summary>
        /// <param name="tableTemplateNode"></param>
        public override void Visit(ITableTemplateNode tableTemplateNode)
        {
            tableTemplateNodes[tableTemplateNode.Name] = tableTemplateNode;
        }

        /// <summary>
        /// Visits the given <paramref name="addTableNode"/>.
        /// </summary>
        /// <param name="addTableNode"></param>
        public override void Visit(IAddTableNode addTableNode)
        {
            //
            // If neither "template" nor "templates" properties specified on the node, skip it
            if(!addTableNode.Properties.ContainsProperty(MdlSyntax.Template) &&
                !addTableNode.Properties.ContainsProperty(MdlSyntax.Templates))
                return;

            if(addTableNode.Properties.ContainsProperty(MdlSyntax.Template))
            {
                SubstituteTemplate(addTableNode, addTableNode.Properties[MdlSyntax.Template].Value.ToString());
                return;
            } // if

            object[] templateNames = (object[])addTableNode.Properties[MdlSyntax.Templates].Value;
            foreach(object templateName in templateNames)
                SubstituteTemplate(addTableNode, templateName.ToString());
        }
        #endregion

        private void SubstituteTemplate(IAddTableNode addTableNode, string templateName)
        {
            if(!tableTemplateNodes.ContainsKey(templateName))
                return;

            foreach(IAstNode childNode in tableTemplateNodes[templateName].ChildNodes)
            {
                IAstNode astNode = AstUtil.Clone(childNode);
                
                astNode.Parent = addTableNode;
                addTableNode.ChildNodes.Add(astNode);
            } // foreach
        }
    }
}
