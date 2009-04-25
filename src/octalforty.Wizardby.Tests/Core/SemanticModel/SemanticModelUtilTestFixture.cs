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
    public class SemanticModelUtilTestFixture
    {
        [Test()]
        public void CopyIndexDefinitions()
        {
            IIndexDefinition sourceIndex = new IndexDefinition("IX_Foo",
                new IndexColumnDefinition("Foo", SortDirection.Ascending),
                new IndexColumnDefinition("ID", SortDirection.Descending));
            sourceIndex.Clustered = true;
            sourceIndex.Unique = true;

            IIndexDefinition targetIndex = new IndexDefinition();

            SemanticModelUtil.Copy(sourceIndex, targetIndex);

            Assert.AreEqual(sourceIndex.Clustered, targetIndex.Clustered);
            Assert.AreEqual(sourceIndex.Name, targetIndex.Name);
            Assert.AreEqual(sourceIndex.Table, targetIndex.Table);
            Assert.AreEqual(sourceIndex.Unique, targetIndex.Unique);

            for(int i = 0; i < sourceIndex.Columns.Count; ++i)
            {
                Assert.AreEqual(sourceIndex.Columns[i].Name, targetIndex.Columns[i].Name);
                Assert.AreEqual(sourceIndex.Columns[i].SortDirection, targetIndex.Columns[i].SortDirection);
            } // for
        }
        
        [Test()]
        public void CopyReferenceDefinitions()
        {
            IReferenceDefinition sourceReference = new ReferenceDefinition("FK_Foo",
                "Foo", "Bar");
            sourceReference.FkTableSchema = "dbo";
            sourceReference.FkColumns.Add("ID");
            sourceReference.FkColumns.Add("IDx");

            sourceReference.FkTableSchema = "db";
            sourceReference.FkColumns.Add("FooID");
            sourceReference.FkColumns.Add("FooIDx");

            IReferenceDefinition targetReference = new ReferenceDefinition();

            SemanticModelUtil.Copy(sourceReference, targetReference);

            Assert.AreEqual(sourceReference.FkTable, targetReference.FkTable);
            Assert.AreEqual(sourceReference.FkTableSchema, targetReference.FkTableSchema);
            Assert.AreEqual(sourceReference.Name, targetReference.Name);
            Assert.AreEqual(sourceReference.PkTable, targetReference.PkTable);
            Assert.AreEqual(sourceReference.PkTableSchema, targetReference.PkTableSchema);

            CollectionAssert.AreEqual(sourceReference.PkColumns, targetReference.PkColumns);
            CollectionAssert.AreEqual(sourceReference.FkColumns, targetReference.FkColumns);
        }
    }
}
