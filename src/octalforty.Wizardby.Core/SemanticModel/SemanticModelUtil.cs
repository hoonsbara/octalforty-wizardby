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
