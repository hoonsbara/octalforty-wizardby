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

using octalforty.Wizardby.Console.Deployment;
using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Console.Commands
{
    [MigrationCommand(MigrationCommand.Generate)]
    public class GenerateMigrationCommand : MigrationCommandBase
    {
        private const string CreateNativeSqlDirectories = "create-native-sql-directories";

        public GenerateMigrationCommand() :
            base(false, true, false, false)
        {
        }

        /// <summary>
        /// Executes the current command.
        /// </summary>
        /// <param name="parameters"></param>
        protected override void InternalExecute(MigrationParameters parameters)
        {
            //
            // If no MDL file specified, grab the first in the current directory
            if (string.IsNullOrEmpty(parameters.MdlFileName))
                parameters.MdlFileName = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.mdl")[0].FullName;

            //
            // If extension is omitted, append ".mdl"
            if (string.IsNullOrEmpty(Path.GetExtension(parameters.MdlFileName)))
                parameters.MdlFileName = parameters.MdlFileName + ".mdl";

            long timestamp = ServiceProvider.GetService<ITimestampProvider>().GetTimestamp();
            System.Console.WriteLine();

            if(!File.Exists(parameters.MdlFileName))
            {
                using(StreamWriter streamWriter = new StreamWriter(parameters.MdlFileName, false))
                    streamWriter.Write(Resources.MdlTemplate,
                        Path.GetFileNameWithoutExtension(parameters.MdlFileName), timestamp);

                using(new ConsoleStylingScope(ConsoleColor.Green))
                    System.Console.WriteLine(Resources.GeneratedFile, Path.GetFullPath(parameters.MdlFileName));

                using(StreamWriter streamWriter = new StreamWriter("database.wdi", false))
                    streamWriter.Write(Resources.WdiTemplate,
                        Path.GetFileNameWithoutExtension(parameters.MdlFileName).ToLowerInvariant());

                using (new ConsoleStylingScope(ConsoleColor.Green))
                    System.Console.WriteLine(Resources.GeneratedFile, Path.GetFullPath("database.wdi"));
            } // if
            else
            {
                using(StreamWriter streamWriter = new StreamWriter(parameters.MdlFileName, true))
                    streamWriter.Write("{0}{0}    version {1}:", Environment.NewLine, timestamp);

                using(new ConsoleStylingScope(ConsoleColor.Green))
                    System.Console.WriteLine(Resources.GeneratedVersion, timestamp);

                ResolveDbPlatform(parameters);

                IEnvironment environment = ServiceProvider.GetService<IEnvironment>();
                if(environment.Properties[CreateNativeSqlDirectories] != null && 
                    bool.Parse(environment.Properties[CreateNativeSqlDirectories].ToLowerInvariant()))
                {
                    IDbPlatform dbPlatform = ServiceProvider.GetService<IDbPlatform>();
                    string nativeResourcesPath = Path.Combine(Directory.GetCurrentDirectory(),
                        string.Format("{0}{1}{2}",
                            DbPlatformUtil.GetDbPlatformAlias(dbPlatform),
                            Path.DirectorySeparatorChar,
                            timestamp));


                    Directory.CreateDirectory(nativeResourcesPath);

                    var upgradeFilePath = Path.Combine(nativeResourcesPath, "upgrade.sql");
                    File.WriteAllText(upgradeFilePath, string.Format("/* This is an Upgrade script for {0} for version {1} */", DbPlatformUtil.GetDbPlatformAlias(dbPlatform), timestamp));

                    var downgradeFilePath = Path.Combine(nativeResourcesPath, "downgrade.sql");
                    File.WriteAllText(downgradeFilePath, string.Format("/* This is a Downgrade script for {0} for version {1} */", DbPlatformUtil.GetDbPlatformAlias(dbPlatform), timestamp));

                    using(new ConsoleStylingScope(ConsoleColor.Green))
                        System.Console.WriteLine(Resources.GeneratedNativeSqlResourcesDirectory, nativeResourcesPath);
                } // if
            } // else
        }
    }
}
