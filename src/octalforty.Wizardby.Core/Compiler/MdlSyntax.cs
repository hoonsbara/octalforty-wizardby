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
namespace octalforty.Wizardby.Core.Compiler
{
    public static class MdlSyntax
    {
        public const string Table = "table";
        public const string Clustered = "clustered";
        public const string Scale = "scale";
        public const string Precision = "precision";
        public const string Migration = "migration";
        public const string Revision = "revision";
        public const string Deployment = "deployment";
        public const string Database = "database";

        public const string Type = "type";
        public const string PrimaryKey = "primary-key";
        public const string PkColumn = "pk-column";
        public const string PkColumns = "pk-columns";
        public const string PkTable = "pk-table";
        public const string FkColumn = "fk-column";
        public const string FkColumns = "fk-columns";
        public const string FkTable = "fk-table";
        public const string Nullable = "nullable";
        public const string Length = "length";

        public const string Environment = "environment";

        public const string Reference = "reference";
        public const string References = "references";
        public const string Unique = "unique";

        public const string Column = "column";
        public const string Columns = "columns";

        public const string Identity = "identity";
        public const string TypeAliases = "type-aliases";
        public const string TypeAlias = "type-alias";

        public const string DefaultPrimaryKey = "default-primary-key";
        public const string Defaults = "defaults";
        public const string Default = "default";
        public const string Baseline = "baseline";
        public const string Version = "version";
        public const string Index = "index";
        public const string Add = "add";
        public const string Remove = "remove";
        public const string Alter = "alter";
        public const string Templates = "templates";
        public const string Template = "template";
        public const string Refactor = "refactor";
        public const string Constraint = "constraint";
    }
}
