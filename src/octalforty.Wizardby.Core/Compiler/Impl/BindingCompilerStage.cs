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
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Resources;
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
                } // else
                else if(node is IAlterColumnNode)
                {
                    IColumnDefinition columnDefinition = table.GetColumn(((IAlterColumnNode)node).Name);
                    IColumnNode columnNode = (IColumnNode)node;
                    columnNode.Type = columnDefinition.Type;
                    columnNode.Length = columnDefinition.Length;
                    BindColumnProperties(columnNode, columnDefinition);

                    AlterColumn(columnDefinition, (IAlterColumnNode)node);

                } // else if
                
                node.Accept(this);
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
                addColumnNode.Table = table.Name;

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
            if(addIndexNode.Properties[MdlSyntax.Table] != null)
            {
                indexDefinition.Table = addIndexNode.Table =
                    AstNodePropertyUtil.AsString(addIndexNode.Properties[MdlSyntax.Table].Value);
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
                throw CreateMdlCompilerException(MdlCompilerResources.CouldNotResolveTableForAddIndex,
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
            else if(addIndexNode.Properties[MdlSyntax.Column] != null)
            {
                IndexColumnDefinition indexColumnDefinition =
                    GetIndexColumnDefinition(addIndexNode.Properties[MdlSyntax.Column].Value);
                indexDefinition.Columns.Add(indexColumnDefinition);
                addIndexNode.Columns.Add(indexColumnDefinition);
            } // else if
            else if(addIndexNode.Properties[MdlSyntax.Columns] != null)
            {
                IListAstNodePropertyValue list = (IListAstNodePropertyValue)addIndexNode.Properties[MdlSyntax.Columns].Value;
                foreach(IAstNodePropertyValue value in list.Items)
                {
                    IndexColumnDefinition indexColumnDefinition = GetIndexColumnDefinition(value);
                    indexDefinition.Columns.Add(indexColumnDefinition);
                    addIndexNode.Columns.Add(indexColumnDefinition);
                } // foreach
            } // else if

            BindIndexProperties(addIndexNode, indexDefinition);
        }

        /// <summary>
        /// Visits the given <paramref name="addConstraintNode"/>.
        /// </summary>
        /// <param name="addConstraintNode"></param>
        public override void Visit(IAddConstraintNode addConstraintNode)
        {
            //
            // If we have "table" property, use that as a value for "table" property
            if(addConstraintNode.Properties[MdlSyntax.Table] != null)
                addConstraintNode.Table = AstNodePropertyUtil.AsString(addConstraintNode.Properties, MdlSyntax.Table);
            else
            {
                //
                // If we have IAddColumnNode or IAlterColumnNode as a parent, use its table name
                // plus use that column as a target for the constraint
                if(addConstraintNode.Parent is IAddColumnNode || addConstraintNode.Parent is IAlterColumnNode)
                {
                    IColumnNode columnNode = ((IColumnNode)addConstraintNode.Parent);
                    
                    addConstraintNode.Table = columnNode.Table;
                    addConstraintNode.Columns.Add(columnNode.Name);
                } // if

                //
                // If parent is IAddTableNode or IAlterTableNode, use its name
                if(addConstraintNode.Parent is IAddTableNode || addConstraintNode.Parent is IAlterTableNode)
                    addConstraintNode.Table = ((ITableNode)addConstraintNode.Parent).Name;

                addConstraintNode.Properties.AddProperty(new AstNodeProperty(MdlSyntax.Table, 
                    new StringAstNodePropertyValue(addConstraintNode.Table)));
            } // else

            //
            // If we have "default" property, this is is IDefaultConstraintDefinition
            IConstraintDefinition constraintDefinition = null;
            if(addConstraintNode.Properties[MdlSyntax.Default] != null)
            {
                IAstNodePropertyValue value = addConstraintNode.Properties[MdlSyntax.Default].Value;
                constraintDefinition =
                    new DefaultConstraintDefinition(addConstraintNode.Name,
                        addConstraintNode.Table,
                        value is IIntegerAstNodePropertyValue ?
                            AstNodePropertyUtil.AsInteger(value).ToString() :
                            AstNodePropertyUtil.AsString(value));
                Environment.Schema.GetTable(constraintDefinition.Table).AddConstraint(constraintDefinition);
            } // if

            if(addConstraintNode.Columns.Count > 0)
                constraintDefinition.Columns.Add(addConstraintNode.Columns[0]);

            //
            // Look for "columns" or "column" properties
            if(addConstraintNode.Properties[MdlSyntax.Column] != null)
            {
                string columnName = AstNodePropertyUtil.AsString(
                    addConstraintNode.Properties[MdlSyntax.Column].Value);
                
                addConstraintNode.Columns.Add(columnName);
                constraintDefinition.Columns.Add(columnName);
            } // if

            if(addConstraintNode.Properties[MdlSyntax.Columns] != null)
            {
                IListAstNodePropertyValue columns =
                    (IListAstNodePropertyValue)addConstraintNode.Properties[MdlSyntax.Columns].Value;
                foreach (IAstNodePropertyValue column in columns.Items)
                {
                    string columnName = AstNodePropertyUtil.AsString(column);
                    
                    addConstraintNode.Columns.Add(columnName);
                    constraintDefinition.Columns.Add(columnName);
                }
            } // if
        }

        /// <summary>
        /// Visits the given <paramref name="removeConstraintNode"/>.
        /// </summary>
        /// <param name="removeConstraintNode"></param>
        public override void Visit(IRemoveConstraintNode removeConstraintNode)
        {
            //
            // If we have a "table" property, use that
            if(removeConstraintNode.Properties[MdlSyntax.Table] != null)
                removeConstraintNode.Table = AstNodePropertyUtil.AsString(removeConstraintNode.Properties, MdlSyntax.Table);
            else
            {
                if(removeConstraintNode.Parent is IAlterTableNode)
                    removeConstraintNode.Table = ((IAlterTableNode)removeConstraintNode.Parent).Name;
            } // else
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
            if(removeIndexNode.Properties[MdlSyntax.Table] != null)
            {
                removeIndexNode.Table = 
                    AstNodePropertyUtil.AsString(removeIndexNode.Properties[MdlSyntax.Table].Value);
            } // if
            else if(removeIndexNode.Parent is IAlterTableNode)
            {
                removeIndexNode.Table =
                    ((ITableNode)removeIndexNode.Parent).Name;
            } // else if
            else
                throw CreateMdlCompilerException(
                    MdlCompilerResources.CouldNotResolveTableForRemoveIndex, removeIndexNode.Name);
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
                    ITableDefinition pkTable = Environment.Schema.GetTable(addReferenceNode.PkTable);
                    IColumnDefinition pkColumn = pkTable.GetColumn(addReferenceNode.PkColumns[0]);
                    IColumnDefinition fkColumn =
                        Environment.Schema.GetTable(((ITableNode)addColumnNode.Parent).Name).GetColumn(addColumnNode.Name);

                    addColumnNode.Type = fkColumn.Type = pkColumn.Type;

                    if(!addColumnNode.Nullable.HasValue || !fkColumn.Nullable.HasValue)
                        addColumnNode.Nullable = fkColumn.Nullable = false;
                } // if
            } // if
        }

        /// <summary>
        /// Visits the given <paramref name="removeReferenceNode"/>.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
            if(removeReferenceNode.Properties[MdlSyntax.FkTable] != null)
            {
                ITableDefinition table =
                    Environment.Schema.GetTable(AstNodePropertyUtil.AsString(removeReferenceNode.Properties[MdlSyntax.FkTable].Value));
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

        /// <summary>
        /// Visits the given <paramref name="executeNativeSqlNode"/>.
        /// </summary>
        /// <param name="executeNativeSqlNode"></param>
        public override void Visit(IExecuteNativeSqlNode executeNativeSqlNode)
        {
            if(executeNativeSqlNode.Properties[MdlSyntax.UpgradeResource] != null)
                executeNativeSqlNode.UpgradeResource =
                    AstNodePropertyUtil.AsString(executeNativeSqlNode.Properties[MdlSyntax.UpgradeResource].Value);

            if(executeNativeSqlNode.Properties[MdlSyntax.DowngradeResource] != null)
                executeNativeSqlNode.DowngradeResource =
                    AstNodePropertyUtil.AsString(executeNativeSqlNode.Properties[MdlSyntax.DowngradeResource].Value);
        }
        #endregion

        private void BindPrimaryKeyColumns(IAddReferenceNode addReferenceNode, IReferenceDefinition reference)
        {
            //
            // Presence of "pk-column" means we're referencing only one column.
            if(addReferenceNode.Properties[MdlSyntax.PkColumn] != null)
            {
                //
                // Primary key table must already be bound, so we can use just that.
                IColumnDefinition pkColumn =
                    Environment.Schema.GetTable(reference.PkTable).GetColumn(
                        AstNodePropertyUtil.AsString(addReferenceNode.Properties[MdlSyntax.PkColumn].Value));
                
                addReferenceNode.PkColumns.Add(pkColumn.Name);
                reference.PkColumns.Add(pkColumn.Name);
            } // if
            else
            {
                //
                // Neither pk-column nor pk-columns were specified, so we assume
                // that this is a reference to the primary key of the pk-table
                var pkColumns = Environment.Schema.GetTable(reference.PkTable).GetPrimaryKeyColumns();
                if(pkColumns == null)
                    throw new MigrationException(string.Format("Table '{0}' has not Primary Key defined", reference.PkTable));

                if(pkColumns.Length > 1)
                    throw new MigrationException(
                        string.Format("Table '{0}' has Composite Primary Key defined. You have to explicitly specify 'pk-column` property", 
                            reference.PkTable));
                
                addReferenceNode.PkColumns.Add(pkColumns[0].Name);
                reference.PkColumns.Add(pkColumns[0].Name);
            }
        }

        private void BindPrimaryKeyTable(IAddReferenceNode addReferenceNode, IReferenceDefinition reference)
        {
            if(addReferenceNode.Properties[MdlSyntax.PkTable] != null)
            {
                string pkTableName = AstNodePropertyUtil.AsString(addReferenceNode.Properties[MdlSyntax.PkTable].Value);
                ITableDefinition pkTable =
                    Environment.Schema.GetTable(pkTableName);

                if(pkTable == null)
                    throw new MigrationException(string.Format("Could not resolve Primary Key table '{0}'",  pkTableName));

                addReferenceNode.PkTable = pkTable.Name;
                reference.PkTable = pkTable.Name;

                return;
            } // if

            throw CreateMdlCompilerException(MdlCompilerResources.CouldNotResolvePkTableForAddReference, 
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
            if(addReferenceNode.Properties[MdlSyntax.FkColumn] != null)
            {
                //
                // FK table is already bound here
                IColumnDefinition fkColumn =
                    Environment.Schema.GetTable(reference.FkTable).GetColumn(
                        AstNodePropertyUtil.AsString(addReferenceNode.Properties[MdlSyntax.FkColumn].Value));
                
                addReferenceNode.FkColumns.Add(fkColumn.Name);
                reference.FkColumns.Add(fkColumn.Name);

                return;
            } // if

            //
            // If we have fk-columns specified
            if(addReferenceNode.Properties[MdlSyntax.FkColumns] != null)
            {
                IListAstNodePropertyValue list =
                    (IListAstNodePropertyValue)addReferenceNode.Properties[MdlSyntax.FkColumns].Value;
                foreach(IAstNodePropertyValue value in list.Items)
                {
                    string fkColumnName = ((IStringAstNodePropertyValue)value).Value;

                    //
                    // FK table is already bound here
                    IColumnDefinition fkColumn =
                        Environment.Schema.GetTable(reference.FkTable).GetColumn(fkColumnName);

                    addReferenceNode.FkColumns.Add(fkColumn.Name);
                    reference.FkColumns.Add(fkColumn.Name);
                } // foreach

                return;
            } // if

            throw CreateMdlCompilerException(MdlCompilerResources.CouldNotResolveFkColumnForAddReference, addReferenceNode.Name);
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
            if(addReferenceNode.Properties[MdlSyntax.FkTable] != null)
            {
                ITableDefinition fkTable =
                    Environment.Schema.GetTable(AstNodePropertyUtil.AsString(addReferenceNode.Properties[MdlSyntax.FkTable].Value));
                addReferenceNode.FkTable = reference.FkTable = fkTable.Name;

                return;
            } // if

            throw CreateMdlCompilerException(MdlCompilerResources.CouldNotResolveFkTableForAddReference,
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
            if(columnNode.Properties[MdlSyntax.PrimaryKey] != null)
                columnNode.PrimaryKey = columnDefinition.PrimaryKey = 
                    columnNode.Properties.AsBoolean(MdlSyntax.PrimaryKey);

            if (columnNode.Properties[MdlSyntax.Nullable] != null)
                columnNode.Nullable = columnDefinition.Nullable =
                    columnNode.Properties.AsBoolean(MdlSyntax.Nullable);

            if (columnNode.Properties[MdlSyntax.Length] != null)
                columnNode.Length = columnDefinition.Length =
                    AstNodePropertyUtil.AsInteger(columnNode.Properties, MdlSyntax.Length);
            //
            // Type aliases have already been resolved, so we can freely parse textual type representation.
            if(columnNode.Properties[MdlSyntax.Type] != null)
            {
                string textualType = AstNodePropertyUtil.AsString(columnNode.Properties, MdlSyntax.Type);

                try
                {
                    columnNode.Type = columnDefinition.Type =
                        (DbType)Enum.Parse(typeof(DbType), textualType, true);
                } // try
                catch(ArgumentException e)
                {
                    throw new MdlCompilerException(string.Format(MdlCompilerResources.UnknownType, textualType), e);
                } // catch
            } // if

            if(columnNode.Properties[MdlSyntax.Scale] != null)
                columnNode.Scale = columnDefinition.Scale =
                    AstNodePropertyUtil.AsInteger(columnNode.Properties, MdlSyntax.Scale);

            if(columnNode.Properties[MdlSyntax.Precision] != null)
                columnNode.Precision = columnDefinition.Precision =
                    AstNodePropertyUtil.AsInteger(columnNode.Properties, MdlSyntax.Precision);

            if(columnNode.Properties[MdlSyntax.Identity] != null)
                columnNode.Identity = columnDefinition.Identity =
                    Convert.ToBoolean(AstNodePropertyUtil.AsString(columnNode.Properties, MdlSyntax.Identity));

            if(columnNode.Properties[MdlSyntax.Table] != null)
                columnNode.Table = columnDefinition.Table =
                    AstNodePropertyUtil.AsString(columnNode.Properties, MdlSyntax.Table);
        }

        private static void BindIndexProperties(IAddIndexNode addIndexNode, IIndexDefinition indexDefinition)
        {
            if(addIndexNode.Properties[MdlSyntax.Unique] != null)
                addIndexNode.Unique = indexDefinition.Unique =
                    Convert.ToBoolean(((IStringAstNodePropertyValue)addIndexNode.Properties[MdlSyntax.Unique].Value).Value);

            if(addIndexNode.Properties[MdlSyntax.Clustered] != null)
                addIndexNode.Clustered = indexDefinition.Clustered =
                    Convert.ToBoolean(((IStringAstNodePropertyValue)addIndexNode.Properties[MdlSyntax.Clustered].Value).Value);
        }

        private static MdlCompilerException CreateMdlCompilerException(string format, params object[] args)
        {
            return new MdlCompilerException(string.Format(format, args));
        }

        private IndexColumnDefinition GetIndexColumnDefinition(IAstNodePropertyValue value)
        {
            if(value is IStringAstNodePropertyValue)
                return new IndexColumnDefinition(((IStringAstNodePropertyValue)value).Value);
            
            if(value is IListAstNodePropertyValue)
            {
                IListAstNodePropertyValue list = (IListAstNodePropertyValue)value;

                return new IndexColumnDefinition(((IStringAstNodePropertyValue)list.Items[0]).Value, 
                    GetSortDirection(((IStringAstNodePropertyValue)list.Items[1]).Value));
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
