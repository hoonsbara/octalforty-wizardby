namespace octalforty.Wizardby.Core.SemanticModel
{
    public class SemanticModelUtil
    {
        public static void Copy(IColumnDefinition sourceColumn, IColumnDefinition targetColumn)
        {
            targetColumn.Default = sourceColumn.Default;
            targetColumn.Identity = sourceColumn.Identity;
            targetColumn.Length = sourceColumn.Length;
            targetColumn.Name = sourceColumn.Name;
            targetColumn.Nullable = sourceColumn.Nullable;
            targetColumn.Precision = sourceColumn.Precision;
            targetColumn.PrimaryKey = sourceColumn.PrimaryKey;
            targetColumn.Scale = sourceColumn.Scale;
            targetColumn.Type = sourceColumn.Type;
            targetColumn.Table = sourceColumn.Table;
        }

        public static void Copy(IIndexDefinition sourceIndex, IIndexDefinition targetIndex)
        {
            targetIndex.Clustered = sourceIndex.Clustered;
            targetIndex.Name = sourceIndex.Name;
            targetIndex.Table = sourceIndex.Table;
            targetIndex.Unique = sourceIndex.Unique;

            foreach (IIndexColumnDefinition indexColumn in sourceIndex.Columns)
            {
                targetIndex.Columns.Add(Clone(indexColumn));
            } // foreach
        }

        public static IIndexColumnDefinition Clone(IIndexColumnDefinition indexColumn)
        {
            return new IndexColumnDefinition(indexColumn.Name, indexColumn.SortDirection);
        }
    }
}
