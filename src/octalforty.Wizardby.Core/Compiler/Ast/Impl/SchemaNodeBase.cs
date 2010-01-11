using System;

namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    public abstract class SchemaNodeBase : AstNode, ISchemaNode
    {
        protected SchemaNodeBase(IAstNode parent) : 
            base(parent)
        {
        }

        protected SchemaNodeBase(IAstNode parent, string name) : 
            this(parent)
        {
            Name = name;
        }

        public string Name 
        { get; set; }
    }
}
