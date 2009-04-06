using System;
using System.Reflection;
using octalforty.Wizardby.Console.Util;

namespace octalforty.Wizardby.Console
{
    public abstract class AttributeAwareTypeRegistry<TAttribute>
        where TAttribute : Attribute
    {
        public void RegisterAssembly(Assembly assembly)
        {
            foreach(Type exportedType in assembly.GetExportedTypes())
            {
                if(!ReflectionUtil.IsDefined<TAttribute>(exportedType)) 
                    continue;

                TAttribute attribute = ReflectionUtil.GetCustomAttribute<TAttribute>(exportedType);
                RegisterType(exportedType, attribute);
            } // foreach
        }

        protected abstract void RegisterType(Type type, TAttribute attribute);
    }
}