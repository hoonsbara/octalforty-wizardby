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
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Core.Db
{
    /// <summary>
    /// Represents abase class for <see cref="IDbScriptGenerator"/> implementors.
    /// </summary>
    public class DbScriptGeneratorBase : CodeGeneratorBase, IDbScriptGenerator
    {
        #region Private Fields
        private readonly IDbStatementBatchWriter statementBatchWriter;
        private INativeSqlResourceProvider nativeSqlResourceProvider;
        private MigrationMode migrationMode;
        #endregion

        #region Protected Properties
        protected IndentedTextWriter TextWriter
        {
            [DebuggerStepThrough]
            get { return statementBatchWriter.BatchWriter; }
        }

        protected IDbStatementBatchWriter StatementBatchWriter
        {
            [DebuggerStepThrough]
            get { return statementBatchWriter; }
        }

        protected MigrationMode MigrationMode
        {
            [DebuggerStepThrough]
            get { return migrationMode; }
        }

        protected INativeSqlResourceProvider NativeSqlResourceProvider
        {
            [DebuggerStepThrough]
            get { return nativeSqlResourceProvider; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DbScriptGeneratorBase"/> class.
        /// </summary>
        /// <param name="statementBatchWriter"></param>
        public DbScriptGeneratorBase(IDbStatementBatchWriter statementBatchWriter)
        {
            this.statementBatchWriter = statementBatchWriter;
        }

        public override void Visit(IVersionNode versionNode)
        {
            Visit(versionNode.ChildNodes);
        }

        protected string MapToNativeType(IColumnDefinition columnDefinition)
        {
            if(!columnDefinition.Type.HasValue)
                throw new DbPlatformException(string.Format("Could not resolve type in {0}", GetFragment(((IAstNode)columnDefinition).Parent)));

            if(!columnDefinition.Length.HasValue && !columnDefinition.Scale.HasValue && !columnDefinition.Precision.HasValue)
                return Platform.TypeMapper.MapToNativeType(columnDefinition.Type.Value, columnDefinition.Length);
            
            return columnDefinition.Length.HasValue ? 
                Platform.TypeMapper.MapToNativeType(columnDefinition.Type.Value, columnDefinition.Length) : 
                Platform.TypeMapper.MapToNativeType(columnDefinition.Type.Value, columnDefinition.Scale, columnDefinition.Precision);
        }

        #region IDbPlatformDependency Members
        public virtual IDbPlatform Platform 
        { get; set; }
        #endregion

        #region IDbScriptGenerator Members
        public virtual void SetMigrationMode(MigrationMode migrationMode)
        {
            this.migrationMode = migrationMode;
        }

        public void SetNativeSqlResourceProvider(INativeSqlResourceProvider nativeSqlResourceProvider)
        {
            this.nativeSqlResourceProvider = nativeSqlResourceProvider;
        }
        #endregion

        string GetFragment(IAstNode astNode)
        {
            StringBuilder sb = new StringBuilder();
            using(StringWriter stringWriter = new StringWriter(sb))
            {
                MdlGenerator mdlGenerator = new MdlGenerator();   
                mdlGenerator.Generate(astNode, stringWriter);
            }

            return sb.ToString();
        }
    }
}
