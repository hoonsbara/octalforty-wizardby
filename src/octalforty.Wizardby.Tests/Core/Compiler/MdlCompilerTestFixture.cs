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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Tests.Core.Compiler
{
    [TestFixture()]
    public class MdlCompilerTestFixture
    {
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddCompilerStageThrowsArgumentNullExceptionOnNullCompilerStage()
        {
            IMdlCompiler mdlCompiler = new MdlCompiler(new NullCodeGenerator(), new Wizardby.Core.Compiler.Environment());
            mdlCompiler.AddCompilerStage(null);
        }

        [Test()]
        public void CompileWaffle()
        {
            IAstNode astNode;
            Wizardby.Core.Compiler.Environment environment = new Wizardby.Core.Compiler.Environment();

            using(Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Waffle.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                astNode = mdlParser.Parse();

                IMdlCompiler mdlCompiler = new MdlCompiler(new NullCodeGenerator(), environment);
                mdlCompiler.Compile(astNode, MdlCompilationOptions.All);
            } // using
        }

        [Test()]
        public void CompileBlog()
        {
            IAstNode astNode;
            Wizardby.Core.Compiler.Environment environment = new Wizardby.Core.Compiler.Environment();

            using (Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("octalforty.Wizardby.Tests.Resources.Blog.mdl"))
            {
                IMdlParser mdlParser = new MdlParser(MdlParserTestFixture.CreateScanner(new StreamReader(resourceStream, Encoding.UTF8)));
                astNode = mdlParser.Parse();

                IMdlCompiler mdlCompiler = new MdlCompiler(new NullCodeGenerator(), environment);
                mdlCompiler.Compile(astNode, MdlCompilationOptions.All);
            } // using
        }

        private IEnumerable<T> Filter<T>(IEnumerable enumerable)
        {
            foreach(object o in enumerable)
                if(o is T)
                    yield return (T)o;
        }
    }
}
