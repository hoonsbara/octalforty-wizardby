using NUnit.Framework;

using octalforty.Wizardby.Console;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;

namespace octalforty.Wizardby.Tests.Console
{
    [TestFixture()]
    public class InfoMigrationCommandTestFixture
    {
        [Test()]
        [ExpectedException(typeof(MigrationException), ExpectedMessage = "Could not resolve Platform Alias 'foodb'.")]
        public void ExecuteCommand()
        {
            MigrationParameters parameters = new MigrationParameters();
            parameters.Environment = "dev";
            parameters.PlatformAlias = "foodb";

            IMigrationCommand migrationCommand = new InfoMigrationCommand();
            migrationCommand.Execute(new DbPlatformRegistry(), parameters);
        }
    }
}
