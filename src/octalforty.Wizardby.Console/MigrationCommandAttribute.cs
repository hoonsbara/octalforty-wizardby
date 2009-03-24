using System;
using System.Diagnostics;

namespace octalforty.Wizardby.Console
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MigrationCommandAttribute : Attribute
    {
        private readonly MigrationCommand command;

        public MigrationCommand Command
        {
            [DebuggerStepThrough]
            get { return command; }
        }

        public MigrationCommandAttribute(MigrationCommand command)
        {
            this.command = command;
        }
    }
}
