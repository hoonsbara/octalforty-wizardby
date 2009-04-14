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
using System.Diagnostics;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Compiler
{
    public class SchemaDefinitionBuilderVisitor : AstVisitorBase
    {
        private Schema schema;

        public Schema Schema
        {
            [DebuggerStepThrough]
            get { return schema; }
            [DebuggerStepThrough]
            set { schema = value; }
        }

        public SchemaDefinitionBuilderVisitor(Schema schema)
        {
            this.schema = schema;
        }

        #region AstVisitorBase Members
        /// <summary>
        ///  Visits the given <paramref name="addTableNode" />.
        /// </summary>
        /// <param name="addTableNode"></param>
        public override void Visit(IAddTableNode addTableNode)
        {
            TableDefinition tableDefinition = new TableDefinition(addTableNode.Name);
            schema.AddTable(tableDefinition);

            Visit(addTableNode.ChildNodes);
        }

        /// <summary>
        ///  Visits the given <paramref name="addColumnNode" />.
        /// </summary>
        /// <param name="addColumnNode"></param>
        public override void Visit(IAddColumnNode addColumnNode)
        {
            ITableDefinition table = schema.GetTable(((ITableNode)addColumnNode.Parent).Name);
            table.AddColumn(new ColumnDefinition(addColumnNode.Name, table.Name, addColumnNode.Type, addColumnNode.Nullable, 
                addColumnNode.Length, addColumnNode.Scale, addColumnNode.Precision, addColumnNode.PrimaryKey, addColumnNode.Identity));
        }

        /// <summary>
        ///  Visits the given <paramref name="addIndexNode" />.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            ITableDefinition table = schema.GetTable(addIndexNode.Table);
            table.AddIndex(addIndexNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="addReferenceNode" />.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            ITableDefinition table = schema.GetTable(addReferenceNode.FkTable);

            ReferenceDefinition reference = 
                new ReferenceDefinition(addReferenceNode.Name, addReferenceNode.PkTable, addReferenceNode.FkTable);
            Copy(addReferenceNode.PkColumns, reference.PkColumns);
            Copy(addReferenceNode.FkColumns, reference.FkColumns);

            table.AddReference(reference);
        }
        #endregion

        public static void Copy<T>(IEnumerable<T> source, ICollection<T> destination)
        {
            foreach(T value in source)
                destination.Add(value);
        }
    }
}
