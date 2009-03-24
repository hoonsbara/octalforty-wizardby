using System;
using System.Diagnostics;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    public class MigrationScriptExecutionEventArgs : EventArgs
    {
        private readonly MigrationMode mode;
        private readonly long version;

        public MigrationMode Mode
        {
            [DebuggerStepThrough]
            get { return mode; }
        }

        public long Version
        {
            [DebuggerStepThrough]
            get { return version; }
        }

        public MigrationScriptExecutionEventArgs(MigrationMode mode, long version)
        {
            this.mode = mode;
            this.version = version;
        }
    }
}
