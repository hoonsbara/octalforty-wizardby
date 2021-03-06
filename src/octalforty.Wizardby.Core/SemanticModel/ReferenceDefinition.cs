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
using System.Collections.Generic;
using System.Diagnostics;

namespace octalforty.Wizardby.Core.SemanticModel
{
    [DebuggerDisplay("{Name}")]
    public class ReferenceDefinition : SchemaElementDefinitionBase, IReferenceDefinition
    {
        private readonly IList<string> pkColumns = new List<string>();
        private readonly IList<string> fkColumns = new List<string>();
        private string pkTable;
        private string fkTable;

        private string pkTableSchema;

        private string fkTableSchema;

        public IList<string> PkColumns
        {
            get { return pkColumns; }
        }

        public IList<string> FkColumns
        {
            get { return fkColumns; }
        }

        public string PkTable
        {
            get { return pkTable; }
            set { pkTable = value; }
        }

        public string FkTable
        {
            get { return fkTable; }
            set { fkTable = value; }
        }

        public string PkTableSchema
        {
            get { return pkTableSchema; }
            set { pkTableSchema = value; }
        }

        public string FkTableSchema
        {
            get { return fkTableSchema; }
            set { fkTableSchema = value; }
        }

        public ReferenceCascadeAction? OnUpdate { get; set; }

        public ReferenceCascadeAction? OnDelete { get; set; }

        public ReferenceDefinition()
        {
        }

        public ReferenceDefinition(string name) : 
            base(name)
        {
        }

        public ReferenceDefinition(string name, string pkTable, string fkTable) : base(name)
        {
            this.pkTable = pkTable;
            this.fkTable = fkTable;
        }
    }
}