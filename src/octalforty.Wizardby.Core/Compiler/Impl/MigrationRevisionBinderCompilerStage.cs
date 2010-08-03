using octalforty.Wizardby.Core.Compiler.Ast;

namespace octalforty.Wizardby.Core.Compiler.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class MigrationRevisionBinderCompilerStage : MdlCompilerStageBase
    {
        public override void Visit(IMigrationNode migrationNode)
        {
            var revision = migrationNode.Properties["revision"] == null ?
                null :
                (int?)AstNodePropertyUtil.AsInteger(migrationNode.Properties["revision"].Value);

            migrationNode.Revision = revision ?? 1;
        }
    }
}