namespace octalforty.Wizardby.Core.SemanticModel
{
    public class DefaultConstraintDefinition : ConstraintDefinition, IDefaultConstraintDefinition
    {
        private string @default;

        public DefaultConstraintDefinition()
        {
        }

        public DefaultConstraintDefinition(string name) : 
            base(name)
        {
        }

        public DefaultConstraintDefinition(string name, string table, string @default) : 
            base(name, table)
        {
            this.@default = @default;
        }

        public string Default
        {
            get { return @default; }
            set { @default = value; }
        }
    }
}
