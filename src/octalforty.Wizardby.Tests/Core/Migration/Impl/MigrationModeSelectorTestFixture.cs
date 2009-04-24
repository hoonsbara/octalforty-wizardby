using NUnit.Framework;

using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;

namespace octalforty.Wizardby.Tests.Core.Migration.Impl
{
    [TestFixture()]
    public class MigrationModeSelectorTestFixture
    {
        [Test()]
        public void GetMigrationMode()
        {
            MigrationModeSelector migrationModeSelector = new MigrationModeSelector();

            Assert.AreEqual(MigrationMode.Downgrade, migrationModeSelector.GetMigrationMode(0, 0));
            Assert.AreEqual(MigrationMode.Downgrade, migrationModeSelector.GetMigrationMode(10, 0));

            Assert.AreEqual(MigrationMode.Upgrade, migrationModeSelector.GetMigrationMode(10, null));
            Assert.AreEqual(MigrationMode.Upgrade, migrationModeSelector.GetMigrationMode(10, 10));
            Assert.AreEqual(MigrationMode.Upgrade, migrationModeSelector.GetMigrationMode(10, 15));
        }
    }
}
