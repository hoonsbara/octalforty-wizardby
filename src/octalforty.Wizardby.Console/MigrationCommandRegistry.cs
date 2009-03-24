using System;
using System.Collections.Generic;
using System.Reflection;

using octalforty.Wizardby.Console.Util;

namespace octalforty.Wizardby.Console
{
    public class MigrationCommandRegistry
    {
        private readonly IDictionary<MigrationCommand, Type> migrationCommandTypes = new Dictionary<MigrationCommand, Type>();

        public void RegisterAssembly(Assembly assembly)
        {
            foreach(Type exportedType in assembly.GetExportedTypes())
            {
                if(!ReflectionUtil.IsDefined<MigrationCommandAttribute>(exportedType)) 
                    continue;
                
                MigrationCommandAttribute migrationCommand =
                    ReflectionUtil.GetCustomAttribute<MigrationCommandAttribute>(exportedType);
                migrationCommandTypes[migrationCommand.Command] = exportedType;
            } // foreach
        }

        public IMigrationCommand ResolveCommand(MigrationCommand command)
        {
            return migrationCommandTypes.ContainsKey(command) ?
                (IMigrationCommand)Activator.CreateInstance(migrationCommandTypes[command]) :
                null;
        }
    }
}
