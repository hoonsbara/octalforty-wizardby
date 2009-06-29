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
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Deployment.Impl;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Core.ReverseEngineering.Impl;

namespace octalforty.Wizardby.Console
{
    class Program
    {
        private static Stopwatch stopwatch;
        private static IServiceProvider serviceProvider;
        private static MigrationParameters parameters;

        static void Main(string[] args)
        {
            System.Console.WriteLine(Resources.CopyrightInformation, 
                Assembly.GetExecutingAssembly().GetName().Version.ToString(2));

            if(args.Length == 0 || (args.Length == 1 && args[0] == "/?"))
            {
                System.Console.WriteLine();
                System.Console.WriteLine(Resources.UsageInformation);

                return;
            } // if

            serviceProvider = new ServiceProvider();
            serviceProvider.RegisterService(BuildDbPlatformRegistry());
            serviceProvider.RegisterService<IMigrationService>(delegate(IServiceProvider sp)
                {
                    MigrationService migrationService = new MigrationService(
                        sp.GetService<IDbPlatform>(),
                        sp.GetService<IMigrationVersionInfoManager>(),
                        sp.GetService<IMigrationScriptExecutive>(),
                        sp.GetService<INativeSqlResourceProvider>());

                    migrationService.Migrating += MigrationServiceMigrating;
                    migrationService.Migrated += MigrationServiceMigrated;
                
                    return migrationService;
                });
            serviceProvider.RegisterService<IMigrationVersionInfoManager>(delegate(IServiceProvider sp)
                {
                    return new DbMigrationVersionInfoManager(
                        sp.GetService<IDbPlatform>(), 
                        sp.GetService<IDbCommandExecutionStrategy>(),
                        "SchemaInfo");
                });
            serviceProvider.RegisterService<IMigrationScriptExecutive>(delegate(IServiceProvider sp)
                {
                    return new DbMigrationScriptExecutive(
                        sp.GetService<IDbCommandExecutionStrategy>());
                });
            serviceProvider.RegisterService(new DeploymentService());
            serviceProvider.RegisterService(new ReverseEngineeringService());
            serviceProvider.RegisterService(new UtcDateTimeTimestampProvider());
            
            //
            // Prepare Migration Command Registry...
            MigrationCommandRegistry migrationCommandRegistry = new MigrationCommandRegistry();
            migrationCommandRegistry.RegisterAssembly(typeof(Program).Assembly);

            try
            {
                //
                // Parse parameters
                MigrationParametersParser parametersParser = new MigrationParametersParser();
                parameters = parametersParser.ParseMigrationParameters(args);

                //
                // If we have an output file name specified, use special IDbCommandExecutionStrategy
                if(!string.IsNullOrEmpty(parameters.OutputFileName))
                    serviceProvider.RegisterService(new FileDbCommandExecutionStrategy(parameters.OutputFileName));
                else
                    serviceProvider.RegisterService(new DbCommandExecutionStrategy());

                //
                // ...and execute whatever command we need
                IMigrationCommand migrationCommand = migrationCommandRegistry.ResolveCommand(parameters.Command);
                migrationCommand.ServiceProvider = serviceProvider;
                
                migrationCommand.Execute(parameters);
            } // try

            catch(MdlParserException e)
            {
                using(new ConsoleStylingScope(ConsoleColor.Red))
                    System.Console.WriteLine(System.Environment.NewLine + "Compilation Exception: {0}", e.Message);
            } // catch

            catch(MdlCompilerException e)
            {
                using(new ConsoleStylingScope(ConsoleColor.Red))
                    System.Console.WriteLine(System.Environment.NewLine + "Compilation Exception: {0} ({1})", e.Message,
                        e.Location);
            } // catch

            catch(MigrationException e)
            {
                using(new ConsoleStylingScope(ConsoleColor.Red))
                    System.Console.WriteLine(System.Environment.NewLine + "Migration Exception: {0} ({1})", e.Message, e.SqlStatement);
            } // catch

            catch(DbPlatformException e)
            {
                IDbPlatform dbPlatform = serviceProvider.GetService<DbPlatformRegistry>().ResolvePlatform(parameters.PlatformAlias);
                using(new ConsoleStylingScope(ConsoleColor.Red))
                    System.Console.WriteLine(System.Environment.NewLine + "{0} Exception: {1}", 
                        serviceProvider.GetService<DbPlatformRegistry>().GetPlatformName(dbPlatform), e.Message + e.StackTrace.ToString());
            } // catch

            catch(DbException e)
            {
                IDbPlatform dbPlatform = serviceProvider.GetService<DbPlatformRegistry>().ResolvePlatform(parameters.PlatformAlias);
                using (new ConsoleStylingScope(ConsoleColor.Red))
                    System.Console.WriteLine(System.Environment.NewLine + "{0} Exception: {1}",
                        serviceProvider.GetService<DbPlatformRegistry>().GetPlatformName(dbPlatform), e.Message + e.StackTrace.ToString());
            } // catch

            catch(Exception e)
            {
                using(new ConsoleStylingScope(ConsoleColor.Red))
                    System.Console.WriteLine(System.Environment.NewLine + "Unknown Exception: {0}", e.ToString());
            } // catch
        }

        private static void MigrationServiceMigrated(object sender, MigrationEventArgs args)
        {
            if (args.Mode == MigrationMode.Upgrade)
                using (new ConsoleStylingScope(ConsoleColor.Green))
                    System.Console.WriteLine(Resources.UpgradedToVersion, args.Version, stopwatch.Elapsed.TotalSeconds);
            else
            {
                long currentMigrationVersion = MigrationVersionInfoManagerUtil.GetCurrentMigrationVersion(
                    serviceProvider.GetService<IMigrationVersionInfoManager>(),
                    serviceProvider.GetService<IDbPlatform>(), parameters.ConnectionString);
                using(new ConsoleStylingScope(ConsoleColor.Yellow))
                    System.Console.WriteLine(Resources.DowngradedToVersion,
                        currentMigrationVersion, stopwatch.Elapsed.TotalSeconds);
            } // else
        }

        private static void MigrationServiceMigrating(object sender, MigrationEventArgs args)
        {
            stopwatch = Stopwatch.StartNew();
        }

        private static DbPlatformRegistry BuildDbPlatformRegistry()
        {
            DbPlatformRegistry dbPlatformRegistry = new DbPlatformRegistry();
            foreach(FileInfo file in new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).GetFiles("*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file.FullName);
                    dbPlatformRegistry.RegisterAssembly(assembly);
                } // try

                catch(BadImageFormatException)
                {
                    
                } // catch
            } // foreach

            return dbPlatformRegistry;
        }
    }
}
