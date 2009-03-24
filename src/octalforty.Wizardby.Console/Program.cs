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
using System.IO;
using System.Reflection;
using System.Text;

using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Deployment;
using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Db.Jet;
using octalforty.Wizardby.Db.SqlServer;

namespace octalforty.Wizardby.Console
{
    class Program
    {
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

            //
            // Parse parameters
            MigrationParametersParser parametersParser = new MigrationParametersParser();
            MigrationParameters parameters = parametersParser.ParseMigrationParameters(args);

            //
            // Preparing platform registry
            DbPlatformRegistry dbPlatformRegistry = new DbPlatformRegistry();
            dbPlatformRegistry.RegisterPlatform<JetPlatform>();
            dbPlatformRegistry.RegisterPlatform<SqlServerPlatform>();

            //
            // Prepare Migration Command Registry...
            MigrationCommandRegistry migrationCommandRegistry = new MigrationCommandRegistry();
            migrationCommandRegistry.RegisterAssembly(typeof(Program).Assembly);

            //
            // ...and execute whatever command we need
            /*IMigrationCommand migrationCommand = migrationCommandRegistry.ResolveCommand(parameters.Command);
            migrationCommand.Execute(dbPlatformRegistry, parameters);*/

            
            IDbPlatform dbPlatform = null;

