using System.Diagnostics;
using System.Resources;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace octalforty.Wizardby.Ci.MSBuild
{
    public abstract class DatabaseTaskBase : Task
    {
        #region Private Fields
        private string migrationDefinitionPath;
        private bool allowDowngrade;
        #endregion

        #region Public Fields
        [Required()]
        public virtual string MigrationDefinitionPath
        {
            [DebuggerStepThrough]
            get { return migrationDefinitionPath; }
            [DebuggerStepThrough]
            set { migrationDefinitionPath = value; }
        }

        public virtual bool AllowDowngrade
        {
            [DebuggerStepThrough]
            get { return allowDowngrade; }
            [DebuggerStepThrough]
            set { allowDowngrade = value; }
        }
        #endregion

        protected DatabaseTaskBase()
        {
        }

        protected DatabaseTaskBase(ResourceManager taskResources) : 
            base(taskResources)
        {
        }

        protected DatabaseTaskBase(ResourceManager taskResources, string helpKeywordPrefix) : 
            base(taskResources, helpKeywordPrefix)
        {
        }

        
    }
}
