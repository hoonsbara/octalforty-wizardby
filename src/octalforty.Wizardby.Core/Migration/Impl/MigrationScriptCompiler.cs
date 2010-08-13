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
using System.IO;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Impl;
using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    public class MigrationScriptCompiler
    {
        private readonly IDbPlatform dbPlatform;
        private readonly MigrationMode migrationMode;
        private readonly INativeSqlResourceProvider nativeSqlResourceProvider;

        public MigrationScriptCompiler(IDbPlatform dbPlatform, INativeSqlResourceProvider nativeSqlResourceProvider, MigrationMode migrationMode)
        {
            this.dbPlatform = dbPlatform;
            this.nativeSqlResourceProvider = nativeSqlResourceProvider;
            this.migrationMode = migrationMode;
        }

        public MigrationScriptCollection CompileMigrationScripts(TextReader migrationDefinition)
        {
            Environment environment = new Environment();

            MigrationScriptsCodeGenerator migrationScriptsCodeGenerator =
                new MigrationScriptsCodeGenerator(dbPlatform, nativeSqlResourceProvider, migrationMode);
            //migrationScriptsCodeGenerator.SetEnvironment(environment);

            IMdlCompiler mdlCompiler = new MdlCompiler(migrationScriptsCodeGenerator, environment);
            mdlCompiler.AddCompilerStageAfter<AstFlattenerCompilerStage>(new DbNamingCompilerStage(dbPlatform.NamingStrategy));
            mdlCompiler.AddCompilerStageBefore<AstFlattenerCompilerStage>(new RefactoringStage(dbPlatform));

            mdlCompiler.Compile(migrationDefinition, MdlCompilationOptions.All);

            return migrationScriptsCodeGenerator.MigrationScripts;
        }
    }
}
