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
using System.Data;

using NUnit.Framework;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Impl;
using octalforty.Wizardby.Core.SemanticModel;

namespace octalforty.Wizardby.Tests.Core.Compiler.Impl
{
    [TestFixture()]
    public class TemplateSubstitutionCompilerStageTestFixture : AstTestFixtureBase
    {
        [Test()]
        public void SubstituteTableTemplates()
        {
            IMdlCompilerStage templateSubstitutionStage = new TemplateSubstitutionCompilerStage();
            templateSubstitutionStage.SetEnvironment(new Environment());

            IAstNode astNode = new MdlParser(MdlParserTestFixture.CreateScanner(
@"migration ""Waffle"" revision => 1:
    templates:
        table template T:
            ID type => Int32, nullable => false, ignore-primary-key => true, identity => true

        table template U:
            Foo type => String, length => 200
            Abc type => String

    version 1:
        add table Foo:
            include-template T
            include-template U
            Baz type => Int32

        add table Bar templates = [T, U]")).Parse();

            astNode.Accept(templateSubstitutionStage);

            IVersionNode version1Node = (IVersionNode)astNode.ChildNodes[1];
            
            AssertAddTable(version1Node.ChildNodes[0], "Foo",
                new ColumnDefinition("ID"),
                new ColumnDefinition("Foo"),
                new ColumnDefinition("Abc"),
                new ColumnDefinition("Baz"));

            AssertAddTable(version1Node.ChildNodes[1], "Bar",
                new ColumnDefinition("ID"),
                new ColumnDefinition("Foo"),
                new ColumnDefinition("Abc"));
        }
    }
}
