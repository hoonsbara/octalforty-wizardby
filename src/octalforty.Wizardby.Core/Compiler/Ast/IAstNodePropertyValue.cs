namespace octalforty.Wizardby.Core.Compiler.Ast
{
    public interface IAstNodePropertyValue
    {
    }

    public interface IListAstNodePropertyValue : IAstNodePropertyValue
    {
        IAstNodePropertyValue[] Items
        { get; set; }
    }

    public interface IStringAstNodePropertyValue : IAstNodePropertyValue
    {
        string Value
        { get; set; }
    }

    public interface ISymbolAstNodePropertyValue : IStringAstNodePropertyValue
    {
    }

    public interface IIntegerAstNodePropertyValue : IAstNodePropertyValue
    {
        int Value
        { get; set; }
    }
}