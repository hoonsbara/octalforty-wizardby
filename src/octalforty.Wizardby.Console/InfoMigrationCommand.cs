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
using System.Collections.Generic;

using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Migration;

namespace octalforty.Wizardby.Console
{
    /// <summary>
    /// Implements <see cref="MigrationCommand.Info"/> command logic.
    /// </summary>
    [MigrationCommand(MigrationCommand.Info)]
    public class InfoMigrationCommand : MigrationCommandBase
    {
        public InfoMigrationCommand() :
            base(true, false, false, true)
        {
        }

        #region MigrationCommandBase Members
        /// <summary>
        /// Executes the current command.
        /// </summary>
        /// <param name="parameters"></param>
        protected override void InternalExecute(MigrationParameters parameters)
        {
            IMigrationVersionInfoManager migrationVersionInfoManager =
                ServiceProvider.GetService<IMigrationVersionInfoManager>();

            long? currentMigrationVersion = 
                migrationVersionInfoManager.GetCurrentMigrationVersion(parameters.ConnectionString);
            IList<long> registeredMigrationVersions = 
                migrationVersionInfoManager.GetAllRegisteredMigrationVersions(parameters.ConnectionString);

            if(!currentMigrationVersion.HasValue)
            {
                using(new ConsoleStylingScope(ConsoleColor.Yellow))
                    System.Console.WriteLine(Environment.NewLine + Resources.DatabaseIsNotVersioned);

                return;
            } // if


            System.Console.WriteLine();
            using(new ConsoleStylingScope(ConsoleColor.Green))
            {
                System.Console.WriteLine(Resources.CurrentDatabaseVersionInfo, currentMigrationVersion.Value);

                if(registeredMigrationVersions.Count == 0)
                    return;

                System.Console.WriteLine(Resources.RegisteredDatabaseVersionsInfo);
                foreach(long registeredVersion in registeredMigrationVersions)
                    System.Console.WriteLine(Resources.RegisteredDatabaseVersionInfo, registeredVersion);
            } // using
        }
        #endregion
    }
}
