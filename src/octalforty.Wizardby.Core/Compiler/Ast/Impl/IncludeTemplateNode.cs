using System;

namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    internal class IncludeTemplateNode : AstNode, IIncludeTemplateNode
    {
        public IncludeTemplateNode(IAstNode parent, string name) : 
            base(parent)
        {
            Name = name;
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string Name
        { get; private set; }
    }
}