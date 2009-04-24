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
namespace octalforty.Wizardby.Core.Migration.Impl
{
    /// <summary>
    /// Performs selection of <see cref="MigrationMode"/> depending on versions.
    /// </summary>
    public class MigrationModeSelector
    {
        /// <summary>
        /// Returns a member of <see cref="MigrationMode"/> given a <paramref name="currentVersion"/> and a <paramref name="targetVersion"/>.
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <param name="targetVersion"></param>
        /// <returns></returns>
        public MigrationMode GetMigrationMode(long currentVersion, long? targetVersion)
        {
            if(targetVersion.HasValue && targetVersion.Value == 0)
                return MigrationMode.Downgrade;

            if(!targetVersion.HasValue)
                return MigrationMode.Upgrade;

            return currentVersion <= targetVersion ?
                MigrationMode.Upgrade :
                MigrationMode.Downgrade;
        }
    }
}
