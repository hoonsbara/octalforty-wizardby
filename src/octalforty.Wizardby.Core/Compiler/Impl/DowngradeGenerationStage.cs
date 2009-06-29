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

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.SemanticModel;
using octalforty.Wizardby.Core.Util;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    public class DowngradeGenerationStage : MdlCompilerStageBase
    {
        /// <summary>
        /// Sets <paramref name="environment"/> for the current compilation session.
        /// </summary>
        /// <param name="environment"></param>
        public override void SetEnvironment(Environment environment)
        {
            //
            // We don't need existing environment here.
            base.SetEnvironment(new Environment());
        }

        /// <summary>
        /// Visits the given <paramref name="baselineNode"/>.
        /// </summary>
        /// <param name="baselineNode"></param>
        public override void Visit(IBaselineNode baselineNode)
        {
            foreach(IAstNode childNode in baselineNode.ChildNodes)
                BuildSchemaDefinitionFor(childNode);
        }

        /// <summary>
        /// Visits the given <paramref name="versionNode"/>.
        /// </summary>
        /// <param name="versionNode"></param>
        public override void Visit(IVersionNode versionNode)
        {
            //
            // Add an IDowngradeNode to versionNode if it does not
            // have one already
            // TODO: Shoud user be able to specify downgrade: expilcitly?
            versionNode.ChildNodes.Add(new DowngradeNode(versionNode));
            
            base.Visit(versionNode);
        }

        /// <summary>
        /// Visits the given <paramref name="downgradeNode"/>.
        /// </summary>
        /// <param name="downgradeNode"></param>
        public override void Visit(IDowngradeNode downgradeNode)
        {
            //
            // Do not go below
        }

        /// <summary>
        /// Visits the given <paramref name="addTableNode"/>.
        /// </summary>
        /// <param name="addTableNode"></param>
        public override void Visit(IAddTableNode addTableNode)
        {
            BuildSchemaDefinitionFor(addTableNode);

            //
            // Add IRemoveTableNode to the front if IDowngradeNode
            IDowngradeNode downgradeNode = GetDowngradeNodeFor(addTableNode);
            downgradeNode.ChildNodes.Insert(0, new RemoveTableNode(downgradeNode, addTableNode.Name));
        }

        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            BuildSchemaDefinitionFor(addIndexNode);

            //
            // Add IRemoveIndexNode
            IDowngradeNode downgradeNode = GetDowngradeNodeFor(addIndexNode);

            IRemoveIndexNode removeIndexNode = new RemoveIndexNode(downgradeNode, addIndexNode.Name);
            removeIndexNode.Table = addIndexNode.Table;

            downgradeNode.ChildNodes.Insert(0, removeIndexNode);
        }

        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            BuildSchemaDefinitionFor(addReferenceNode);

            //
            // Add IRemoveReferenceNode
            IDowngradeNode downgradeNode = GetDowngradeNodeFor(addReferenceNode);
            
            RemoveReferenceNode removeReferenceNode = new RemoveReferenceNode(downgradeNode, addReferenceNode.Name);
            removeReferenceNode.Table = addReferenceNode.FkTable;

            downgradeNode.ChildNodes.Insert(0, removeReferenceNode);
        }

        /// <summary>
        /// Visits the given <paramref name="removeTableNode"/>.
        /// </summary>
        /// <param name="removeTableNode"></param>
        public override void Visit(IRemoveTableNode removeTableNode)
        {
            ITableDefinition table = Environment.Schema.GetTable(removeTableNode.Name);

            IDowngradeNode downgradeNode = GetDowngradeNodeFor(removeTableNode);
            IAddTableNode addTableNode = new AddTableNode(removeTableNode, table.Name);

            foreach(IColumnDefinition column in table.Columns)
            {
                IAddColumnNode addColumnNode = GetAddColumnNode(addTableNode, column);
                addTableNode.ChildNodes.Add(addColumnNode);
            } // foreach

            downgradeNode.ChildNodes.Insert(0, addTableNode);
        }
        /// <summary>
        /// Visits the given <paramref name="removeReferenceNode"/>.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
            ITableDefinition table = Environment.Schema.GetTable(removeReferenceNode.Table);
            IReferenceDefinition reference = table.GetReference(removeReferenceNode.Name);

            IDowngradeNode downgradeNode = GetDowngradeNodeFor(removeReferenceNode);
            IAddReferenceNode addReferenceNode = new AddReferenceNode(downgradeNode, removeReferenceNode.Name);

            addReferenceNode.PkTable = reference.PkTable;
            Copy(reference.PkColumns, addReferenceNode.PkColumns);
            addReferenceNode.FkTable = reference.FkTable;
            Copy(reference.FkColumns, addReferenceNode.FkColumns);

            downgradeNode.ChildNodes.Insert(0, addReferenceNode);
        }

        /// <summary>
        /// Visits the given <paramref name="removeIndexNode"/>.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            ITableDefinition table = Environment.Schema.GetTable(removeIndexNode.Table);
            if(table == null)
                throw new MigrationException(removeIndexNode.Table);

            IIndexDefinition index = table.GetIndex(removeIndexNode.Name);
            if(index == null)
                throw new MigrationException(removeIndexNode.Name);

            IDowngradeNode downgradeNode = GetDowngradeNodeFor(removeIndexNode);

            IAddIndexNode addIndexNode = new AddIndexNode(downgradeNode, removeIndexNode.Name);
            addIndexNode.Clustered = index.Clustered;
            Copy(index.Columns, addIndexNode.Columns);
            addIndexNode.Table = index.Table;
            addIndexNode.Unique = index.Unique;

            downgradeNode.ChildNodes.Insert(0, addIndexNode);

            //
            // IndexDefinition must be in place here
            /*ResolveRemoveIndex(removeIndexNode);
            IIndexDefinition index = Environment.Schema.GetTable(removeIndexNode.Table).GetIndex(removeIndexNode.Name);

            IDowngradeNode downgradeNode = GetDowngradeNodeFor(removeIndexNode);
            
            IAddIndexNode addIndexNode = new AddIndexNode(downgradeNode, index.Name);
            addIndexNode.Clustered = index.Clustered;
            Copy(index.Columns, addIndexNode.Columns);
            addIndexNode.Table = index.Table;
            addIndexNode.Unique = index.Unique;

            downgradeNode.ChildNodes.Insert(0, addIndexNode);*/
        }

        /// <summary>
        /// Visits the given <paramref name="alterTableNode"/>.
        /// </summary>
        /// <param name="alterTableNode"></param>
        public override void Visit(IAlterTableNode alterTableNode)
        {
            IDowngradeNode downgradeNode = GetDowngradeNodeFor(alterTableNode);
            IAlterTableNode downgradeAlterTableNode = new AlterTableNode(downgradeNode, alterTableNode.Name);

            foreach(IAstNode astNode in alterTableNode.ChildNodes)
            {
                if(astNode is IAddColumnNode)
                {
                    BuildSchemaDefinitionFor(astNode);

                    IAddColumnNode addColumnNode = (IAddColumnNode)astNode;
                    IRemoveColumnNode removeColumnNode = 
                        new RemoveColumnNode(downgradeAlterTableNode, addColumnNode.Name);
                    removeColumnNode.Table = addColumnNode.Table;

                    downgradeAlterTableNode.ChildNodes.Insert(0, removeColumnNode);
                } // if

                if(astNode is IRemoveColumnNode)
                {
                    IRemoveColumnNode removeColumnNode = (IRemoveColumnNode)astNode;

                    ITableDefinition table = Environment.Schema.GetTable(alterTableNode.Name);

                    IColumnDefinition columnDefinition = table.GetColumn(removeColumnNode.Name);
                    table.RemoveColumn(removeColumnNode.Name);

                    IAddColumnNode addColumnNode = GetAddColumnNode(downgradeAlterTableNode, columnDefinition);

                    downgradeAlterTableNode.ChildNodes.Insert(0, addColumnNode);
                } // if

                if(astNode is IAlterColumnNode)
                {
                    IAlterColumnNode alterColumnNode = (IAlterColumnNode)astNode;
                    ITableDefinition table = Environment.Schema.GetTable(alterTableNode.Name);
                    IColumnDefinition column = table.GetColumn(alterColumnNode.Name);

                    IAlterColumnNode downgradeAlterColumnNode = 
                        new AlterColumnNode(downgradeAlterTableNode, alterColumnNode.Name);
                    downgradeAlterColumnNode.Table = table.Name;

                    downgradeAlterColumnNode.Type = column.Type;
                    downgradeAlterColumnNode.Length = column.Length;
                    if(column.Nullable.HasValue && alterColumnNode.Nullable.HasValue && column.Nullable.Value != alterColumnNode.Nullable.Value)
                    {
                        downgradeAlterColumnNode.Nullable = column.Nullable;
                    } // if

                    downgradeAlterTableNode.ChildNodes.Insert(0, downgradeAlterColumnNode);
                } // if
            } // foreach

            downgradeNode.ChildNodes.Insert(0, downgradeAlterTableNode);
        }

        /// <summary>
        /// Visits the given <paramref name="executeNativeSqlNode"/>.
        /// </summary>
        /// <param name="executeNativeSqlNode"></param>
        public override void Visit(IExecuteNativeSqlNode executeNativeSqlNode)
        {
            IDowngradeNode downgradeNode = GetDowngradeNodeFor(executeNativeSqlNode);
            
            ExecuteNativeSqlNode nativeSqlNode = new ExecuteNativeSqlNode(downgradeNode);
            nativeSqlNode.UpgradeResource = executeNativeSqlNode.UpgradeResource;
            nativeSqlNode.DowngradeResource = executeNativeSqlNode.DowngradeResource;

            downgradeNode.ChildNodes.Insert(0, nativeSqlNode);
        }

        private IDowngradeNode GetDowngradeNodeFor(IAstNode astNode)
        {
            IAstNode currentNode = astNode;
            while(!(currentNode is IVersionNode))
            {
                if(currentNode == null)
                    throw new ArgumentException(astNode.GetType().Name + " - " + astNode.Location.ToString());
                currentNode = currentNode.Parent;
            } // while

            return (IDowngradeNode)Algorithms.FindFirst(currentNode.ChildNodes, 
                delegate(IAstNode an) { return an is IDowngradeNode; });
        }

        public void Copy<T>(IEnumerable<T> source, ICollection<T> destination)
        {
            foreach(T value in source)
                destination.Add(value);
        }

        private void BuildSchemaDefinitionFor(IAstNode astNode)
        {
            SchemaDefinitionBuilderVisitor schemaDefinitionBuilder = new SchemaDefinitionBuilderVisitor(Environment.Schema);
            astNode.Accept(schemaDefinitionBuilder);
        }

        private static IAddColumnNode GetAddColumnNode(IAstNode addTableNode, IColumnDefinition column)
        {
            IAddColumnNode addColumnNode = new AddColumnNode(addTableNode, column.Name);

            addColumnNode.Identity = column.Identity;
            addColumnNode.Length = column.Length;
            addColumnNode.Nullable = column.Nullable;
            addColumnNode.Precision = column.Precision;
            addColumnNode.PrimaryKey = column.PrimaryKey;
            addColumnNode.Scale = column.Scale;
            addColumnNode.Type = column.Type;
            addColumnNode.Table = column.Table;

            return addColumnNode;
        }

    }
}
