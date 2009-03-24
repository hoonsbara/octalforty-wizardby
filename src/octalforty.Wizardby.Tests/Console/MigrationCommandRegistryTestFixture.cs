using NUnit.Framework;

using octalforty.Wizardby.Console;

namespace octalforty.Wizardby.Tests.Console
{
    [TestFixture()]
    public class MigrationCommandRegistryTestFixture
    {
        [Test()]
        public void RegisterAssembly()
        {
            MigrationCommandRegistry commandRegistry = new MigrationCommandRegistry();
            commandRegistry.RegisterAssembly(typeof(InfoMigrationCommand).Assembly);
        }

        [Test()]
        public void RegisterAssemblyAndResolveCommand()
        {
            MigrationCommandRegistry commandRegistry = new MigrationCommandRegistry();
            commandRegistry.RegisterAssembly(typeof(InfoMigrationCommand).Assembly);

            Assert.IsNotNull(commandRegistry.ResolveCommand(MigrationCommand.Info));
        }
    }
}
