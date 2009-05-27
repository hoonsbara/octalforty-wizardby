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
using System.Resources;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;

namespace octalforty.Wizardby.Ci.MSBuild
{
    /// <summary>
    /// Microsoft Build task wich performs an upgrade of a database schema.
    /// </summary>
    public class UpgradeDatabase : Task
    {
        #region Private Fields
        private string dbPlatformType;
        private string connectionString;
        private int? targetVersion;
        private string migrationDefinitionPath;
        #endregion

        #region Public Properties
        [Required()]
        public string MigrationDefinitionPath
        {
            [DebuggerStepThrough]
            get { return migrationDefinitionPath; }
            [DebuggerStepThrough]
            set { migrationDefinitionPath = value; }
        }

        [Required()]
        public string DbPlatformType
        {
            [DebuggerStepThrough]
            get { return dbPlatformType; }
            [DebuggerStepThrough]
            set { dbPlatformType = value; }
        }

        [Required()]
        public string ConnectionString
        {
            [DebuggerStepThrough]
            get { return connectionString; }
            [DebuggerStepThrough]
            set { connectionString = value; }
        }

        public int TargetVersion
        {
            [DebuggerStepThrough]
            get { return targetVersion ?? -1; }
            [DebuggerStepThrough]
            set { targetVersion = value; }
        }
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
            //
            // Load DbPlatform
            Type platformType = Type.GetType(DbPlatformType);
            IDbPlatform dbPlatform = (IDbPlatform)Activator.CreateInstance(platformType, null);

            IMigrationService migrationService = new MigrationService(dbPlatform,
                new DbMigrationVersionInfoManager(dbPlatform, new DbCommandExecutionStrategy(), "SchemaInfo"),
                new DbMigrationScriptExecutive(new DbCommandExecutionStrategy()));
            migrationService.Migrated += delegate(object sender, MigrationEventArgs args)
                {
                    if(BuildEngine != null)
                        Log.LogMessage(MessageImportance.Normal, "Migrated to version {0}", args.Version);
                };
            
            using(StreamReader streamReader = new StreamReader(MigrationDefinitionPath))
                migrationService.Migrate(ConnectionString, targetVersion, streamReader);
            
            return true;
        }
        #endregion
    }
}
