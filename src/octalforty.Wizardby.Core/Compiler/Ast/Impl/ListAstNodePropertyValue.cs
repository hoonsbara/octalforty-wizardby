namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    public class ListAstNodePropertyValue : IListAstNodePropertyValue
    {
        private IAstNodePropertyValue[] items;

        public ListAstNodePropertyValue()
        {
        }

        public ListAstNodePropertyValue(params IAstNodePropertyValue[] items)
        {
            this.items = items;
        }

        public IAstNodePropertyValue[] Items
        {
            get { return items; }
            set { items = value; }
        }
    }
}
