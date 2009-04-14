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

using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Tests.Core.Compiler.Ast
{
    [TestFixture()]
    public class AstUtilTestFixture
    {
        [Test()]
        public void CloneAddColumnNode()
        {
            IBaselineNode baselineNode = new BaselineNode(null);
            IAddColumnNode originalNode = new AddColumnNode(baselineNode, "Add Column");
            originalNode.Default = "Foo";
            originalNode.Identity = true;
            originalNode.Length = 321;
            originalNode.Nullable = true;
            originalNode.Precision = 32;
            originalNode.PrimaryKey = true;
            originalNode.Scale = 55;
            originalNode.Table = "Qux";
            originalNode.Type = DbType.Boolean;
            
            IAddColumnNode clonedNode = AstUtil.Clone(originalNode);

            Assert.AreSame(originalNode.Parent, clonedNode.Parent);
            Assert.AreEqual(originalNode.Name, clonedNode.Name);
            Assert.AreEqual(originalNode.Default, clonedNode.Default);
            Assert.AreEqual(originalNode.Identity, clonedNode.Identity);
            Assert.AreEqual(originalNode.Length, clonedNode.Length);
            Assert.AreEqual(originalNode.Nullable, clonedNode.Nullable);
            Assert.AreEqual(originalNode.Precision, clonedNode.Precision);
            Assert.AreEqual(originalNode.PrimaryKey, clonedNode.PrimaryKey);
            Assert.AreEqual(originalNode.Scale, clonedNode.Scale);
            Assert.AreEqual(originalNode.Type, clonedNode.Type);
            Assert.AreEqual(originalNode.Table, clonedNode.Table);
        }
    }
}
