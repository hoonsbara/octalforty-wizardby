using System;
using System.Diagnostics;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    public class MigrationScriptExecutionEventArgs : EventArgs
    {
        private readonly long version;

        public long Version
        {
            [DebuggerStepThrough]
            get { return version; }
        }

        public MigrationScriptExecutionEventArgs(long version)
        {
            this.version = version;
        }
    }
}
