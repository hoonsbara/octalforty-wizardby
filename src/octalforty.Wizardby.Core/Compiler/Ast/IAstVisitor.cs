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
namespace octalforty.Wizardby.Core.Compiler.Ast
{
    public interface IAstVisitor
    {
        /// <summary>
        /// Visits the given <paramref name="migrationNode"/>.
        /// </summary>
        /// <param name="migrationNode"></param>
        void Visit(IMigrationNode migrationNode);

        /// <summary>
        /// Visits the given <paramref name="baselineNode"/>.
        /// </summary>
        /// <param name="baselineNode"></param>
        void Visit(IBaselineNode baselineNode);

        /// <summary>
        /// Visits the given <paramref name="versionNode"/>.
        /// </summary>
        /// <param name="versionNode"></param>
        void Visit(IVersionNode versionNode);

        /// <summary>
        /// Visits the given <paramref name="addTableNode"/>.
        /// </summary>
        /// <param name="addTableNode"></param>
        void Visit(IAddTableNode addTableNode);

        /// <summary>
        /// Visits the given <paramref name="addColumnNode"/>.
        /// </summary>
        /// <param name="addColumnNode"></param>
        void Visit(IAddColumnNode addColumnNode);

        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        void Visit(IAddIndexNode addIndexNode);

        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        void Visit(IAddReferenceNode addReferenceNode);

        /// <summary>
        /// Visits the given <paramref name="deploymentNode"/>.
        /// </summary>
        /// <param name="deploymentNode"></param>
        void Visit(IDeploymentNode deploymentNode);

        /// <summary>
        /// Visits the given <paramref name="defaultsNode"/>.
        /// </summary>
        /// <param name="defaultsNode"></param>
        void Visit(IDefaultsNode defaultsNode);

        /// <summary>
        /// Visits the given <paramref name="defaultPrimaryKeyNode"/>.
        /// </summary>
        /// <param name="defaultPrimaryKeyNode"></param>
        void Visit(IDefaultPrimaryKeyNode defaultPrimaryKeyNode);

        /// <summary>
        /// Visits the given <paramref name="typeAliasesNode"/>.
        /// </summary>
        /// <param name="typeAliasesNode"></param>
        void Visit(ITypeAliasesNode typeAliasesNode);

        /// <summary>
        /// Visits the given <paramref name="typeAliasNode"/>.
        /// </summary>
        /// <param name="typeAliasNode"></param>
        void Visit(ITypeAliasNode typeAliasNode);

        /// <summary>
        /// Visits the given <paramref name="removeTableNode"/>.
        /// </summary>
        /// <param name="removeTableNode"></param>
        void Visit(IRemoveTableNode removeTableNode);

        /// <summary>
        /// Visits the given <paramref name="removeReferenceNode"/>.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        void Visit(IRemoveReferenceNode removeReferenceNode);

        /// <summary>
        /// Visits the given <paramref name="removeIndexNode"/>.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        void Visit(IRemoveIndexNode removeIndexNode);

        /// <summary>
        /// Visits the given <paramref name="alterTableNode"/>.
        /// </summary>
        /// <param name="alterTableNode"></param>
        void Visit(IAlterTableNode alterTableNode);

        /// <summary>
        /// Visits the given <paramref name="alterColumnNode"/>.
        /// </summary>
        /// <param name="alterColumnNode"></param>
        void Visit(IAlterColumnNode alterColumnNode);

        /// <summary>
        /// Visits the given <paramref name="environmentNode"/>.
        /// </summary>
        /// <param name="environmentNode"></param>
        void Visit(IEnvironmentNode environmentNode);

        /// <summary>
        /// Visits the given <paramref name="upgradeNode"/>.
        /// </summary>
        /// <param name="upgradeNode"></param>
        void Visit(IUpgradeNode upgradeNode);

        /// <summary>
        /// Visits the given <paramref name="downgradeNode"/>.
        /// </summary>
        /// <param name="downgradeNode"></param>
        void Visit(IDowngradeNode downgradeNode);

        /// <summary>
        /// Visits the given <paramref name="removeColumnNode"/>.
        /// </summary>
        /// <param name="removeColumnNode"></param>
        void Visit(IRemoveColumnNode removeColumnNode);

        /// <summary>
        /// Visits the given <paramref name="templatesNode"/>.
        /// </summary>
        /// <param name="templatesNode"></param>
        void Visit(ITemplatesNode templatesNode);

        /// <summary>
        /// Visits the given <paramref name="tableTemplateNode"/>.
        /// </summary>
        /// <param name="tableTemplateNode"></param>
        void Visit(ITableTemplateNode tableTemplateNode);

        /// <summary>
        /// Visits the given <paramref name="refactorNode"/>.
        /// </summary>
        /// <param name="refactorNode"></param>
        void Visit(IRefactorNode refactorNode);
    }
}
