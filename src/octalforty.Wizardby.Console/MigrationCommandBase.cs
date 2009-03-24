using System;
using System.IO;

using octalforty.Wizardby.Console.Properties;
using octalforty.Wizardby.Core.Db;
using octalforty.Wizardby.Core.Deployment;
using octalforty.Wizardby.Core.Migration;

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
            IDbPlatform dbPlatform = null;

            if(!string.IsNullOrEmpty(parameters.Environment) || 
               (string.IsNullOrEmpty(parameters.ConnectionString) || string.IsNullOrEmpty(parameters.PlatformAlias)))
            {
                //
                // Environment name defaults to "development".
                if(string.IsNullOrEmpty(parameters.Environment))
                    parameters.Environment = DefaultEnvironmentName;

                using(StreamReader streamReader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "database.wdi")))
                {
                    DeploymentInfoParser deploymentInfoParser = new DeploymentInfoParser();

                    IDeploymentInfo deploymentInfo = deploymentInfoParser.ParseDeploymentInfo(streamReader);
                    IEnvironment environment = GetEnvironment(parameters, deploymentInfo);

                    parameters.PlatformAlias = environment.Properties["platform"];
                    dbPlatform = platformRegistry.ResolvePlatform(parameters.PlatformAlias);
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
            else
                dbPlatform = platformRegistry.ResolvePlatform(parameters.PlatformAlias);

            if(dbPlatform == null)
                throw new MigrationException(
                    string.Format(Resources.CouldNotResolvePlatformAlias, parameters.PlatformAlias));

            //
            // If no MDL file specified, grab the first in the current directory
            if(string.IsNullOrEmpty(parameters.MdlFileName))
                parameters.MdlFileName = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.mdl")[0].FullName;

            //
            // If extension is omitted, append ".mdl"
            if(string.IsNullOrEmpty(Path.GetExtension(parameters.MdlFileName)))
                parameters.MdlFileName = parameters.MdlFileName + ".mdl";

            //
            // Environment & connection string information
            System.Console.WriteLine();
            System.Console.WriteLine(Resources.MigrationDefinitionInformation, parameters.MdlFileName);
            System.Console.WriteLine(Resources.EnvironmentInformation, parameters.Environment);
            System.Console.WriteLine(Resources.ConnectionStringInformation, parameters.ConnectionString);

            InternalExecute(parameters, dbPlatform);
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

        protected abstract void InternalExecute(MigrationParameters parameters, IDbPlatform dbPlatform);
    }
}
