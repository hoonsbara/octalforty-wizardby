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
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    public class BindingCompilerStage : MdlCompilerStageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingCompilerStage"/> class.
        /// </summary>
        public BindingCompilerStage()
        {
        }

        #region MdlCompilerStageBase Members
        /// <summary>
        ///  Visits the given <paramref name="templatesNode" />.
        /// </summary>
        /// <param name="templatesNode"></param>
        public override void Visit(ITemplatesNode templatesNode)
        {
        }

        /// <summary>
        /// Visits the given <paramref name="removeTableNode"/>.
        /// </summary>
        /// <param name="removeTableNode"></param>
        public override void Visit(IRemoveTableNode removeTableNode)
        {
            Environment.Schema.RemoveTable(removeTableNode.Name);
        }

        /// <summary>
        /// Visits the given <paramref name="alterTableNode"/>.
        /// </summary>
        /// <param name="alterTableNode"></param>
        public override void Visit(IAlterTableNode alterTableNode)
        {
            ITableDefinition table = Environment.Schema.GetTable(alterTableNode.Name);

            foreach(IAstNode node in alterTableNode.ChildNodes)
            {
                if(node is IRemoveColumnNode)
                    table.RemoveColumn(((IRemoveColumnNode)node).Name);
                else if(node is IAddColumnNode)
                {
                    IAddColumnNode addColumnNode = (IAddColumnNode)node;
                    table.AddColumn(BindAddColumn(addColumnNode));
                    addColumnNode.Table = table.Name;
                    node.Accept(this);
                } // else
                else if(node is IAlterColumnNode)
                {
                    IColumnDefinition columnDefinition = table.GetColumn(((IAlterColumnNode)node).Name);
                    IColumnNode columnNode = (IColumnNode)node;
                    columnNode.Type = columnDefinition.Type;
                    columnNode.Length = columnDefinition.Length;
                    BindColumnProperties(columnNode, columnDefinition);

                    AlterColumn(columnDefinition, (IAlterColumnNode)node);

                    node.Accept(this);
                } // else if
                else if(node is IAddIndexNode)
                {
                    node.Accept(this);
                } // else if
                else if(node is IRemoveReferenceNode)
                {
                    node.Accept(this);
                } // else if
                else if(node is IRemoveIndexNode)
                {
                    node.Accept(this);
                } // else if
            } // foreach
        }

        /// <summary>
        /// Visits the given <paramref name="addTableNode"/>.
        /// </summary>
        /// <param name="addTableNode"></param>
        public override void Visit(IAddTableNode addTableNode)
        {
            TableDefinition table = new TableDefinition(addTableNode.Name);
            Environment.Schema.AddTable(table);

            foreach(IAddColumnNode addColumnNode in Filter<IAddColumnNode>(addTableNode.ChildNodes))
            {
                table.AddColumn(BindAddColumn(addColumnNode));
                addColumnNode.Accept(this);
            } // foreach

            //
            // Now process all but IAddColumnNode
            foreach(IAstNode astNode in FilterNot<IAddColumnNode>(addTableNode.ChildNodes))
            {
                astNode.Accept(this);
            } // foreach
        }

        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            ReferenceDefinition reference = new ReferenceDefinition(addReferenceNode.Name);

            BindForeignKeyTable(addReferenceNode, reference);
            BindForeignKeyColumns(addReferenceNode, reference);

            BindPrimaryKeyTable(addReferenceNode, reference);
            BindPrimaryKeyColumns(addReferenceNode, reference);

            ITableDefinition table = Environment.Schema.GetTable(reference.FkTable);
            table.AddReference(reference);
        }

        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            IndexDefinition indexDefinition = new IndexDefinition(addIndexNode.Name);

            //
            // If we have a "table" attribute, use it as a table name.
            if(addIndexNode.Properties.ContainsProperty(MdlSyntax.Table))
            {
                indexDefinition.Table = addIndexNode.Table =
                    addIndexNode.Properties[MdlSyntax.Table].Value.ToString();
            } // if
            else if(addIndexNode.Parent is IAddColumnNode || addIndexNode.Parent is IAlterColumnNode)
            {
                indexDefinition.Table = addIndexNode.Table =
                    ((ITableNode)addIndexNode.Parent.Parent).Name;
            } // else if
            else if(addIndexNode.Parent is IAddTableNode || addIndexNode.Parent is IAlterTableNode)
            {
                indexDefinition.Table = addIndexNode.Table =
                    ((ITableNode)addIndexNode.Parent).Name;
            } // else if
            else
                throw CreateMdlCompilerException(Resources.BindingCompilerStage.CouldNotResolveTableForAddIndex,
                    addIndexNode.Name);

            ITableDefinition table = Environment.Schema.GetTable(indexDefinition.Table);
            table.AddIndex(indexDefinition);

            //
            // If our parent is IAddColumn or IAlterColumn, use it as indexed column
            if(addIndexNode.Parent is IAddColumnNode || addIndexNode.Parent is IAlterColumnNode)
            {
                string indexColumnName = ((IColumnNode)addIndexNode.Parent).Name;
                IndexColumnDefinition indexColumnDefinition = new IndexColumnDefinition(indexColumnName);

                indexDefinition.Columns.Add(indexColumnDefinition);
                addIndexNode.Columns.Add(indexColumnDefinition);
            } // if
            else if(addIndexNode.Properties.ContainsProperty(MdlSyntax.Column))
            {
                IndexColumnDefinition indexColumnDefinition =
                    GetIndexColumnDefinition(addIndexNode.Properties[MdlSyntax.Column].Value);
                indexDefinition.Columns.Add(indexColumnDefinition);
                addIndexNode.Columns.Add(indexColumnDefinition);
            } // else if
            else if(addIndexNode.Properties.ContainsProperty(MdlSyntax.Columns))
            {
                foreach(object icd in (object[])addIndexNode.Properties[MdlSyntax.Columns].Value)
                {
                    IndexColumnDefinition indexColumnDefinition = GetIndexColumnDefinition(icd);
                    indexDefinition.Columns.Add(indexColumnDefinition);
                    addIndexNode.Columns.Add(indexColumnDefinition);
                } // foreach
            } // else if

            BindIndexProperties(addIndexNode, indexDefinition);
        }

        /// <summary>
        /// Visits the given <paramref name="removeIndexNode"/>.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            ResolveRemoveIndex(removeIndexNode);

            ITableDefinition table = Environment.Schema.GetTable(removeIndexNode.Table);
            table.RemoveIndex(removeIndexNode.Name);
        }

        protected void ResolveRemoveIndex(IRemoveIndexNode removeIndexNode)
        {
            //
            // Resolve table name
            if(removeIndexNode.Properties.ContainsProperty(MdlSyntax.Table))
            {
                removeIndexNode.Table = 
                    removeIndexNode.Properties[MdlSyntax.Table].Value.ToString();
            } // if
            else if(removeIndexNode.Parent is IAlterTableNode)
            {
                removeIndexNode.Table =
                    ((ITableNode)removeIndexNode.Parent).Name;
            } // else if
            else
                throw CreateMdlCompilerException(
                    Resources.BindingCompilerStage.CouldNotResolveTableForRemoveIndex, removeIndexNode.Name);
        }

        /// <summary>
        /// Visits the given <paramref name="addColumnNode"/>.
        /// </summary>
        /// <param name="addColumnNode"></param>
        public override void Visit(IAddColumnNode addColumnNode)
        {
            base.Visit(addColumnNode);

            //
            // If no "type" is specified, we can get that from the type of the
            // referenced column, if there is one
            if(addColumnNode.Type == null)
            {
                IAddReferenceNode addReferenceNode = GetFirst<IAddReferenceNode>(addColumnNode.ChildNodes);
                if(addReferenceNode != null)
                {
                    ITableDefinition table = Environment.Schema.GetTable(addReferenceNode.PkTable);
                    IColumnDefinition column = table.GetColumn(addReferenceNode.PkColumns[0]);
                    IColumnDefinition column2 =
                        Environment.Schema.GetTable(((ITableNode)addColumnNode.Parent).Name).GetColumn(addColumnNode.Name);

                    addColumnNode.Type = column2.Type = column.Type;
                } // if
            } // if
        }

        /// <summary>
        /// Visits the given <paramref name="removeReferenceNode"/>.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
            if(removeReferenceNode.Properties.ContainsProperty(MdlSyntax.FkTable))
            {
                ITableDefinition table =
                    Environment.Schema.GetTable(removeReferenceNode.Properties[MdlSyntax.FkTable].Value.ToString());
                table.RemoveReference(removeReferenceNode.Name);

                removeReferenceNode.Table = table.Name;

                return;
            } // if

            //
            // If this node is a child node of IAlterTableNode, use that tables' name
            // as the FkTable
            if(IsImmediateChildOf<IAlterTableNode>(removeReferenceNode))
            {
                IAlterTableNode alterTableNode = (IAlterTableNode)removeReferenceNode.Parent;
                
                ITableDefinition table = Environment.Schema.GetTable(alterTableNode.Name);
                
                table.RemoveReference(removeReferenceNode.Name);
                removeReferenceNode.Table = table.Name;
            } // if
        }
        #endregion

        private void BindPrimaryKeyColumns(IAddReferenceNode addReferenceNode, IReferenceDefinition reference)
        {
            //
            // Presence of "pk-column" means we're referencing only one column.
            if(addReferenceNode.Properties.ContainsProperty(MdlSyntax.PkColumn))
            {
                //
                // Primary key table must already be bound, so we can use just that.
                IColumnDefinition pkColumn =
                    Environment.Schema.GetTable(reference.PkTable).GetColumn(
                        addReferenceNode.Properties[MdlSyntax.PkColumn].Value.ToString());
                
                addReferenceNode.PkColumns.Add(pkColumn.Name);
                reference.PkColumns.Add(pkColumn.Name);
            } // if
            else
            {
                //
                // Neither pk-column nor pk-columns were specified, so we assume
                // that this is a reference to the primary key of the pk-table
                IColumnDefinition pkColumn = Environment.Schema.GetTable(reference.PkTable).GetPrimaryKeyColumn();
                
                addReferenceNode.PkColumns.Add(pkColumn.Name);
                reference.PkColumns.Add(pkColumn.Name);
            }
        }

        private void BindPrimaryKeyTable(IAddReferenceNode addReferenceNode, IReferenceDefinition reference)
        {
            if(addReferenceNode.Properties.ContainsProperty(MdlSyntax.PkTable))
            {
                ITableDefinition pkTable =
                    Environment.Schema.GetTable(addReferenceNode.Properties[MdlSyntax.PkTable].Value.ToString());

                addReferenceNode.PkTable = pkTable.Name;
                reference.PkTable = pkTable.Name;

                return;
            } // if

            throw CreateMdlCompilerException(Resources.BindingCompilerStage.CouldNotResolvePkTableForAddReference, 
                addReferenceNode.Name);
        }

        private void BindForeignKeyColumns(IAddReferenceNode addReferenceNode, IReferenceDefinition reference)
        {
            //
            // If the parent of IAddReferenceNode is an IAddColumnNode or IAlterColumnNode, that column becomes the fk column
            if(addReferenceNode.Parent is IAddColumnNode || addReferenceNode.Parent is IAlterColumnNode)
            {
                string fkColumn = 
                    Environment.Schema.GetTable(reference.FkTable).GetColumn(((IAddColumnNode)addReferenceNode.Parent).Name).Name;
                
                addReferenceNode.FkColumns.Add(fkColumn);
                reference.FkColumns.Add(fkColumn);
                
                return;
            } // if

            //
            // fk-column must be explicitly specified
            if(addReferenceNode.Properties.ContainsProperty(MdlSyntax.FkColumn))
            {
                //
                // FK table is already bound here
                IColumnDefinition fkColumn =
                    Environment.Schema.GetTable(reference.FkTable).GetColumn(addReferenceNode.Properties[MdlSyntax.FkColumn].Value.ToString());
                
                addReferenceNode.FkColumns.Add(fkColumn.Name);
                reference.FkColumns.Add(fkColumn.Name);

                return;
            } // if

            //
            // If we have fk-columns specified
            if(addReferenceNode.Properties.ContainsProperty(MdlSyntax.FkColumns))
            {
                foreach(string fkColumnName in (object[])addReferenceNode.Properties[MdlSyntax.FkColumns].Value)
                {
                    //
                    // FK table is already bound here
                    IColumnDefinition fkColumn =
                        Environment.Schema.GetTable(reference.FkTable).GetColumn(fkColumnName);

                    addReferenceNode.FkColumns.Add(fkColumn.Name);
                    reference.FkColumns.Add(fkColumn.Name);
                } // foreach

                return;
            } // if

            throw CreateMdlCompilerException(Resources.BindingCompilerStage.CouldNotResolveFkColumnForAddReference, addReferenceNode.Name);
        }

        private void BindForeignKeyTable(IAddReferenceNode addReferenceNode, IReferenceDefinition reference)
        {
            // If the parent of IAddReferenceNode is either IAddTableNode or IAlterTableNode,
            // ForeignKeyTable is the binding for that node
            if(addReferenceNode.Parent is IAddTableNode || addReferenceNode.PkColumns is IAlterTableNode)
            {
                addReferenceNode.FkTable = reference.FkTable = 
                    Environment.Schema.GetTable(((ITableNode)addReferenceNode.Parent).Name).Name;
                return;
            } // if

            //
            // If parent is IAddColumnNode or IAlterColumnNode, we just move on level higher 
            // and do basically the same
            if(addReferenceNode.Parent is IAddColumnNode || addReferenceNode.PkColumns is IAlterColumnNode)
            {
                addReferenceNode.FkTable = reference.FkTable =
                    Environment.Schema.GetTable(((ITableNode)addReferenceNode.Parent.Parent).Name).Name;
                return;
            } // if

            //
            // If foreign key table is explicitly specified in properties, use that
            if(addReferenceNode.Properties.ContainsProperty(MdlSyntax.FkTable))
            {
                ITableDefinition fkTable =
                    Environment.Schema.GetTable(addReferenceNode.Properties[MdlSyntax.FkTable].Value.ToString());
                addReferenceNode.FkTable = reference.FkTable = fkTable.Name;

                return;
            } // if

            throw CreateMdlCompilerException(Resources.BindingCompilerStage.CouldNotResolveFkTableForAddReference,
                addReferenceNode.Name);
        }

        private ColumnDefinition BindAddColumn(IAddColumnNode addColumnNode)
        {
            ColumnDefinition column = new ColumnDefinition(addColumnNode.Name);
            BindColumnProperties(addColumnNode, column);

            return column;
        }

        private static void BindColumnProperties(IColumnNode columnNode, IColumnDefinition columnDefinition)
        {
            if(columnNode.Properties.ContainsProperty(MdlSyntax.PrimaryKey))
                columnNode.PrimaryKey = columnDefinition.PrimaryKey =
                    Convert.ToBoolean(columnNode.Properties[MdlSyntax.PrimaryKey].Value);

            if(columnNode.Properties.ContainsProperty(MdlSyntax.Nullable))
                columnNode.Nullable = columnDefinition.Nullable =
                    Convert.ToBoolean(columnNode.Properties[MdlSyntax.Nullable].Value);

            if(columnNode.Properties.ContainsProperty(MdlSyntax.Length))
                columnNode.Length = columnDefinition.Length =
                    Convert.ToInt32(columnNode.Properties[MdlSyntax.Length].Value);
            //
            // Type aliases have already been resolved, so we can freely parse textual type representation.
            if(columnNode.Properties.ContainsProperty(MdlSyntax.Type))
                columnNode.Type = columnDefinition.Type =
                    (DbType)Enum.Parse(typeof(DbType), columnNode.Properties[MdlSyntax.Type].Value.ToString());

            if(columnNode.Properties.ContainsProperty(MdlSyntax.Scale))
                columnNode.Scale = columnDefinition.Scale =
                    Convert.ToInt32(columnNode.Properties[MdlSyntax.Scale].Value);

            if(columnNode.Properties.ContainsProperty(MdlSyntax.Precision))
                columnNode.Precision = columnDefinition.Precision =
                    Convert.ToInt32(columnNode.Properties[MdlSyntax.Precision].Value);

            if(columnNode.Properties.ContainsProperty(MdlSyntax.Identity))
                columnNode.Identity = columnDefinition.Identity =
                    Convert.ToBoolean(columnNode.Properties[MdlSyntax.Identity].Value);

            if(columnNode.Properties.ContainsProperty(MdlSyntax.Default))
                columnNode.Default = columnDefinition.Default =
                    columnNode.Properties[MdlSyntax.Default].Value.ToString();
        }

        private static void BindIndexProperties(IAddIndexNode addIndexNode, IIndexDefinition indexDefinition)
        {
            if(addIndexNode.Properties.ContainsProperty(MdlSyntax.Unique))
                addIndexNode.Unique = indexDefinition.Unique =
                    Convert.ToBoolean(addIndexNode.Properties[MdlSyntax.Unique].Value);

            if(addIndexNode.Properties.ContainsProperty(MdlSyntax.Clustered))
                addIndexNode.Clustered = indexDefinition.Clustered =
                    Convert.ToBoolean(addIndexNode.Properties[MdlSyntax.Clustered].Value);
        }

        private static MdlCompilerException CreateMdlCompilerException(string format, params object[] args)
        {
            return new MdlCompilerException(string.Format(format, args));
        }

        private IndexColumnDefinition GetIndexColumnDefinition(object value)
        {
            if(value is string)
                return new IndexColumnDefinition((string)value);
            
            if(value is object[])
            {
                object[] icd = (object[])value;

                return new IndexColumnDefinition((string)icd[0], GetSortDirection((string)icd[1]));
            } //

            throw new ArgumentOutOfRangeException("value");
        }

        private SortDirection GetSortDirection(string s)
        {
            switch(s)
            {
                case "asc":
                case "ascending":
                    return SortDirection.Ascending;
                case "desc":
                case "descending":
                    return SortDirection.Descending;
                default:
                    throw new ArgumentOutOfRangeException("s");
            } // switch
        }

        private void AlterColumn(IColumnDefinition definition, IAlterColumnNode alterColumnNode)
        {
            if(alterColumnNode.Length.HasValue)
                definition.Length = alterColumnNode.Length;

            if(alterColumnNode.Nullable.HasValue)
                definition.Nullable = alterColumnNode.Nullable;
        }
    }
}
