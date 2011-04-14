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

namespace octalforty.Wizardby.Core.Migration.Impl
{
    /// <summary>
    /// Performs selection of <see cref="MigrationScript"/> to perform upgrades or downgrades.
    /// </summary>
    public class MigrationScriptSelector
    {
        /// <summary>
        /// Returns a collection of <see cref="MigrationScript"/> objects to perform migration (<paramref name="migrationMode"/>)
        /// from <paramref name="currentVersion"/> to <paramref name="targetVersion"/>.
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <param name="targetVersion"></param>
        /// <param name="migrationMode"></param>
        /// <param name="registeredVersions"></param>
        /// <param name="migrationScripts"></param>
        /// <returns></returns>
        public MigrationScriptCollection SelectMigrationScripts(long currentVersion, long? targetVersion, MigrationMode migrationMode,
            long[] registeredVersions, IEnumerable<MigrationScript> migrationScripts)
        {
            var selectedMigrationScripts = new MigrationScriptCollection();

            foreach(var migrationScript in migrationScripts)
            {
                if(migrationMode == MigrationMode.Upgrade)
                {
                    //
                    // Skip already registered migrations
                    if(Contains(registeredVersions, migrationScript.MigrationVersion))
                        continue;

                    if(migrationScript.MigrationVersion <= currentVersion)
                        continue;

                    if(targetVersion.HasValue && migrationScript.MigrationVersion > targetVersion.Value)
                        continue;
                } // if

                if(migrationMode == MigrationMode.Downgrade)
                {
                    if(!Contains(registeredVersions, migrationScript.MigrationVersion))
                        continue;

                    if(targetVersion.HasValue && migrationScript.MigrationVersion <= targetVersion.Value)
                        continue;
                } // if

                selectedMigrationScripts.Add(migrationScript);
            } // foreach

            return selectedMigrationScripts;
        }

        public static bool Contains<T>(T[] values, T value)
        {
            if(values == null)
                return false;

            return Array.IndexOf(values, value) > -1;
        }
    }
}
