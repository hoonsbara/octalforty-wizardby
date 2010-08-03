using System;

namespace octalforty.Wizardby.Core.Compiler.Ast
{
    public static class AstNodePropertyCollectionExtensions
    {
        public static bool AsBoolean(this IAstNodePropertyCollection properties, string name)
        {
            return Convert.ToBoolean(properties.AsString(name));
        }

        public static string AsString(this IAstNodePropertyCollection properties, string name)
        {
            return AstNodePropertyUtil.AsString(properties, name);
        }
    }
}
