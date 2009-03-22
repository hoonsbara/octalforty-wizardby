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
using System.CodeDom.Compiler;
using System.IO;

using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    internal class MdlGeneratorAstVisitor : AstVisitorBase
    {
        #region Private Fields
        private readonly IndentedTextWriter textWriter;
        #endregion

        public MdlGeneratorAstVisitor(TextWriter textWriter)
        {
            this.textWriter = new IndentedTextWriter(textWriter, "    ");
        }

        #region AstVisitorBase Members
        /// <summary>
        ///  Visits the given <paramref name="upgradeNode" />.
        /// </summary>
        /// <param name="upgradeNode"></param>
        public override void Visit(IUpgradeNode upgradeNode)
        {
            textWriter.Write("upgrade");
            
            WriteProperties(upgradeNode);
            VisitBlock(upgradeNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="downgradeNode" />.
        /// </summary>
        /// <param name="downgradeNode"></param>
        public override void Visit(IDowngradeNode downgradeNode)
        {
            textWriter.Write("downgrade");

            WriteProperties(downgradeNode);
            VisitBlock(downgradeNode);
        }

        /// <summary>
        /// Visits the given <paramref name="migrationNode"/>.
        /// </summary>
        /// <param name="migrationNode"></param>
        public override void Visit(IMigrationNode migrationNode)
        {
            textWriter.Write("migration {0}", migrationNode.Name);

            WriteProperties(migrationNode);
            VisitBlock(migrationNode);
        }

        /// <summary>
        /// Visits the given <paramref name="baselineNode"/>.
        /// </summary>
        /// <param name="baselineNode"></param>
        public override void Visit(IBaselineNode baselineNode)
        {
            textWriter.Write("baseline");

            WriteProperties(baselineNode);
            VisitBlock(baselineNode);
        }

        /// <summary>
        /// Visits the given <paramref name="versionNode"/>.
        /// </summary>
        /// <param name="versionNode"></param>
        public override void Visit(IVersionNode versionNode)
        {
            textWriter.Write("version {0}", versionNode.Number);

            WriteProperties(versionNode);
            VisitBlock(versionNode);
        }

        public override void Visit(IAddTableNode addTableNode)
        {
            textWriter.Write("add table {0}", addTableNode.Name);
            
            WriteProperties(addTableNode);
            VisitBlock(addTableNode);
        }

        public override void Visit(IAddColumnNode addColumnNode)
        {
            textWriter.Write("add column {0}", addColumnNode.Name);

            WriteProperties(addColumnNode);
            VisitBlock(addColumnNode);
        }

        public override void Visit(IRemoveTableNode removeTableNode)
        {
            textWriter.Write("remove table {0}", removeTableNode.Name);

            WriteProperties(removeTableNode);
            VisitBlock(removeTableNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="addIndexNode" />.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            textWriter.Write("add index {0}", addIndexNode.Name);

            WriteProperties(addIndexNode);
            VisitBlock(addIndexNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="addReferenceNode" />.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            textWriter.Write("add reference {0}", addReferenceNode.Name);

            WriteProperties(addReferenceNode);
            VisitBlock(addReferenceNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="defaultsNode" />.
        /// </summary>
        /// <param name="defaultsNode"></param>
        public override void Visit(IDefaultsNode defaultsNode)
        {
            textWriter.Write("defaults");

            WriteProperties(defaultsNode);
            VisitBlock(defaultsNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="deploymentNode" />.
        /// </summary>
        /// <param name="deploymentNode"></param>
        public override void Visit(IDeploymentNode deploymentNode)
        {
            base.Visit(deploymentNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="defaultPrimaryKeyNode" />.
        /// </summary>
        /// <param name="defaultPrimaryKeyNode"></param>
        public override void Visit(IDefaultPrimaryKeyNode defaultPrimaryKeyNode)
        {
            textWriter.Write("default-primary-key {0}", defaultPrimaryKeyNode.Name);

            WriteProperties(defaultPrimaryKeyNode);
            VisitBlock(defaultPrimaryKeyNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="typeAliasesNode" />.
        /// </summary>
        /// <param name="typeAliasesNode"></param>
        public override void Visit(ITypeAliasesNode typeAliasesNode)
        {
            textWriter.Write("type-aliases");

            WriteProperties(typeAliasesNode);
            VisitBlock(typeAliasesNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="typeAliasNode" />.
        /// </summary>
        /// <param name="typeAliasNode"></param>
        public override void Visit(ITypeAliasNode typeAliasNode)
        {
            textWriter.Write("type-alias {0}", typeAliasNode.Name);

            WriteProperties(typeAliasNode);
            VisitBlock(typeAliasNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="removeReferenceNode" />.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
            textWriter.Write("remove reference {0}", removeReferenceNode.Name);

            WriteProperties(removeReferenceNode);
            VisitBlock(removeReferenceNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="removeIndexNode" />.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            textWriter.Write("remove index {0}", removeIndexNode.Name);

            WriteProperties(removeIndexNode);
            VisitBlock(removeIndexNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="alterTableNode" />.
        /// </summary>
        /// <param name="alterTableNode"></param>
        public override void Visit(IAlterTableNode alterTableNode)
        {
            textWriter.Write("alter table {0}", alterTableNode.Name);

            WriteProperties(alterTableNode);
            VisitBlock(alterTableNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="alterColumnNode" />.
        /// </summary>
        /// <param name="alterColumnNode"></param>
        public override void Visit(IAlterColumnNode alterColumnNode)
        {
            textWriter.Write("alter column {0}", alterColumnNode.Name);

            WriteProperties(alterColumnNode);
            VisitBlock(alterColumnNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="environmentNode" />.
        /// </summary>
        /// <param name="environmentNode"></param>
        public override void Visit(IEnvironmentNode environmentNode)
        {
            base.Visit(environmentNode);
        }

        /// <summary>
        ///  Visits the given <paramref name="removeColumnNode" />.
        /// </summary>
        /// <param name="removeColumnNode"></param>
        public override void Visit(IRemoveColumnNode removeColumnNode)
        {
            textWriter.Write("remove column {0}", removeColumnNode.Name);

            WriteProperties(removeColumnNode);
            VisitBlock(removeColumnNode);
        }
        #endregion

        private void WriteProperties(IAstNode astNode)
        {
            if(astNode.Properties.Count == 0)
                return;

            bool firstProperty = true;
            foreach(IAstNodeProperty property in astNode.Properties)
            {
                if (!firstProperty)
                    textWriter.Write(",");
                else
                    firstProperty = false;

                textWriter.Write(" {0} => {1}", property.Name, 
                    property.Value is string ? 
                        "\"" + property.Value + "\"" :
                        property.Value);
            } // foreach
        }

        private void VisitBlock(IAstNode astNode)
        {
            if(astNode.ChildNodes.Count == 0)
            {
                textWriter.WriteLine();
                return;
            } // if

            textWriter.WriteLine(":");
            VisitChildNodes(astNode);
        }

        private void VisitChildNodes(IAstNode astNode)
        {
            textWriter.Indent += 1;
            Visit(astNode.ChildNodes);
            textWriter.Indent -= 1;
        }
    }
}
