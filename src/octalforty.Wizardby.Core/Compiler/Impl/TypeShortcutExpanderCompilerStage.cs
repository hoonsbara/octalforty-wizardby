using System.Text.RegularExpressions;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Compiler.Ast.Impl;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    /// <summary>
    /// Expands type shortcuts to normal type specifications.
    /// </summary>
    /// <remarks>
    /// This stage expands short forms of type definitions like <c>"int32!"</c>, <c>string?(200)</c> or
    /// <c>decimal?(18,3)</c> to their full counterparts.
    /// <para />
    /// This applies only to <see cref="IAddColumnNode"/> and <see cref="IAlterColumnNode"/>.
    /// </remarks>
    public class TypeShortcutExpanderCompilerStage : MdlCompilerStageBase
    {
        private readonly Regex TypeWithScaleAndPrecision = new Regex(@"(?<TypeName>(\w+?))\s*(?<Nullability>(\!|\?))\s*(\(\s*(?<Scale>(\d+?))\s*,\s*(?<Precision>(\d+?))\s*\))");
        private readonly Regex TypeWithOptionalLength = new Regex(@"(?<TypeName>(\w+?))\s*(?<Nullability>(\!|\?))\s*(\(\s*(?<Length>(\d+?))\s*\))?");

        public override void Visit(octalforty.Wizardby.Core.Compiler.Ast.IAddColumnNode addColumnNode)
        {
            if(addColumnNode.Properties[MdlSyntax.Type] == null)
                return;

            var type = AstNodePropertyUtil.AsString(addColumnNode.Properties[MdlSyntax.Type].Value);
            
            //
            // If we match against something like "decimal!(18,2)"
            var typeWithScaleAndPrecision = TypeWithScaleAndPrecision.Match(type);
            if(typeWithScaleAndPrecision.Success)
            {
                ExpandTypeAndNullability(addColumnNode, typeWithScaleAndPrecision);

                addColumnNode.Properties.AddProperty(
                    AstNodeProperty.Integer(MdlSyntax.Scale, int.Parse(typeWithScaleAndPrecision.Groups["Scale"].Value)));
                addColumnNode.Properties.AddProperty(
                    AstNodeProperty.Integer(MdlSyntax.Precision, int.Parse(typeWithScaleAndPrecision.Groups["Precision"].Value)));

                return;
            } // if

            //
            // If we match against something like "int32!" or "string?(300)", make it so.
            var typeWithOptionalLength = TypeWithOptionalLength.Match(type);
            if(typeWithOptionalLength.Success)
            {
                ExpandTypeAndNullability(addColumnNode, typeWithOptionalLength);

                if(typeWithOptionalLength.Groups["Length"].Success)
                {
                    addColumnNode.Properties.AddProperty(
                        AstNodeProperty.Integer(MdlSyntax.Length, int.Parse(typeWithOptionalLength.Groups["Length"].Value)));
                } // if
            }
        }

        public override void Visit(octalforty.Wizardby.Core.Compiler.Ast.IAlterColumnNode alterColumnNode)
        {
            base.Visit(alterColumnNode);
        }

        private void ExpandTypeAndNullability(IAddColumnNode addColumnNode, Match match)
        {
            addColumnNode.Properties.AddProperty(
                AstNodeProperty.Symbol(MdlSyntax.Type, match.Groups["TypeName"].Value));
            addColumnNode.Properties.AddProperty(
                AstNodeProperty.Symbol(MdlSyntax.Nullable, match.Groups["Nullability"].Value == "?" ?
                    "true" : "false"));
        }
    }
}
