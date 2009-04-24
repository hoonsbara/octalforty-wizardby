#region The MIT License
// The MIT License
// 
// Copyright (c) 2009 octalforty studios
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
using NUnit.Framework;

using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;

namespace octalforty.Wizardby.Tests.Core.Migration.Impl
{
    [TestFixture()]
    public class MigrationScriptSelectorTestFixture
    {
        [Test()]
        public void SelectMigrationScriptsForUpgrade()
        {
            MigrationScriptSelector migrationScriptSelector = new MigrationScriptSelector();
            MigrationScriptCollection migrationScripts = 
                migrationScriptSelector.SelectMigrationScripts(0, 3, MigrationMode.Upgrade, null, 
                    new MigrationScript[]
                        {
                            new MigrationScript(1, null), 
                            new MigrationScript(2, null), 
                            new MigrationScript(3, null), 
                        });

            Assert.AreEqual(3, migrationScripts.Count);
            Assert.AreEqual(1, migrationScripts[0].MigrationVersion);
            Assert.AreEqual(2, migrationScripts[1].MigrationVersion);
            Assert.AreEqual(3, migrationScripts[2].MigrationVersion);
        }

        [Test()]
        public void SelectMigrationScriptsForUpgradeWithRegisteredMigrations()
        {
            MigrationScriptSelector migrationScriptSelector = new MigrationScriptSelector();
            MigrationScriptCollection migrationScripts = 
                migrationScriptSelector.SelectMigrationScripts(0, 5, MigrationMode.Upgrade, new long[] { 2, 4 }, 
                    new MigrationScript[]
                        {
                            new MigrationScript(1, null), 
                            new MigrationScript(2, null), 
                            new MigrationScript(3, null), 
                            new MigrationScript(4, null), 
                            new MigrationScript(5, null), 
                        });

            Assert.AreEqual(3, migrationScripts.Count);
            Assert.AreEqual(1, migrationScripts[0].MigrationVersion);
            Assert.AreEqual(3, migrationScripts[1].MigrationVersion);
            Assert.AreEqual(5, migrationScripts[2].MigrationVersion);
        }

        [Test()]
        public void SelectMigrationScriptsForUpgradeWithRegisteredMigrations2()
        {
            MigrationScriptSelector migrationScriptSelector = new MigrationScriptSelector();
            MigrationScriptCollection migrationScripts = 
                migrationScriptSelector.SelectMigrationScripts(1, 3, MigrationMode.Upgrade, new long[] { 2, 4, 5 }, 
                    new MigrationScript[]
                        {
                            new MigrationScript(1, null), 
                            new MigrationScript(2, null), 
                            new MigrationScript(3, null), 
                            new MigrationScript(4, null), 
                            new MigrationScript(5, null), 
                        });

            Assert.AreEqual(1, migrationScripts.Count);
            Assert.AreEqual(3, migrationScripts[0].MigrationVersion);
        }

        [Test()]
        public void SelectMigrationScriptsForUpgradeWithRegisteredMigrations3()
        {
            MigrationScriptSelector migrationScriptSelector = new MigrationScriptSelector();
            MigrationScriptCollection migrationScripts = 
                migrationScriptSelector.SelectMigrationScripts(2, null, MigrationMode.Upgrade, new long[] { 2, 4 }, 
                    new MigrationScript[]
                        {
                            new MigrationScript(1, null), 
                            new MigrationScript(2, null), 
                            new MigrationScript(3, null), 
                            new MigrationScript(4, null), 
                            new MigrationScript(5, null), 
                        });

            Assert.AreEqual(2, migrationScripts.Count);
            Assert.AreEqual(3, migrationScripts[0].MigrationVersion);
            Assert.AreEqual(5, migrationScripts[1].MigrationVersion);
        }

        [Test()]
        public void SelectMigrationScriptsForDowngrade()
        {
            MigrationScriptSelector migrationScriptSelector = new MigrationScriptSelector();
            MigrationScriptCollection migrationScripts =
                migrationScriptSelector.SelectMigrationScripts(3, 0, MigrationMode.Upgrade, null,
                    new MigrationScript[]
                        {
                            new MigrationScript(1, null), 
                            new MigrationScript(2, null), 
                            new MigrationScript(3, null), 
                        });

            //
            // We've not registered a single migration, so we should not have anything here
            Assert.AreEqual(0, migrationScripts.Count);
        }

        [Test()]
        public void SelectMigrationScriptsForDowngradeWithRegisteredMigrations()
        {
            MigrationScriptSelector migrationScriptSelector = new MigrationScriptSelector();
            MigrationScriptCollection migrationScripts = 
                migrationScriptSelector.SelectMigrationScripts(5, 0, MigrationMode.Downgrade, new long[] { 1, 2, 5 }, 
                    new MigrationScript[]
                        {
                            new MigrationScript(1, null), 
                            new MigrationScript(2, null), 
                            new MigrationScript(3, null), 
                            new MigrationScript(4, null), 
                            new MigrationScript(5, null), 
                        });

            Assert.AreEqual(3, migrationScripts.Count);

            //
            // We need to roll back only those migrations we previously applied
            Assert.AreEqual(1, migrationScripts[0].MigrationVersion);
            Assert.AreEqual(2, migrationScripts[1].MigrationVersion);
            Assert.AreEqual(5, migrationScripts[2].MigrationVersion);
        }

        [Test()]
        public void SelectMigrationScriptsSubsetForDowngradeWithRegisteredMigrations()
        {
            MigrationScriptSelector migrationScriptSelector = new MigrationScriptSelector();
            MigrationScriptCollection migrationScripts = 
                migrationScriptSelector.SelectMigrationScripts(4, 0, MigrationMode.Downgrade, new long[] { 1, 2 }, 
                    new MigrationScript[]
                        {
                            new MigrationScript(1, null), 
                            new MigrationScript(2, null), 
                            new MigrationScript(3, null), 
                            new MigrationScript(4, null), 
                            new MigrationScript(5, null), 
                        });

            Assert.AreEqual(2, migrationScripts.Count);

            Assert.AreEqual(1, migrationScripts[0].MigrationVersion);
            Assert.AreEqual(2, migrationScripts[1].MigrationVersion);
        }

        [Test()]
        public void SelectMigrationScriptsSubsetForDowngradeWithRegisteredMigrations2()
        {
            MigrationScriptSelector migrationScriptSelector = new MigrationScriptSelector();
            MigrationScriptCollection migrationScripts = 
                migrationScriptSelector.SelectMigrationScripts(0, null, MigrationMode.Downgrade, new long[] { 1, 2 }, 
                    new MigrationScript[]
                        {
                            new MigrationScript(1, null), 
                            new MigrationScript(2, null), 
                            new MigrationScript(3, null), 
                            new MigrationScript(4, null), 
                            new MigrationScript(5, null), 
                        });

            Assert.AreEqual(2, migrationScripts.Count);

            Assert.AreEqual(1, migrationScripts[0].MigrationVersion);
            Assert.AreEqual(2, migrationScripts[1].MigrationVersion);
        }
    }
}
