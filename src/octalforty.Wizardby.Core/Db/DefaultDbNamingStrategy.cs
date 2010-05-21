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
using octalforty.Wizardby.Core.SemanticModel;
using octalforty.Wizardby.Core.Util;

namespace octalforty.Wizardby.Core.Db
{
    public class DefaultDbNamingStrategy : DbPlatformDependencyBase, IDbNamingStrategy
    {
        public virtual string GetReferenceName(IReferenceDefinition reference)
        {
            string pkColumnNames = Algorithms.Join("", reference.PkColumns,
                                                   delegate(string s) { return s; });
            string fkColumnNames = Algorithms.Join("", reference.FkColumns,
                                                   delegate(string s) { return s; });

            return string.Format("FK_{0}_{1}_{2}_{3}",
                                 GetBareIdentifier(reference.FkTable), fkColumnNames,
                                 GetBareIdentifier(reference.PkTable), pkColumnNames);
        }

        public virtual string GetIndexName(IIndexDefinition index)
        {
            string columnNames = Algorithms.Join("", index.Columns,
                delegate(IIndexColumnDefinition icd) { return icd.Name; });

            return GetIndexNameForColumns(index, columnNames);
        }

        protected virtual string GetIndexNameForColumns(IIndexDefinition index, string columnNames)
        {
            return index.Unique ?? false ?
                string.Format("UQ_{0}", columnNames) :
                string.Format("IX_{0}", columnNames);
        }

        public virtual string GetConstraintName(IConstraintDefinition constraint)
        {
            if (constraint is IDefaultConstraintDefinition)
                return string.Format("DF_{0}", constraint.Columns[0]);

            return constraint.Name;
        }

        private static string GetBareIdentifier(string identifier)
        {
            if (identifier.Contains("."))
                return identifier.Substring(identifier.LastIndexOf('.') + 1);

            return identifier;
        }
    }
}
