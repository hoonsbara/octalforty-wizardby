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
