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

namespace octalforty.Wizardby.Core.SemanticModel
{
    public class SemanticModelUtil
    {
        /// <summary>
        /// Copies all properties from <paramref name="sourceColumn"/> to <paramref name="targetColumn"/>.
        /// </summary>
        /// <param name="sourceColumn"></param>
        /// <param name="targetColumn"></param>
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

        /// <summary>
        /// Copies all properties from <paramref name="sourceIndex"/> to <paramref name="targetIndex"/>.
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
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

        /// <summary>
        /// Copies all properties from <paramref name="sourceReference"/> to <paramref name="targetReference"/>.
        /// </summary>
        /// <param name="sourceReference"></param>
        /// <param name="targetReference"></param>
        public static void Copy(IReferenceDefinition sourceReference, IReferenceDefinition targetReference)
        {
            targetReference.FkTable = sourceReference.FkTable;
            targetReference.FkTableSchema = sourceReference.FkTableSchema;
            targetReference.Name = sourceReference.Name;
            targetReference.PkTable = sourceReference.PkTable;
            targetReference.PkTableSchema = sourceReference.PkTableSchema;

            Copy(sourceReference.FkColumns, targetReference.FkColumns);
            Copy(sourceReference.PkColumns, targetReference.PkColumns);
        }

        /// <summary>
        /// Clones the given <paramref name="indexColumn"/>.
        /// </summary>
        /// <param name="indexColumn"></param>
        /// <returns></returns>
        public static IIndexColumnDefinition Clone(IIndexColumnDefinition indexColumn)
        {
            return new IndexColumnDefinition(indexColumn.Name, indexColumn.SortDirection);
        }

        private static void Copy<T>(ICollection<T> source, ICollection<T> target)
        {
            foreach(T item in source)
                target.Add(item);
        }
    }
}
