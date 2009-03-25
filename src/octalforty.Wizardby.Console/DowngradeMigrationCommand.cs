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
using System;
using System.Diagnostics;
using System.IO;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;

namespace octalforty.Wizardby.Console
{
    /// <summary>
    /// Implements <see cref="MigrationCommand.Downgrade"/> command logic.
    /// </summary>
    [MigrationCommand(MigrationCommand.Downgrade)]
    public class DowngradeMigrationCommand : MigrationCommandBase
    {
        protected override void InternalExecute(MigrationParameters parameters, IDbPlatform dbPlatform)
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                new DbMigrationVersionInfoManager(dbPlatform, "SchemaInfo");
            IMigrationScriptExecutive migrationScriptExecutive = new DbMigrationScriptExecutive();

            Stopwatch stopwatch = null;

            migrationScriptExecutive.Migrating += delegate(object sender, MigrationScriptExecutionEventArgs args1)
                {
                    using(new ConsoleStylingScope(ConsoleColor.Yellow))
                        System.Console.WriteLine("Downgrading from version {0}", args1.Version);

                    stopwatch = Stopwatch.StartNew();
                };

            migrationScriptExecutive.Migrated += delegate(object sender, MigrationScriptExecutionEventArgs args1)
                {
                    using(new ConsoleStylingScope(ConsoleColor.Yellow))
                        System.Console.WriteLine("Downgraded from version {0} ({1:N2} sec.)", args1.Version, stopwatch.Elapsed.TotalSeconds);
                };

            IMigrationService migrationService = new MigrationService();

            System.Console.WriteLine();
            using(StreamReader streamReader = new StreamReader(parameters.MdlFileName, true))
                migrationService.Migrate(dbPlatform, parameters.ConnectionString, 0, streamReader, 
                    migrationVersionInfoManager, migrationScriptExecutive);
        }
    }
}
