namespace octalforty.Wizardby.Core.Compiler.Ast.Impl
{
    public class IntegerAstNodePropertyValue : IIntegerAstNodePropertyValue
    {
        private int value;

        public IntegerAstNodePropertyValue()
        {
        }

        public IntegerAstNodePropertyValue(int value)
        {
            this.value = value;
        }

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
