namespace octalforty.Wizardby.Core.Compiler.Ast
{
    /// <summary>
    /// Represents an <c>include-template</c> construct.
    /// </summary>
    public interface IIncludeTemplateNode : IAstNode
    {
        /// <summary>
        /// Gets the name of the template to include.
        /// </summary>
        string Name
        { get; }
    }
}
