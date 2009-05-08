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

using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Core.Compiler
{
    /// <summary>
    /// Provides a base class for MDL compiler stages.
    /// </summary>
    public abstract class MdlCompilerStageBase : AstVisitorBase, IMdlCompilerStage
    {
        #region Private Fields
        private Environment environment;
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets a reference to the <see cref="Compiler.Environment"/> object for the current
        /// compilation session.
        /// </summary>
        protected Environment Environment
        {
            get { return environment; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MdlCompilerStageBase"/> class.
        /// </summary>
        protected MdlCompilerStageBase()
        {
        }

        #region AstVisitorBase Members
        /// <summary>
        /// Visits the given <paramref name="migrationNode"/>.
        /// </summary>
        /// <param name="migrationNode"></param>
        public override void Visit(IMigrationNode migrationNode)
        {
            Visit(migrationNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="baselineNode"/>.
        /// </summary>
        /// <param name="baselineNode"></param>
        public override void Visit(IBaselineNode baselineNode)
        {
            Visit(baselineNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="versionNode"/>.
        /// </summary>
        /// <param name="versionNode"></param>
        public override void Visit(IVersionNode versionNode)
        {
            Visit(versionNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="addTableNode"/>.
        /// </summary>
        /// <param name="addTableNode"></param>
        public override void Visit(IAddTableNode addTableNode)
        {
            Visit(addTableNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="addColumnNode"/>.
        /// </summary>
        /// <param name="addColumnNode"></param>
        public override void Visit(IAddColumnNode addColumnNode)
        {
            Visit(addColumnNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            Visit(addIndexNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            Visit(addReferenceNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="deploymentNode"/>.
        /// </summary>
        /// <param name="deploymentNode"></param>
        public override void Visit(IDeploymentNode deploymentNode)
        {
            Visit(deploymentNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="defaultsNode"/>.
        /// </summary>
        /// <param name="defaultsNode"></param>
        public override void Visit(IDefaultsNode defaultsNode)
        {
            Visit(defaultsNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="defaultPrimaryKeyNode"/>.
        /// </summary>
        /// <param name="defaultPrimaryKeyNode"></param>
        public override void Visit(IDefaultPrimaryKeyNode defaultPrimaryKeyNode)
        {
            Visit(defaultPrimaryKeyNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="typeAliasesNode"/>.
        /// </summary>
        /// <param name="typeAliasesNode"></param>
        public override void Visit(ITypeAliasesNode typeAliasesNode)
        {
            Visit(typeAliasesNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="typeAliasNode"/>.
        /// </summary>
        /// <param name="typeAliasNode"></param>
        public override void Visit(ITypeAliasNode typeAliasNode)
        {
            Visit(typeAliasNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="removeTableNode"/>.
        /// </summary>
        /// <param name="removeTableNode"></param>
        public override void Visit(IRemoveTableNode removeTableNode)
        {
            Visit(removeTableNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="removeReferenceNode"/>.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
            Visit(removeReferenceNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="removeIndexNode"/>.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            Visit(removeIndexNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="alterTableNode"/>.
        /// </summary>
        /// <param name="alterTableNode"></param>
        public override void Visit(IAlterTableNode alterTableNode)
        {
            Visit(alterTableNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="alterColumnNode"/>.
        /// </summary>
        /// <param name="alterColumnNode"></param>
        public override void Visit(IAlterColumnNode alterColumnNode)
        {
            Visit(alterColumnNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="environmentNode"/>.
        /// </summary>
        /// <param name="environmentNode"></param>
        public override void Visit(IEnvironmentNode environmentNode)
        {
            Visit(environmentNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="upgradeNode"/>.
        /// </summary>
        /// <param name="upgradeNode"></param>
        public override void Visit(IUpgradeNode upgradeNode)
        {
            Visit(upgradeNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="downgradeNode"/>.
        /// </summary>
        /// <param name="downgradeNode"></param>
        public override void Visit(IDowngradeNode downgradeNode)
        {
            Visit(downgradeNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="removeColumnNode"/>.
        /// </summary>
        /// <param name="removeColumnNode"></param>
        public override void Visit(IRemoveColumnNode removeColumnNode)
        {
            Visit(removeColumnNode.ChildNodes);
        }

        /// <summary>
        ///  Visits the given <paramref name="templatesNode" />.
        /// </summary>
        /// <param name="templatesNode"></param>
        public override void Visit(ITemplatesNode templatesNode)
        {
            Visit(templatesNode.ChildNodes);
        }

        /// <summary>
        ///  Visits the given <paramref name="tableTemplateNode" />.
        /// </summary>
        /// <param name="tableTemplateNode"></param>
        public override void Visit(ITableTemplateNode tableTemplateNode)
        {
            Visit(tableTemplateNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="addConstraintNode"/>.
        /// </summary>
        /// <param name="addConstraintNode"></param>
        public override void Visit(IAddConstraintNode addConstraintNode)
        {
            Visit(addConstraintNode.ChildNodes);
        }

        /// <summary>
        /// Visits the given <paramref name="removeConstraintNode"/>.
        /// </summary>
        /// <param name="removeConstraintNode"></param>
        public override void Visit(IRemoveConstraintNode removeConstraintNode)
        {
            Visit(removeConstraintNode.ChildNodes);
        }
        #endregion

        #region IMdlCompilerStage Members
        /// <summary>
        /// Sets <paramref name="environment"/> for the current compilation session.
        /// </summary>
        /// <param name="environment"></param>
        public virtual void SetEnvironment(Environment environment)
        {
            this.environment = environment;
        }
        #endregion
    }
}
