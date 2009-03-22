using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;
using octalforty.Wizardby.Core.Util;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    public class DowngradeGenerationStage : MdlCompilerStageBase
    {
        /// <summary>
        /// Sets <paramref name="environment"/> for the current compilation session.
        /// </summary>
        /// <param name="environment"></param>
        public override void SetEnvironment(Environment environment)
        {
            //
            // We don't need existing environment here.
            base.SetEnvironment(new Environment());
        }

        /// <summary>
        /// Visits the given <paramref name="baselineNode"/>.
        /// </summary>
        /// <param name="baselineNode"></param>
        public override void Visit(IBaselineNode baselineNode)
        {
        }

        /// <summary>
        /// Visits the given <paramref name="versionNode"/>.
        /// </summary>
        /// <param name="versionNode"></param>
        public override void Visit(IVersionNode versionNode)
        {
            //
            // Add an IDowngradeNode to versionNode if it does not
            // have one already
            // TODO: Shoud user be able to specify downgrade: expilcitly?
            versionNode.ChildNodes.Add(new DowngradeNode(versionNode));
            
            base.Visit(versionNode);
        }

        /// <summary>
        /// Visits the given <paramref name="downgradeNode"/>.
        /// </summary>
        /// <param name="downgradeNode"></param>
        public override void Visit(IDowngradeNode downgradeNode)
        {
        }

        /// <summary>
        /// Visits the given <paramref name="upgradeNode"/>.
        /// </summary>
        /// <param name="upgradeNode"></param>
        public override void Visit(IUpgradeNode upgradeNode)
        {
            base.Visit(upgradeNode);
        }

        /// <summary>
        /// Visits the given <paramref name="addTableNode"/>.
        /// </summary>
        /// <param name="addTableNode"></param>
        public override void Visit(IAddTableNode addTableNode)
        {
            //
            // Add IRemoveTableNode to the front if IDowngradeNode
            IDowngradeNode downgradeNode = GetDowngradeNodeFor(addTableNode);
            downgradeNode.ChildNodes.Insert(0, new RemoveTableNode(downgradeNode, addTableNode.Name));
        }

        /// <summary>
        /// Visits the given <paramref name="addIndexNode"/>.
        /// </summary>
        /// <param name="addIndexNode"></param>
        public override void Visit(IAddIndexNode addIndexNode)
        {
            //
            // Add IRemoveIndexNode
            IDowngradeNode downgradeNode = GetDowngradeNodeFor(addIndexNode);

            IRemoveIndexNode removeIndexNode = new RemoveIndexNode(downgradeNode, addIndexNode.Name);
            removeIndexNode.Table = addIndexNode.Table;

            downgradeNode.ChildNodes.Insert(0, removeIndexNode);

        }

        /// <summary>
        /// Visits the given <paramref name="addReferenceNode"/>.
        /// </summary>
        /// <param name="addReferenceNode"></param>
        public override void Visit(IAddReferenceNode addReferenceNode)
        {
            //
            // Add IRemoveReferenceNode
            IDowngradeNode downgradeNode = GetDowngradeNodeFor(addReferenceNode);
            
            RemoveReferenceNode removeReferenceNode = new RemoveReferenceNode(downgradeNode, addReferenceNode.Name);
            removeReferenceNode.Table = addReferenceNode.FkTable;

            downgradeNode.ChildNodes.Insert(0, removeReferenceNode);
        }

        /// <summary>
        /// Visits the given <paramref name="removeTableNode"/>.
        /// </summary>
        /// <param name="removeTableNode"></param>
        public override void Visit(IRemoveTableNode removeTableNode)
        {
            base.Visit(removeTableNode);
        }

        /// <summary>
        /// Visits the given <paramref name="removeReferenceNode"/>.
        /// </summary>
        /// <param name="removeReferenceNode"></param>
        public override void Visit(IRemoveReferenceNode removeReferenceNode)
        {
            base.Visit(removeReferenceNode);
        }

        /// <summary>
        /// Visits the given <paramref name="removeIndexNode"/>.
        /// </summary>
        /// <param name="removeIndexNode"></param>
        public override void Visit(IRemoveIndexNode removeIndexNode)
        {
            base.Visit(removeIndexNode);
        }

        /// <summary>
        /// Visits the given <paramref name="alterTableNode"/>.
        /// </summary>
        /// <param name="alterTableNode"></param>
        public override void Visit(IAlterTableNode alterTableNode)
        {
            base.Visit(alterTableNode);
        }

        private IDowngradeNode GetDowngradeNodeFor(IAstNode astNode)
        {
            IAstNode currentNode = astNode;
            while(!(currentNode is  IVersionNode))
                currentNode = currentNode.Parent;

            return (IDowngradeNode)Algorithms.FindFirst(currentNode.ChildNodes, 
                delegate(IAstNode an) { return an is IDowngradeNode; });
        }
    }
}
