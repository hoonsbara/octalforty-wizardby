using System.Diagnostics;

namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    [DebuggerDisplay("add constraint {Name}")]
    internal class AddConstraintNode : AstNode, IAddConstraintNode
    {
        #region Private Fields
        private string name;
        #endregion

        public AddConstraintNode(IAstNode parent, string name) : base(parent)
        {
            this.name = name;
        }

        #region AstNode Members
        /// <summary>
        /// Accepts a given <paramref name="visitor"/>.
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
        #endregion

        #region IAddConstraintNode Members
        /// <summary>
        /// Gets or sets a string which contains the name of the current schema element.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion
    }
}