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
using System.Diagnostics;
using System.IO;
using System.Text;

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Impl;

namespace octalforty.Wizardby.Core.Compiler
{
    /// <summary>
    /// Compiles MDL AST and generates code using supplied code generator.
    /// </summary>
    public class MdlCompiler : IMdlCompiler
    {
        #region Private Fields
        private readonly ICodeGenerator codeGenerator;
        private readonly Environment environment;
        private readonly List<IMdlCompilerStage> compilerStages = new List<IMdlCompilerStage>();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MdlCompiler"/> class.
        /// </summary>
        /// <param name="codeGenerator"></param>
        /// <param name="environment"></param>
        public MdlCompiler(ICodeGenerator codeGenerator, Environment environment)
        {
            this.codeGenerator = codeGenerator;
            this.environment = environment;
            
            AddCompilerStage(new TemplateSubstitutionCompilerStage());

            //
            // First, we need to resolve type aliases for other stages to be able to use standard DbType
            // members.
            AddCompilerStage(new TypeAliasResolutionCompilerStage());

            //
            // Next we need to resolve primary keys. This will just add a new node to IAddTableNode,
            // all binding creation will be performed later.
            AddCompilerStage(new PrimaryKeyResolutionCompilerStage());

            AddCompilerStage(new VersionOrdererCompilerStage());
            AddCompilerStage(new SchemaInfoBuilderCompilerStage());

            //
            // Next we have to resolve shortcuts
            AddCompilerStage(new ShortcutResolutionCompilerStage());

            AddCompilerStage(new NamingCompilerStage());
            AddCompilerStage(new BindingCompilerStage());
            AddCompilerStage(new TypeInferenceCompilerStage());

            //
            // Flatten the whole thing
            AddCompilerStage(new AstFlattenerCompilerStage());

            //
            // Generate "upgrade" and "downgrade" nodes
            AddCompilerStage(new UpgradeGenerationStage());
            AddCompilerStage(new DowngradeGenerationStage());
        }

        /// <summary>
        /// Adds a given <paramref name="compilerStage"/> to the compiler pipeline.
        /// </summary>
        /// <param name="compilerStage"></param>
        public void AddCompilerStage(IMdlCompilerStage compilerStage)
        {
            if(compilerStage == null) 
                throw new ArgumentNullException("compilerStage");

            InternalAddCompilerStage(compilerStage);
        }

        /// <summary>
        /// Adds a given <paramref name="compilerStage"/> before stage <typeparamref name="T"/> to the compiler pipeline.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compilerStage"></param>
        public void AddCompilerStageBefore<T>(IMdlCompilerStage compilerStage) 
            where T : IMdlCompilerStage
        {
            if(compilerStage == null) 
                throw new ArgumentNullException("compilerStage");

            InternalAddCompilerStageBefore<T>(compilerStage);
        }

        /// <summary>
        /// Adds a given <paramref name="compilerStage"/> after stage <typeparamref name="T"/> to the compiler pipeline.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compilerStage"></param>
        public void AddCompilerStageAfter<T>(IMdlCompilerStage compilerStage) 
            where T : IMdlCompilerStage
        {
            if(compilerStage == null) 
                throw new ArgumentNullException("compilerStage");

            InternalAddCompilerStageAfter<T>(compilerStage);
        }

        /// <summary>
        /// Removes a compiler stage of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveCompilerStage<T>() 
            where T : IMdlCompilerStage
        {
            IMdlCompilerStage compilerStage = 
                compilerStages.Find(delegate(IMdlCompilerStage mcs)
                    { return mcs is T; });
            
            if(compilerStage != null)
                compilerStages.Remove(compilerStage);
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="astNode"></param>
        /// <param name="compilationOptions"></param>
        public IAstNode Compile(IAstNode astNode, MdlCompilationOptions compilationOptions)
        {
            //
            // Passing through the pipeline
            foreach(IMdlCompilerStage compilerStage in compilerStages)
            {
                compilerStage.SetEnvironment(environment);
                astNode.Accept(compilerStage);
            } // foreach

            if((compilationOptions & MdlCompilationOptions.GenerateCode) == MdlCompilationOptions.GenerateCode)
            {
                codeGenerator.SetEnvironment(environment);
                astNode.Accept(codeGenerator);
            } // if

            MdlGenerator mdlGenerator = new MdlGenerator();

            StringBuilder mdlBuilder = new StringBuilder();
            mdlGenerator.Generate(astNode, new StringWriter(mdlBuilder));

            //Console.WriteLine(mdlBuilder.ToString());

            return astNode;
        }

        /// <summary>
        /// Performs compilation of the given source from the <paramref name="textReader"/>.
        /// </summary>
        /// <param name="textReader"></param>
        public IAstNode Compile(TextReader textReader, MdlCompilationOptions compilationOptions)
        {
            IMdlScanner mdlScanner = new MdlScanner(new SourceReader(textReader));

            mdlScanner.RegisterKeyword("migration");
            mdlScanner.RegisterKeyword("deployment");
            mdlScanner.RegisterKeyword("type-aliases");
            mdlScanner.RegisterKeyword("type-alias");
            mdlScanner.RegisterKeyword("database");
            mdlScanner.RegisterKeyword(MdlSyntax.Environment);
            mdlScanner.RegisterKeyword("defaults");
            mdlScanner.RegisterKeyword("default-primary-key");
            mdlScanner.RegisterKeyword("baseline");
            mdlScanner.RegisterKeyword("version");
            mdlScanner.RegisterKeyword("add");
            mdlScanner.RegisterKeyword("remove");
            mdlScanner.RegisterKeyword("table");
            mdlScanner.RegisterKeyword("column");
            mdlScanner.RegisterKeyword("index");
            mdlScanner.RegisterKeyword("reference");
            mdlScanner.RegisterKeyword("alter");
            mdlScanner.RegisterKeyword("templates");
            mdlScanner.RegisterKeyword("template");
            mdlScanner.RegisterKeyword("refactor");

            IMdlParser mdlParser = new MdlParser(mdlScanner);
            IAstNode astNode = mdlParser.Parse();

            return Compile(astNode, compilationOptions);
        }

        private void InternalAddCompilerStage(IMdlCompilerStage compilerStage)
        {
            compilerStages.Add(compilerStage);
        }

        private void InternalAddCompilerStageBefore<T>(IMdlCompilerStage compilerStage)
            where T : IMdlCompilerStage
        {
            InsertRelativeTo<T>(delegate(int i) { compilerStages.Insert(i, compilerStage); });
        }

        private void InternalAddCompilerStageAfter<T>(IMdlCompilerStage compilerStage)
            where T : IMdlCompilerStage
        {
            InsertRelativeTo<T>(delegate(int i)
                {
                    if(i == compilerStages.Count - 1)
                        compilerStages.Add(compilerStage);
                    else
                        compilerStages.Insert(i + 1, compilerStage);
                });
        }

        public void InsertRelativeTo<T>(Action<int> action)
            where T : IMdlCompilerStage
        {
            for(int i = 0; i < compilerStages.Count; ++i)
                if(typeof(T).Equals(compilerStages[i].GetType()))
                {
                    action(i);
                    return;
                } // if
        }
    }
}
