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
using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Migration;

namespace octalforty.Wizardby.Console
{
    public class MigrationParametersParser
    {
        public MigrationParameters ParseMigrationParameters(string[] arguments)
        {
            Queue<string> args = new Queue<string>(arguments);
            return ParseMigrationParameters(args);
        }

        private static MigrationParameters ParseMigrationParameters(Queue<string> arguments)
        {
            MigrationParameters parameters = new MigrationParameters();

            //
            // First argument is the command name
            parameters.Command = ParseMigrationCommand(arguments.Dequeue());

            //
            // If next argument is an integer, use it as a version or step
            long versionOrStep;
            if(arguments.Count > 0 && long.TryParse(arguments.Peek(), out versionOrStep))
            {
                parameters.VersionOrStep = versionOrStep;
                arguments.Dequeue();
            }

            foreach(string argument in arguments)
                ParseMigrationParameter(parameters, argument);

            return parameters;
        }

        private static MigrationCommand ParseMigrationCommand(string command)
        {
            string[] commandNames = Enum.GetNames(typeof(MigrationCommand));
            foreach(string commandName in commandNames)
                if(commandName.ToLower().StartsWith(command.ToLower()))
                    return (MigrationCommand)Enum.Parse(typeof(MigrationCommand), commandName, true);

            throw new MigrationException(string.Format(Resources.UnknownCommand, command));
        }

        private static void ParseMigrationParameter(MigrationParameters parameters, string argument)
        {
            string arg = argument.ToLowerInvariant();

            if(arg.StartsWith("/c"))
                parameters.ConnectionString = ExtractArgumentValue(argument).Trim('"');
            else if(arg.StartsWith("/p"))
                parameters.PlatformAlias = ExtractArgumentValue(argument);
            else if(arg.StartsWith("/m"))
                parameters.MdlFileName = ExtractArgumentValue(argument);
            else if(arg.StartsWith("/e"))
                parameters.Environment = ExtractArgumentValue(argument);
            else if(arg.StartsWith("/o"))
                parameters.OutputFileName = ExtractArgumentValue(argument);
        }

        private static string ExtractArgumentValue(string argument)
        {
            return argument.Substring(argument.IndexOf(":") + 1);
        }
    }
}
