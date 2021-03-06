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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using octalforty.Wizardby.Console.Deployment;
using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Migration;

namespace octalforty.Wizardby.Console
{
    public abstract class MigrationCommandBase : IMigrationCommand
    {
        #region Private Constants
        private const string DefaultEnvironmentName = "development";
        #endregion

        #region Private Fields

        private readonly bool resolveDbPlatform;
        private readonly bool resolveMigrationDefinition;
        private readonly bool ensureMigrationDefinitionExists;
        private readonly bool displayEnvironmentInformation;
        #endregion

        protected MigrationCommandBase() :
            this(true, true, true, true)
        {
        }

        protected MigrationCommandBase(bool resolveDbPlatform, bool resolveMigrationDefinition, 
            bool ensureMigrationDefinitionExists, bool displayEnvironmentInformation)
        {
            this.resolveDbPlatform = resolveDbPlatform;
            this.resolveMigrationDefinition = resolveMigrationDefinition;
            this.ensureMigrationDefinitionExists = ensureMigrationDefinitionExists;
            this.displayEnvironmentInformation = displayEnvironmentInformation;
        }

        #region IMigrationCommand Members

        /// <summary>
        /// Set or sets a reference to the <see cref="IServiceProvider"/> which is
        /// used to retrieve service objects.
        /// </summary>
        public IServiceProvider ServiceProvider 
        { get; set; }

        /// <summary>
        /// Executes the current command.
        /// </summary>
        /// <param name="parameters"></param>
        public void Execute(MigrationParameters parameters)
        {
            if(resolveDbPlatform)
                ResolveDbPlatform(parameters);

            if(resolveMigrationDefinition)
                ResolveMigrationDefinition(parameters);

            if(displayEnvironmentInformation)
                DisplayEnvironmentInformation(parameters);

            InternalExecute(parameters);
        }
        #endregion

        protected IDbPlatform ResolveDbPlatform(MigrationParameters parameters)
        {
            IDbPlatform dbPlatform;
            if (!string.IsNullOrEmpty(parameters.Environment) ||
               (string.IsNullOrEmpty(parameters.ConnectionString) || string.IsNullOrEmpty(parameters.PlatformAlias)))
            {
                //
                // Environment name defaults to "development".
                if (string.IsNullOrEmpty(parameters.Environment))
                    parameters.Environment = DefaultEnvironmentName;

                string databaseWdiFilePath = Path.Combine(Directory.GetCurrentDirectory(), "database.wdi");
                if (!File.Exists(databaseWdiFilePath))
                    throw new MigrationException(string.Format(Resources.CouldNotFindDatabaseWdi, Directory.GetCurrentDirectory()));

                using(StreamReader streamReader = new StreamReader(databaseWdiFilePath))
                {
                    DeploymentInfoParser deploymentInfoParser = new DeploymentInfoParser();

                    IDeploymentInfo deploymentInfo = deploymentInfoParser.ParseDeploymentInfo(streamReader);
                    
                    IEnvironment environment = GetEnvironment(parameters, deploymentInfo);
                    ServiceProvider.RegisterService(environment);

                    if(string.IsNullOrEmpty(parameters.PlatformAlias))
                        parameters.PlatformAlias = environment.Properties["platform"];
                    
                    dbPlatform = ServiceProvider.GetService<DbPlatformRegistry>().ResolvePlatform(parameters.PlatformAlias);

                    EnsurePlatformResolved(parameters, dbPlatform);

                    //
                    // Build connection string
                    IDbConnectionStringBuilder connectionStringBuilder = dbPlatform.CreateConnectionStringBuilder();
                    foreach (string key in environment.Properties.AllKeys)
                    {
                        connectionStringBuilder.AppendKeyValuePair(key, environment.Properties[key]);
                    } // foreach

                    parameters.ConnectionString = connectionStringBuilder.ToString();
                } // using
            } // if
            else
            {
                dbPlatform = ServiceProvider.GetService<DbPlatformRegistry>().ResolvePlatform(parameters.PlatformAlias);
                EnsurePlatformResolved(parameters, dbPlatform);
            } // else

            ServiceProvider.RegisterService(dbPlatform);

            return dbPlatform;
        }

        private void EnsurePlatformResolved(MigrationParameters parameters, IDbPlatform dbPlatform)
        {
            if(dbPlatform == null)
                throw new MigrationException(
                    string.Format(Resources.CouldNotResolvePlatformAlias, parameters.PlatformAlias));
        }

        private static IEnvironment GetEnvironment(MigrationParameters parameters, IDeploymentInfo deploymentInfo)
        {
            foreach(var e in deploymentInfo.Environments)
                if(e.Name.ToLowerInvariant().StartsWith(parameters.Environment.ToLowerInvariant()))
                {
                    parameters.Environment = e.Name;
                    return ProcessSystemEnvironmentOverrides(e);
                } // if

            throw new Exception();
        }

        private static IEnvironment ProcessSystemEnvironmentOverrides(IEnvironment environment)
        {
            var environmentVariables = Environment.GetEnvironmentVariables();
            var vars = new Dictionary<string, string>();
            
            foreach(DictionaryEntry e in environmentVariables)
                vars[e.Key.ToString().ToLowerInvariant()] = e.Value.ToString();

            foreach(var k in environment.Properties.AllKeys)
            {
                var n = string.Format("wizardby_{0}", k.ToLowerInvariant().Replace("-", "_"));
                if(vars.ContainsKey(n))
                    environment.Properties[k] = vars[n];
            } // foreach

            return environment;
        }

        protected abstract void InternalExecute(MigrationParameters parameters);

        private void ResolveMigrationDefinition(MigrationParameters parameters)
        {
            //
            // If no MDL file specified, grab the first in the current directory
            // hoping it's the only one there
            if(string.IsNullOrEmpty(parameters.MdlFileName))
            {
                var mdlFiles = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.mdl");
                if(mdlFiles.Length == 0 && ensureMigrationDefinitionExists)
                    throw new MigrationException(string.Format(Resources.NoMigrationDefinition, Directory.GetCurrentDirectory()));

                if(mdlFiles.Length == 1)
                    parameters.MdlFileName = mdlFiles[0].FullName;
            } // if

            //
            // If extension is omitted, append ".mdl"
            if(string.IsNullOrEmpty(Path.GetExtension(parameters.MdlFileName)))
                parameters.MdlFileName = parameters.MdlFileName + ".mdl";
        }

        private void DisplayEnvironmentInformation(MigrationParameters parameters)
        {
            System.Console.WriteLine();
            System.Console.WriteLine(Resources.MigrationDefinitionInformation, parameters.MdlFileName);
            System.Console.WriteLine(Resources.EnvironmentInformation, parameters.Environment);
            System.Console.WriteLine(Resources.ConnectionStringInformation, parameters.ConnectionString);
            
            DbPlatformRegistry dbPlatformRegistry = ServiceProvider.GetService<DbPlatformRegistry>();
            
            System.Console.WriteLine(Resources.PlatformInformation, resolveDbPlatform ?
                dbPlatformRegistry.GetPlatformName(dbPlatformRegistry.ResolvePlatform(parameters.PlatformAlias)) :
                "");
        }
    }
}
