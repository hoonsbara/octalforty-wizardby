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
