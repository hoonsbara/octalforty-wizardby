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
    }
}
