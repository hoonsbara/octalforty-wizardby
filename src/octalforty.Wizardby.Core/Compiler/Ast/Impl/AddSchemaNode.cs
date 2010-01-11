namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    public class AddSchemaNode : SchemaNodeBase, IAddSchemaNode
    {
        public AddSchemaNode(IAstNode parent) : 
            base(parent)
        {
        }

        public AddSchemaNode(IAstNode parent, string name) : 
            base(parent, name)
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}