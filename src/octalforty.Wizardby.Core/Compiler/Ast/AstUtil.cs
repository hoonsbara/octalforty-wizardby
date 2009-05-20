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
using System.Collections.Generic;

using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.SemanticModel;
using octalforty.Wizardby.Core.Util;

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
                    CopyProperties(column, addColumnNode);
                } // foreach

                foreach(IIndexDefinition index in table.Indexes)
                {
                    IAddIndexNode addIndexNode = new AddIndexNode(rootNode, index.Name);
                    rootNode.ChildNodes.Add(addIndexNode);

                    SemanticModelUtil.Copy(index, addIndexNode);
                    CopyProperties(index, addIndexNode);
                } // foreach

                foreach(IReferenceDefinition reference in table.References)
                {
                    IAddReferenceNode addReferenceNode = new AddReferenceNode(rootNode, reference.Name);
                    rootNode.ChildNodes.Add(addReferenceNode);

                    SemanticModelUtil.Copy(reference, addReferenceNode);
                    CopyProperties(reference, addReferenceNode);
                } // foreach
            } // foreach

            return rootNode;
        }

        /// <summary>
        /// Copies all properties from <paramref name="columnDefinition"/> to appropriate <see cref="IAstNodeProperty"/>
        /// objects and adds them to <paramref name="columnNode"/>.
        /// </summary>
        /// <param name="columnDefinition"></param>
        /// <param name="columnNode"></param>
        public static void CopyProperties(IColumnDefinition columnDefinition, IColumnNode columnNode)
        {
            if(columnDefinition.Type != null)
                AddProperty(columnNode, MdlSyntax.Type, columnDefinition.Type.Value.ToString());

            if(columnDefinition.Nullable.HasValue)
                AddProperty(columnNode, MdlSyntax.Nullable, columnDefinition.Nullable.Value.ToString().ToLower());

            if(columnDefinition.Length.HasValue)
                AddProperty(columnNode, MdlSyntax.Length, columnDefinition.Length.Value);

            if(columnDefinition.Precision.HasValue)
                AddProperty(columnNode, MdlSyntax.Precision, columnDefinition.Precision.Value);

            if(columnDefinition.Scale.HasValue)
                AddProperty(columnNode, MdlSyntax.Scale, columnDefinition.Scale.Value);

            if(columnDefinition.PrimaryKey.GetValueOrDefault(false))
                AddProperty(columnNode, MdlSyntax.PrimaryKey, "true");

            if(columnDefinition.Identity.GetValueOrDefault(false))
                AddProperty(columnNode, MdlSyntax.Identity, "true");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="addReferenceNode"></param>
        private static void CopyProperties(IReferenceDefinition reference, IAddReferenceNode addReferenceNode)
        {
            if(!string.IsNullOrEmpty(reference.PkTable))
                AddProperty(addReferenceNode, MdlSyntax.PkTable, reference.PkTable);

            if(reference.PkColumns.Count == 1)
                AddProperty(addReferenceNode, MdlSyntax.PkColumn, reference.PkColumns[0]);
            else
                AddListProperty(addReferenceNode, MdlSyntax.PkColumns,
                    Algorithms.Convert<string, IAstNodePropertyValue>(reference.PkColumns, 
                        delegate(string input) { return new StringAstNodePropertyValue(input); }));

            if(!string.IsNullOrEmpty(reference.FkTable))
                AddProperty(addReferenceNode, MdlSyntax.FkTable, reference.FkTable);

            if(reference.FkColumns.Count == 1)
                AddProperty(addReferenceNode, MdlSyntax.FkColumn, reference.FkColumns[0]);
            else
                AddListProperty(addReferenceNode, MdlSyntax.FkColumns,
                    Algorithms.Convert<string, IAstNodePropertyValue>(reference.FkColumns,
                        delegate(string input) { return new StringAstNodePropertyValue(input); }));
        }

        private static void CopyProperties(IIndexDefinition index, IAddIndexNode addIndexNode)
        {
            if(!string.IsNullOrEmpty(index.Table))
                AddProperty(addIndexNode, MdlSyntax.Table, index.Table);

            if(index.Clustered.GetValueOrDefault(false))
                AddProperty(addIndexNode, MdlSyntax.Clustered, "true");

            if(index.Unique.GetValueOrDefault(false))
                AddProperty(addIndexNode, MdlSyntax.Unique, "true");

            if(index.Columns.Count == 1)
            {
                IAstNodePropertyValue value = GetIndexColumnPropertyValue(index.Columns[0]);
                AddProperty(addIndexNode, MdlSyntax.Column, value);
            } // if
            else
                AddListProperty(addIndexNode, MdlSyntax.Columns, 
                    new List<IIndexColumnDefinition>(index.Columns).ConvertAll<IAstNodePropertyValue>(
                        delegate(IIndexColumnDefinition icd)
                            { return GetIndexColumnPropertyValue(icd); }));
        }

        private static IAstNodePropertyValue GetIndexColumnPropertyValue(IIndexColumnDefinition indexColumn)
        {
            if(indexColumn.SortDirection.HasValue)
                return new ListAstNodePropertyValue(
                    new StringAstNodePropertyValue(indexColumn.Name),
                    new SymbolAstNodePropertyValue(indexColumn.SortDirection.Value == SortDirection.Ascending ? "asc" : "desc"));

            return new StringAstNodePropertyValue(indexColumn.Name);
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

        private static void AddProperty(IAstNode node, string name, IAstNodePropertyValue value)
        {
            node.Properties.AddProperty(new AstNodeProperty(name, value));
        }

        private static void AddProperty(IAstNode node, string name, string value)
        {
            node.Properties.AddProperty(AstNodeProperty.String(name, value));
        }

        private static void AddProperty(IAstNode node, string name, int value)
        {
            node.Properties.AddProperty(AstNodeProperty.Integer(name, value));
        }

        private static void AddListProperty(IAstNode node, string name, IEnumerable<IAstNodePropertyValue> values)
        {
            node.Properties.AddProperty(AstNodeProperty.List(name, new List<IAstNodePropertyValue>(values).ToArray()));
        }
    }
}
