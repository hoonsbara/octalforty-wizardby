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
using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Compiler.Ast
{
    public static class AstUtil
    {
        /// <summary>
        /// Builds up an AST with <paramref name="rootNode"/> as a root node 
        /// from the provided <paramref name="schema"/>.
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static IAstNode BuildAstNodeFromSchema(IAstNode rootNode, Schema schema)
        {
            foreach(ITableDefinition table in schema.Tables)
            {
                IAddTableNode addTableNode = new AddTableNode(rootNode, table.Name);
                rootNode.ChildNodes.Add(addTableNode);

                foreach(IColumnDefinition column in table.Columns)
                {
                    IAddColumnNode addColumnNode = new AddColumnNode(addTableNode, column.Name);
                    addTableNode.ChildNodes.Add(addColumnNode);

                    SemanticModelUtil.Copy(column, addColumnNode);
                } // foreach

                foreach(IIndexDefinition index in table.Indexes)
                {
                    IAddIndexNode addIndexNode = new AddIndexNode(rootNode, index.Name);
                    rootNode.ChildNodes.Add(addIndexNode);

                    SemanticModelUtil.Copy(index, addIndexNode);
                } // foreach

                foreach(IReferenceDefinition reference in table.References)
                {
                    IAddReferenceNode addReferenceNode = new AddReferenceNode(rootNode, reference.Name);
                    rootNode.ChildNodes.Add(addReferenceNode);

                    SemanticModelUtil.Copy(reference, addReferenceNode);
                } // foreach
            } // foreach

            return rootNode;
        }

        /// <summary>
        /// Clones the given <paramref name="addColumnNode"/>.
        /// </summary>
        /// <param name="addColumnNode"></param>
        /// <returns></returns>
        public static IAddColumnNode Clone(IAddColumnNode addColumnNode)
        {
            IAddColumnNode node = new AddColumnNode(addColumnNode.Parent, addColumnNode.Name);
            SemanticModelUtil.Copy(addColumnNode, node);

            foreach(IAstNodeProperty property in addColumnNode.Properties)
                node.Properties.AddProperty(new AstNodeProperty(property.Name, property.Value));

            return node;
        }

        public static IAstNode Clone(IAstNode astNode)
        {
            if(astNode is IAddColumnNode)
                return Clone((IAddColumnNode)astNode);

            return null;
        }
        
        public static void CopyToProperties(IColumnNode columnNode)
        {
            if(!string.IsNullOrEmpty(columnNode.Default))
                AddProperty(columnNode, MdlSyntax.Default, columnNode.Default);

            if(columnNode.Type != null)
                AddProperty(columnNode, MdlSyntax.Type, columnNode.Type.Value.ToString());

            if(columnNode.PrimaryKey.GetValueOrDefault(false))
                AddProperty(columnNode, MdlSyntax.PrimaryKey, "true");

            if(columnNode.Nullable.HasValue)
                AddProperty(columnNode, MdlSyntax.Nullable, columnNode.Nullable.Value.ToString().ToLower());

            if(columnNode.Length.HasValue)
                AddProperty(columnNode, MdlSyntax.Length, columnNode.Length.Value);

            if(columnNode.Identity.GetValueOrDefault(false))
                AddProperty(columnNode, MdlSyntax.Identity, "true");
        }

        private static void AddProperty(IAstNode node, string name, object value)
        {
            node.Properties.AddProperty(new AstNodeProperty(name, value));
        }
    }
}
