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
using System.Resources;

using Microsoft.Build.Utilities;

namespace octalforty.Wizardby.Ci.MSBuild
{
    /// <summary>
    /// Microsoft Build task wich performs an upgrade of a database schema.
    /// </summary>
    public class UpgradeDatabase : Task
    {
        #region Private Fields
        private string platform;
        private string connectionString;
        private int? targetVersion;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeDatabase"/> class.
        /// </summary>
        public UpgradeDatabase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeDatabase"/> class.
        /// </summary>
        /// <param name="taskResources"></param>
        public UpgradeDatabase(ResourceManager taskResources) : 
            base(taskResources)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeDatabase"/> class.
        /// </summary>
        /// <param name="taskResources"></param>
        /// <param name="helpKeywordPrefix"></param>
        public UpgradeDatabase(ResourceManager taskResources, string helpKeywordPrefix) : 
            base(taskResources, helpKeywordPrefix)
        {
        }

        #region Task Members
        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            return true;
        }
        #endregion
    }
}
