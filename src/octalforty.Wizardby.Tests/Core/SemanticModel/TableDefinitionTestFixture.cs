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
using NUnit.Framework;

using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Tests.Core.SemanticModel
{
    [TestFixture()]
    public class TableDefinitionTestFixture
    {
        [Test()]
        public void AddColumn()
        {
            ITableDefinition table = new TableDefinition("Foo");
            IColumnDefinition column = new ColumnDefinition("ID");

            table.AddColumn(column);

            Assert.AreSame(column, table.Columns[0]);
            Assert.AreEqual(table.Name, column.Table);
        }

        [Test()]
        public void GetColumn()
        {
            TableDefinition table = new TableDefinition("Qux");
            table.AddColumn(new ColumnDefinition("Foo"));
            table.AddColumn(new ColumnDefinition("bAr"));

            Assert.AreSame(table.Columns[0], table.GetColumn("Foo"));
            Assert.AreSame(table.Columns[0], table.GetColumn("foo"));

            Assert.AreSame(table.Columns[1], table.GetColumn("Bar"));
            Assert.AreSame(table.Columns[1], table.GetColumn("BAR"));

            Assert.IsNull(table.GetColumn("ID"));
        }

        [Test()]
        public void RemoveColumn()
        {
            TableDefinition table = new TableDefinition("Qux");
            
            table.AddColumn(new ColumnDefinition("Foo"));
            table.AddColumn(new ColumnDefinition("bAr"));

            table.RemoveColumn("foo");
            Assert.AreEqual(1, table.Columns.Count);

            table.RemoveColumn("baz");
            Assert.AreEqual(1, table.Columns.Count);

            table.RemoveColumn("BAR");
            Assert.AreEqual(0, table.Columns.Count);
        }

        [Test()]
        public void GetPrimaryKeyColumns()
        {
            TableDefinition table = new TableDefinition();
            
            table.AddColumn(new ColumnDefinition("Foo"));
            table.AddColumn(new ColumnDefinition("ID"));
            table.AddColumn(new ColumnDefinition("Bar"));
            
            Assert.IsNull(table.GetPrimaryKeyColumns());

            table.Columns[1].PrimaryKey = true;

            Assert.AreSame(table.Columns[1], table.GetPrimaryKeyColumns()[0]);
        }

        [Test()]
        public void GetCompositePrimaryKeyColumn()
        {
            TableDefinition table = new TableDefinition();

            table.AddColumn(new ColumnDefinition("Foo"));
            table.AddColumn(new ColumnDefinition("ID"));
            table.AddColumn(new ColumnDefinition("Bar"));

            Assert.IsNull(table.GetPrimaryKeyColumns());

            table.Columns[1].PrimaryKey = true;
            table.Columns[2].PrimaryKey = true;

            CollectionAssert.AreEqual(new[] { table.Columns[1], table.Columns[2] }, table.GetPrimaryKeyColumns());
        }

        [Test()]
        public void AddIndex()
        {
            TableDefinition table = new TableDefinition("Foo");
            table.AddIndex(new IndexDefinition("IX_Bar"));

            Assert.AreEqual(table.Name, table.Indexes[0].Table);
        }

        [Test()]
        public void GetIndex()
        {
            TableDefinition table = new TableDefinition("Foo");
            IndexDefinition index = new IndexDefinition("IX_Bar");

            table.AddIndex(index);

            Assert.AreSame(index, table.GetIndex("ix_bar"));
            Assert.AreSame(index, table.GetIndex("ix_bAR"));
            Assert.IsNull(table.GetIndex("XX_Bar"));
        }

        [Test()]
        public void RemoveIndex()
        {
            TableDefinition table = new TableDefinition("Foo");
            IndexDefinition index = new IndexDefinition("IX_Bar");

            table.AddIndex(index);

            table.RemoveIndex("XX_Bar");
            Assert.AreEqual(1, table.Indexes.Count);

            table.RemoveIndex("ix_Bar");
            Assert.AreEqual(0, table.Indexes.Count);
        }

        [Test()]
        public void AddReference()
        {
            TableDefinition table = new TableDefinition("Foo");
            table.AddReference(new ReferenceDefinition("FK_Bar"));

            Assert.AreEqual(table.Name, table.References[0].FkTable);
        }

        [Test()]
        public void GetReference()
        {
            TableDefinition table = new TableDefinition("Foo");
            ReferenceDefinition reference = new ReferenceDefinition("IX_Bar");

            table.AddReference(reference);

            Assert.AreSame(reference, table.GetReference("ix_bar"));
            Assert.AreSame(reference, table.GetReference("ix_bAR"));
            Assert.IsNull(table.GetReference("XX_Bar"));
        }

        [Test()]
        public void RemoveReference()
        {
            TableDefinition table = new TableDefinition("Foo");
            ReferenceDefinition reference = new ReferenceDefinition("IX_Bar");

            table.AddReference(reference);

            table.RemoveReference("XX_Bar");
            Assert.AreEqual(1, table.References.Count);

            table.RemoveReference("ix_Bar");
            Assert.AreEqual(0, table.References.Count);
        }

        [Test()]
        public void AddConstraint()
        {
            TableDefinition table = new TableDefinition("Foo");
            table.AddConstraint(new DefaultConstraintDefinition("DF_Bar"));

            Assert.AreEqual(table.Name, table.Constraints[0].Table);
        }

        [Test()]
        public void GetConstraint()
        {
            TableDefinition table = new TableDefinition("Foo");
            IConstraintDefinition constraint = new DefaultConstraintDefinition("DF_Bar");

            table.AddConstraint(constraint);

            Assert.AreSame(constraint, table.GetConstraint("df_bar"));
            Assert.AreSame(constraint, table.GetConstraint("Df_BaR"));
            Assert.IsNull(table.GetConstraint("XX_Bar"));
        }

        [Test()]
        public void RemoveConstraint()
        {
            TableDefinition table = new TableDefinition("Foo");
            IConstraintDefinition constraint = new DefaultConstraintDefinition("DF_Bar");

            table.AddConstraint(constraint);

            table.RemoveConstraint("XX_Bar");
            Assert.AreEqual(1, table.Constraints.Count);

            table.RemoveConstraint("df_Bar");
            Assert.AreEqual(0, table.Constraints.Count);
        }
    }
}
