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

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Tests.Core.Compiler.Impl
{
    public class AstTestFixtureBase
    {
        public static void AssertAddTable(IAstNode astNode, string name, params IColumnDefinition[] columns)
        {
            Assert.IsInstanceOfType(typeof(IAddTableNode), astNode);

            IAddTableNode addTableNode = (IAddTableNode)astNode;
            Assert.AreEqual(name, addTableNode.Name);

            if(columns == null)
                return;

            Assert.AreEqual(columns.Length, addTableNode.ChildNodes.Count);
            for(int i = 0; i < columns.Length; ++i)
            {
                AssertAddColumn(addTableNode.ChildNodes[i], columns[i].Name, columns[i].Table, columns[i].Type,
                    columns[i].Length, columns[i].Scale, columns[i].Precision, columns[i].PrimaryKey, columns[i].Identity, columns[i].Nullable);
            } // for
        }

        public static void AssertAddColumn(IAstNode astNode, string name, string table, DbType? type, int? length, int? scale, 
            int? precision, bool? primaryKey, bool? identity, bool? nullable)
        {
            Assert.IsInstanceOfType(typeof(IAddColumnNode), astNode);

            IAddColumnNode addColumnNode = (IAddColumnNode)astNode;
            AssertColumnNode(addColumnNode, name, table, type, length, scale, precision, primaryKey, identity, nullable);
        }

        public static void AssertRemoveColumn(IAstNode astNode, string name, string table)
        {
            Assert.IsInstanceOfType(typeof(IRemoveColumnNode), astNode);

            IRemoveColumnNode removeColumnNode = (IRemoveColumnNode)astNode;
            Assert.AreEqual(name, removeColumnNode.Name);
            Assert.AreEqual(table, removeColumnNode.Table);
        }

        public static void AssertAlterColumn(IAstNode astNode, string name, string table, DbType type, int? length, int? scale, 
                                             int? precision, bool? primaryKey, bool? identity, bool? nullable)
        {
            Assert.IsInstanceOfType(typeof(IAlterColumnNode), astNode);

            IAlterColumnNode alterColumnNode = (IAlterColumnNode)astNode;
            AssertColumnNode(alterColumnNode, name, table, type, length, scale, precision, primaryKey, identity, nullable);
        }

        private static void AssertColumnNode(IColumnDefinition columnDefinition, string name, string table, 
            DbType? type, int? length, int? scale, int? precision, bool? primaryKey, bool? identity, bool? nullable)
        {
            Assert.AreEqual(name, columnDefinition.Name);
            
            if(!string.IsNullOrEmpty(table))
                Assert.AreEqual(table, columnDefinition.Table);

            if(type.HasValue)
                Assert.AreEqual(type, columnDefinition.Type.Value);

            if(length != null)
                Assert.AreEqual(length.Value, columnDefinition.Length.Value);

            if(scale != null)
                Assert.AreEqual(scale.Value, columnDefinition.Scale.Value);

            if(precision != null)
                Assert.AreEqual(precision.Value, columnDefinition.Precision.Value);

            if(primaryKey != null)
                Assert.AreEqual(primaryKey.Value, columnDefinition.PrimaryKey.Value);

            if(identity != null)
                Assert.AreEqual(identity.Value, columnDefinition.Identity.Value);

            if(nullable != null)
                Assert.AreEqual(nullable.Value, columnDefinition.Nullable.Value);
        }

        public static void AssertAlterTable(IAstNode astNode, string name)
        {
            Assert.IsInstanceOfType(typeof(IAlterTableNode), astNode);

            IAlterTableNode alterTableNode = (IAlterTableNode)astNode;
            Assert.AreEqual(name, alterTableNode.Name);
        }

        public static void AssertAddReference(IAstNode astNode, string name, string pkTable, string[] pkColumns, string fkTable, string[] fkColumns)
        {
            Assert.IsInstanceOfType(typeof(IAddReferenceNode), astNode);

            IAddReferenceNode addReferenceNode = (IAddReferenceNode)astNode;

            if(!string.IsNullOrEmpty(pkTable))
                Assert.AreEqual(pkTable, addReferenceNode.PkTable);

            if(pkColumns != null)
                Assert.AreEqual(pkColumns, addReferenceNode.PkColumns);

            if(!string.IsNullOrEmpty(fkTable))
                Assert.AreEqual(fkTable, addReferenceNode.FkTable);

            if(fkColumns != null)
                Assert.AreEqual(fkColumns, addReferenceNode.FkColumns);
        }

        public static void AssertAddIndex(IAstNode astNode, string name, bool? clustered, bool? unique,
                                          params IIndexColumnDefinition[] columns)
        {
            Assert.IsInstanceOfType(typeof(IAddIndexNode), astNode);

            IAddIndexNode addIndexNode = (IAddIndexNode)astNode;
            Assert.AreEqual(name, addIndexNode.Name);

            if(clustered.HasValue)
                Assert.AreEqual(clustered.Value, addIndexNode.Clustered.Value);

            if(unique.HasValue)
                Assert.AreEqual(unique.Value, addIndexNode.Unique.Value);

            if(columns == null) 
                return;
            
            Assert.AreEqual(columns.Length, addIndexNode.Columns.Count);
            for(int i = 0; i < columns.Length; ++i)
            {
                Assert.AreEqual(columns[i].Name, addIndexNode.Columns[i].Name);
                if(columns[i].SortDirection.HasValue)
                    Assert.AreEqual(columns[i].SortDirection.Value, addIndexNode.Columns[i].SortDirection.Value);
            } // for
        }

        protected static void AssertRemoveTable(IAstNode astNode, string name)
        {
            Assert.IsInstanceOfType(typeof(IRemoveTableNode), astNode);

            IRemoveTableNode removeTableNode = (IRemoveTableNode)astNode;
            Assert.AreEqual(name, removeTableNode.Name);
        }

        protected static void AssertRemoveIndex(IAstNode astNode, string name, string table, params Predicate<IRemoveIndexNode>[] conditions)
        {
            Assert.IsInstanceOfType(typeof(IRemoveIndexNode), astNode);

            IRemoveIndexNode removeIndexNode = (IRemoveIndexNode)astNode;
            Assert.AreEqual(name, removeIndexNode.Name);

            if(!string.IsNullOrEmpty(table))
                Assert.AreEqual(table, removeIndexNode.Table);

            AssertConditions(removeIndexNode, conditions);
        }

        protected static void AssertRemoveReference(IAstNode astNode, string name, string table, params Predicate<IRemoveReferenceNode>[] conditions)
        {
            Assert.IsInstanceOfType(typeof(IRemoveReferenceNode), astNode);

            IRemoveReferenceNode removeReferenceNode = (IRemoveReferenceNode)astNode;
            Assert.AreEqual(name, removeReferenceNode.Name);

            if(!string.IsNullOrEmpty(table))
                Assert.AreEqual(table, removeReferenceNode.Table);

            AssertConditions(removeReferenceNode, conditions);
        }

        private static void AssertConditions<T>(T removeReferenceNode, Predicate<T>[] conditions)
            where T : IAstNode
        {
            if(conditions != null)
                foreach(Predicate<T> assertion in conditions)
                    NUnit.Framework.Assert.IsTrue(assertion(removeReferenceNode), "Assertion {0} violated", Array.IndexOf(conditions, assertion));
        }

        protected static void AssertDowngradeNode(IAstNode astNode)
        {
            Assert.IsInstanceOfType(typeof(IDowngradeNode), astNode);
        }

        protected static void AssertVersionNode(IAstNode astNode, long? number)
        {
            Assert.IsInstanceOfType(typeof(IVersionNode), astNode);

            if(number.HasValue)
                Assert.AreEqual(number.Value, ((IVersionNode)astNode).Number);
        }
    }
}