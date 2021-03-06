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
using System.IO;
using System.Text;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Ast;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.ReverseEngineering;

namespace octalforty.Wizardby.Console.Commands
{
    /// <summary>
    /// Implements <see cref="MigrationCommand.ReverseEngineer"/> command logic.
    /// </summary>
    [MigrationCommand(MigrationCommand.ReverseEngineer)]
    public class ReverseEngineerMigrationCommand : MigrationCommandBase
    {
        public ReverseEngineerMigrationCommand() :
            base(true, false, false, true)
        {
        }

        protected override void InternalExecute(MigrationParameters parameters)
        {
            System.Console.WriteLine();

            using(new ConsoleStylingScope(ConsoleColor.Green))
                System.Console.WriteLine("Reverse engineering '{0}'", parameters.ConnectionString);

            IReverseEngineeringService reverseEngineeringService = ServiceProvider.GetService<IReverseEngineeringService>();
            IDbPlatform dbPlatform = ServiceProvider.GetService<IDbPlatform>();

            IAstNode astNode = reverseEngineeringService.ReverseEngineer(dbPlatform, parameters.ConnectionString);

            MdlGenerator mdlGenerator = new MdlGenerator();

            using(FileStream fs = new FileStream("baseline.mdl", FileMode.Create))
            {
                using(StreamWriter streamWriter = new StreamWriter(fs, Encoding.UTF8))
                {
                    mdlGenerator.Generate(astNode, streamWriter);
                    streamWriter.Flush();
                } // using
            } // using
        }
    }
}
