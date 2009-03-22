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

using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Core.Compiler
{
    public interface IMdlCompiler
    {
        /// <summary>
        /// Adds a given <paramref name="compilerStage"/> to the compiler pipeline.
        /// </summary>
        /// <param name="compilerStage"></param>
        void AddCompilerStage(IMdlCompilerStage compilerStage);

        /// <summary>
        /// Adds a given <paramref name="compilerStage"/> before stage <typeparamref name="T"/> to the compiler pipeline.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compilerStage"></param>
        void AddCompilerStageBefore<T>(IMdlCompilerStage compilerStage)
            where T : IMdlCompilerStage;

        /// <summary>
        /// Adds a given <paramref name="compilerStage"/> after stage <typeparamref name="T"/> to the compiler pipeline.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compilerStage"></param>
        void AddCompilerStageAfter<T>(IMdlCompilerStage compilerStage)
            where T : IMdlCompilerStage;

        /// <summary>
        /// Removes a compiler stage of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemoveCompilerStage<T>()
            where T : IMdlCompilerStage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="astNode"></param>
        /// <param name="compilationOptions"></param>
        IAstNode Compile(IAstNode astNode, MdlCompilationOptions compilationOptions);

        /// <summary>
        /// Performs compilation of the given source from the <paramref name="textReader"/>.
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="compilationOptions"></param>
        IAstNode Compile(TextReader textReader, MdlCompilationOptions compilationOptions);
    }
}