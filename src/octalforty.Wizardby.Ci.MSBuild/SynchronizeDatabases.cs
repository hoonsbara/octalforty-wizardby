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
using System.Diagnostics;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace octalforty.Wizardby.Ci.MSBuild
{
    public class SynchronizeDatabases : Task
    {
        #region Private Fields
        private string sourceDbPlatformType;
        private string sourceConnectionString;
        private string targetDbPlatformType;
        private string targetConnectionString;
        private string migrationDefinition;
        #endregion

        #region Public Properties
        [Required()]
        public string SourceDbPlatformType
        {
            [DebuggerStepThrough]
            get { return sourceDbPlatformType; }
            [DebuggerStepThrough]
            set { sourceDbPlatformType = value; }
        }

        [Required()]
        public string SourceConnectionString
        {
            [DebuggerStepThrough]
            get { return sourceConnectionString; }
            [DebuggerStepThrough]
            set { sourceConnectionString = value; }
        }

        public string TargetDbPlatformType
        {
            [DebuggerStepThrough]
            get { return targetDbPlatformType; }
            [DebuggerStepThrough]
            set { targetDbPlatformType = value; }
        }

        [Required()]
        public string TargetConnectionString
        {
            [DebuggerStepThrough]
            get { return targetConnectionString; }
            [DebuggerStepThrough]
            set { targetConnectionString = value; }
        }

        [Required()]
        public string MigrationDefinition
        {
            [DebuggerStepThrough]
            get { return migrationDefinition; }
            [DebuggerStepThrough]
            set { migrationDefinition = value; }
        }
        #endregion
        
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
    }
}
