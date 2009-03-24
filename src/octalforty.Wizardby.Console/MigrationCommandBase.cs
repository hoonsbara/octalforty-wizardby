using System;
using System.IO;

using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Deployment;

namespace octalforty.Wizardby.Console
{
    public abstract class MigrationCommandBase : IMigrationCommand
    {
        #region Private Constants
        private const string DefaultEnvironmentName = "development";
        #endregion

        #region IMigrationCommand Members
        /// <summary>
        /// Executes the current command.
        /// </summary>
        /// <param name="platformRegistry"></param>
        /// <param name="parameters"></param>
        public void Execute(DbPlatformRegistry platformRegistry, MigrationParameters parameters)
        {
            //
            // If environment was specified or either connection string or platform were omitted,
            // use "database.wdi" in the current directory.
            if(!string.IsNullOrEmpty(parameters.Environment) || 
               (string.IsNullOrEmpty(parameters.ConnectionString) || string.IsNullOrEmpty(parameters.PlatformAlias)))
            {
                //
                // Environment name defaults to "development".
                if(string.IsNullOrEmpty(parameters.Environment))
                    parameters.Environment = DefaultEnvironmentName;

                using (StreamReader streamReader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "database.wdi")))
                {
                    DeploymentInfoParser deploymentInfoParser = new DeploymentInfoParser();

                    IDeploymentInfo deploymentInfo = deploymentInfoParser.ParseDeploymentInfo(streamReader);
                    IEnvironment environment = GetEnvironment(parameters, deploymentInfo);

                    parameters.PlatformAlias = environment.Properties["platform"];
                    IDbPlatform dbPlatform = platformRegistry.ResolvePlatform(parameters.PlatformAlias);

                    //
                    // Build connection string
                    IDbConnectionStringBuilder connectionStringBuilder = dbPlatform.CreateConnectionStringBuilder();
                    foreach(string key in environment.Properties.AllKeys)
                    {
                        connectionStringBuilder.AppendKeyValuePair(key, environment.Properties[key]);
                    } // foreach

                    parameters.ConnectionString = connectionStringBuilder.ToString();
                } // using
            } // if

            InternalExecute(platformRegistry, parameters);
        }
        #endregion

        private static IEnvironment GetEnvironment(MigrationParameters parameters, IDeploymentInfo deploymentInfo)
        {
            IEnvironment environment = null;
            foreach (IEnvironment env in deploymentInfo.Environments)
                if (env.Name.ToLowerInvariant().StartsWith(parameters.Environment.ToLowerInvariant()))
                {
                    parameters.Environment = env.Name;
                    environment = env;
                    break;
                } // if

            if (environment == null)
                throw new Exception();
            return environment;
        }

        protected abstract void InternalExecute(DbPlatformRegistry platformRegistry, MigrationParameters parameters);
    }
}
