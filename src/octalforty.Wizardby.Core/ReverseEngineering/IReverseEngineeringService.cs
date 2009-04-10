using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Core.ReverseEngineering
{
    public interface IReverseEngineeringService
    {
        IAstNode ReverseEngineer(IDbPlatform dbPlatform, string connectionString);
    }
}