            //
            // If environment was specified (or either /c or /p were omitted), look for "database.wdi" in the current
            // directory to get parameters from
            if(!string.IsNullOrEmpty(parameters.Environment) || 
                (string.IsNullOrEmpty(parameters.ConnectionString) || string.IsNullOrEmpty(parameters.PlatformAlias)))
            {
                //
                // Environment defaults to "development"
                if(string.IsNullOrEmpty(parameters.Environment))
                    parameters.Environment = "development";

                if(parameters.Command != MigrationCommand.Generate)
                    using(StreamReader streamReader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "database.wdi")))
                    {
                        DeploymentInfoParser deploymentInfoParser = new DeploymentInfoParser();
                        
                        IDeploymentInfo deploymentInfo = deploymentInfoParser.ParseDeploymentInfo(streamReader);
                        IEnvironment environment = null;
                        foreach(IEnvironment env in deploymentInfo.Environments)
                            if(env.Name.ToLowerInvariant().StartsWith(parameters.Environment.ToLowerInvariant()))
                            {
                                parameters.Environment = env.Name;
                                environment = env;
                                break;
                            } // if

                        if(environment == null)
                            throw new Exception();

                        parameters.PlatformAlias = environment.Properties["platform"];
                        dbPlatform = dbPlatformRegistry.ResolvePlatform(parameters.PlatformAlias);

                        //
                        // Build connection string
                        IDbConnectionStringBuilder connectionStringBuilder = dbPlatform.CreateConnectionStringBuilder();
                        foreach(string key in environment.Properties.AllKeys)
                        {
                            if(key.ToLowerInvariant() != "platform")
                                connectionStringBuilder.AppendKeyValuePair(key, environment.Properties[key]);
                        } // foreach

                        parameters.ConnectionString = connectionStringBuilder.ToString();
                    } // using
            } // if
            else
            {
                dbPlatform = dbPlatformRegistry.ResolvePlatform(parameters.PlatformAlias);
            } // else

            System.Console.WriteLine("Environment: {0}", parameters.Environment);
            System.Console.WriteLine("Connection string: {0}", parameters.ConnectionString);

            //
            // If no MDL file specified, grab the first in the current directory
            if(string.IsNullOrEmpty(parameters.MdlFileName))
                parameters.MdlFileName = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.mdl")[0].FullName;
            
            //
            // If extension is omitted, append ".mdl"
            if(string.IsNullOrEmpty(Path.GetExtension(parameters.MdlFileName)))
                parameters.MdlFileName = parameters.MdlFileName + ".mdl";

            //
            // If we're generating, skip everything
            if(parameters.Command == MigrationCommand.Generate)
            {
                if (!File.Exists(parameters.MdlFileName))
                {
                    using (StreamWriter streamWriter = new StreamWriter(parameters.MdlFileName, false))
                        streamWriter.Write(Resources.MdlTemplate,
                            Path.GetFileNameWithoutExtension(parameters.MdlFileName), DateTime.Now);

                    using (StreamWriter streamWriter = new StreamWriter("database.wdi", false))
                        streamWriter.Write(Resources.WdiTemplate,
                            Path.GetFileNameWithoutExtension(parameters.MdlFileName).ToLowerInvariant());
                } // if
                else
                    using (StreamWriter streamWriter = new StreamWriter(parameters.MdlFileName, true))
                        streamWriter.Write("{0}{0}    version {1:yyyyMMddHHmmss}:", System.Environment.NewLine, DateTime.Now);

                return;
            } // if

            IMigrationVersionInfoManager migrationVersionInfoManager =
                new DbMigrationVersionInfoManager(dbPlatform, "SchemaInfo");
            IMigrationScriptExecutive migrationScriptExecutive = new DbMigrationScriptExecutive();
            migrationScriptExecutive.Migrated += delegate(object sender, MigrationScriptExecutionEventArgs args1)
                { 
                    System.Console.WriteLine(args1.Mode == MigrationMode.Upgrade ?
                        "Upgraded to version {0}" :
                        "Downgraded from version {0}", args1.Version); 
                };

            if(parameters.Command == MigrationCommand.Info)
            {
                long? currentMigrationVersion = 
                    migrationVersionInfoManager.GetCurrentMigrationVersion(parameters.ConnectionString);
                IList<long> registeredMigrationVersions = 
                    migrationVersionInfoManager.GetAllRegisteredMigrationVersions(parameters.ConnectionString);

                System.Console.WriteLine("Current database version: {0}", currentMigrationVersion);
                System.Console.WriteLine("Registered migration versions ({0}):", registeredMigrationVersions.Count);
                
                foreach(long registeredMigrationVersion in registeredMigrationVersions)
                    System.Console.WriteLine("\t{0}", registeredMigrationVersion);

                return;
            } // if

            IMigrationService migrationService = new MigrationService();

            long? effectiveVersionOrStep = null;
            effectiveVersionOrStep = GetEffectiveVersionOrStep(parameters);

            StreamWriter writer = null;
            if(!string.IsNullOrEmpty(parameters.OutputFileName))
            {
                writer = new StreamWriter(parameters.OutputFileName, false, Encoding.ASCII);
                dbPlatform.CommandExecutive = new FileDbCommandExecutive(writer, dbPlatform.CommandExecutive);
            } // if

            try
            {
                using(StreamReader streamReader = new StreamReader(parameters.MdlFileName, true))
                {
                    switch(parameters.Command)
                    {
                        case MigrationCommand.Upgrade:
                        case MigrationCommand.Downgrade:
                        case MigrationCommand.Migrate:
                            migrationService.Migrate(dbPlatform, parameters.ConnectionString, effectiveVersionOrStep,
                                streamReader, migrationVersionInfoManager, migrationScriptExecutive);
                            break;
                        case MigrationCommand.Rollback:
                            migrationService.Rollback(dbPlatform, parameters.ConnectionString, (int)effectiveVersionOrStep.Value,
                                streamReader, migrationVersionInfoManager, migrationScriptExecutive);
                            break;
                        case MigrationCommand.Redo:
                            migrationService.Redo(dbPlatform, parameters.ConnectionString, (int)effectiveVersionOrStep.Value,
                                streamReader, migrationVersionInfoManager, migrationScriptExecutive);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                } // using
            } // try

            catch(MdlParserException e)
            {
                System.Console.WriteLine("Couldn't parse '{0}': {1}", parameters.MdlFileName, e.Message);
            } // catch

            catch(MigrationException e)
            {
                System.Console.WriteLine("Migration Exception: {0}", e.Message);
            } // catch

            catch(DbPlatformException e)
            {
                System.Console.WriteLine("{0} Exception: {1}", dbPlatformRegistry.GetPlatformName(dbPlatform), e.Message);
            } // catch

            if(writer != null)
                writer.Close();
        }

        private static long? GetEffectiveVersionOrStep(MigrationParameters parameters)
        {
            switch(parameters.Command)
            {
                case MigrationCommand.Upgrade:
                    return null;
                case MigrationCommand.Downgrade:
                    return 0;
                case MigrationCommand.Migrate:
                    return parameters.VersionOrStep;
                case MigrationCommand.Rollback:
                case MigrationCommand.Redo:
                    return parameters.VersionOrStep ?? 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
