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
using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    public class DbNamingCompilerStage : MdlCompilerStageBase
    {
        private readonly IDbNamingStrategy namingStrategy;

        public DbNamingCompilerStage(IDbNamingStrategy namingStrategy)
        {
            this.namingStrategy = namingStrategy;
        }

        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            if(!Environment.IsAnonymousIdentifier(addIndexNode.Name))
                return;

            ITableDefinition table = Environment.Schema.GetTable(addIndexNode.Table);
            IIndexDefinition index = table.GetIndex(addIndexNode.Name);

            //
            // Remove index...
            table.RemoveIndex(addIndexNode.Name);

            //
            // Rename 
            addIndexNode.Name = index.Name = namingStrategy.GetIndexName(addIndexNode);

            //
            // And readd to the table
            table.AddIndex(index);
        }

        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            if(!Environment.IsAnonymousIdentifier(addReferenceNode.Name))
                return;

            ITableDefinition table = Environment.Schema.GetTable(addReferenceNode.FkTable);
            IReferenceDefinition reference = table.GetReference(addReferenceNode.Name);

            //
            // Remove reference
            table.RemoveReference(addReferenceNode.Name);

            //
            // Rename
            addReferenceNode.Name = reference.Name = namingStrategy.GetReferenceName(reference);

            //
            // And readd
            table.AddReference(reference);
        }
    }
}
