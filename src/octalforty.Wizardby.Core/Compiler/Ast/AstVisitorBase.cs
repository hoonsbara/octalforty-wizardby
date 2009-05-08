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
using System.Collections.Generic;

using octalforty.Wizardby.Core.Util;

namespace octalforty.Wizardby.Core.Compiler.Ast
{
    /// <summary>
    /// Provides an abstract base class for AST visitors.
    /// </summary>
    public abstract class AstVisitorBase : IAstVisitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AstVisitorBase"/> class.
        /// </summary>
        protected AstVisitorBase()
        {
        }

        public virtual void Visit(IMigrationNode migrationNode)
        {
        }

        public virtual void Visit(IBaselineNode baselineNode)
        {
        }

        public virtual void Visit(IVersionNode versionNode)
        {
        }

        public virtual void Visit(IAddTableNode addTableNode)
        {
        }

        public virtual void Visit(IAddColumnNode addColumnNode)
        {
        }

        public virtual void Visit(IAddIndexNode addIndexNode)
        {
        }

        public virtual void Visit(IAddReferenceNode addReferenceNode)
        {
        }

        public virtual void Visit(IDeploymentNode deploymentNode)
        {
        }

        public virtual void Visit(IDefaultsNode defaultsNode)
        {
        }

        public virtual void Visit(IDefaultPrimaryKeyNode defaultPrimaryKeyNode)
        {
        }

        public virtual void Visit(ITypeAliasesNode typeAliasesNode)
        {
        }

        public virtual void Visit(ITypeAliasNode typeAliasNode)
        {
        }

        public virtual void Visit(IRemoveTableNode removeTableNode)
        {
        }

        public virtual void Visit(IRemoveReferenceNode removeReferenceNode)
        {
        }

        public virtual void Visit(IRemoveIndexNode removeIndexNode)
        {
        }

        public virtual void Visit(IAlterTableNode alterTableNode)
        {
        }

        public virtual void Visit(IAlterColumnNode alterColumnNode)
        {
        }

        public virtual void Visit(IEnvironmentNode environmentNode)
        {
        }

        public virtual void Visit(IUpgradeNode upgradeNode)
        {
        }

        public virtual void Visit(IDowngradeNode downgradeNode)
        {
        }

        public virtual void Visit(IRemoveColumnNode removeColumnNode)
        {
        }

        public virtual void Visit(ITemplatesNode templatesNode)
        {
        }

        public virtual void Visit(ITableTemplateNode tableTemplateNode)
        {
        }

        public virtual void Visit(IRefactorNode refactorNode)
        {
        }

        public virtual void Visit(IAddConstraintNode addConstraintNode)
        {
        }

        public virtual void Visit(IRemoveConstraintNode removeConstraintNode)
        {
        }

        protected virtual void Visit(IList<IAstNode> astNodes)
        {
            List<IAstNode> nodes = new List<IAstNode>(astNodes);
            for(int i = 0; i < nodes.Count; ++i )
                nodes[i].Accept(this);
        }

        protected virtual IEnumerable<T> Filter<T>(IEnumerable<IAstNode> astNodes)
            where T : IAstNode
        {
            foreach(IAstNode astNode in astNodes)
                if(astNode is T)
                    yield return (T)astNode;
        }

        protected virtual IEnumerable<IAstNode> FilterNot<T>(IEnumerable<IAstNode> astNodes)
            where T : IAstNode
        {
            foreach(IAstNode astNode in astNodes)
                if(!(astNode is T))
                    yield return astNode;
        }

        protected virtual T GetFirst<T>(IEnumerable<IAstNode> astNodes)
            where T : IAstNode
        {
            foreach(IAstNode astNode in astNodes)
                if(astNode is T)
                    return (T)astNode;

            return default(T);
        }

        protected static T TraverseToParent<T>(IAstNode astNode)
            where T : IAstNode
        {
            if(astNode.Parent == null)
                return default(T);

            if(astNode.Parent is T)
                return (T)astNode.Parent;

            return TraverseToParent<T>(astNode.Parent);
        }

        protected static IEnumerable<T> Reverse<T>(IEnumerable<T> source)
            where T : IAstNode
        {
            return Algorithms.Reverse(source);
        }

        protected static bool IsImmediateChildOf<T>(IAstNode astNode)
            where T : IAstNode
        {
            return astNode.Parent is T;
        }
    }
}
