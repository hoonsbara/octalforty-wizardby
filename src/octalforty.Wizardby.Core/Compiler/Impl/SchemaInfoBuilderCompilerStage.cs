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
using System.Data;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    public class SchemaInfoBuilderCompilerStage : MdlCompilerStageBase
    {
        private bool builtSchemaInfo;

        /// <summary>
        /// Visits the given <paramref name="versionNode"/>.
        /// </summary>
        /// <param name="versionNode"></param>
        public override void Visit(IVersionNode versionNode)
        {
            //
            // Add a IAddTableNode which will create "SchemaInfo" tabled 
            // to the first IVersionNode we encounter 
            if(builtSchemaInfo)
                return;

            IAddTableNode addSchemaInfoTableNode = new AddTableNode(versionNode, "SchemaInfo");
            
            IAddColumnNode addVersionColumnNode = new AddColumnNode(addSchemaInfoTableNode, "Version");
            addVersionColumnNode.Properties.AddProperty(AstNodeProperty.Symbol(MdlSyntax.Type, DbType.Int64.ToString()));
            addVersionColumnNode.Properties.AddProperty(AstNodeProperty.Symbol(MdlSyntax.Unique, "true"));
            addVersionColumnNode.Properties.AddProperty(AstNodeProperty.Symbol(MdlSyntax.Nullable, "false"));

            addSchemaInfoTableNode.ChildNodes.Add(addVersionColumnNode);

            builtSchemaInfo = true;

            versionNode.ChildNodes.Insert(0, addSchemaInfoTableNode);
        }
    }
}
