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
using System.Data;
using System.Diagnostics;
using System.IO;

using octalforty.Wizardby.Core.Compiler;
using octalforty.Wizardby.Core.Compiler.Impl;
using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    /// <summary>
    /// Standard implementation of the <see cref="IMigrationService"/>.
    /// </summary>
    public class MigrationService : IMigrationService
    {
        #region Private Fields
        private IDbPlatform dbPlatform;
        private IMigrationVersionInfoManager migrationVersionInfoManager;
        private IMigrationScriptExecutive migrationScriptExecutive;
        #endregion
        
        #region Public Properties
        public IDbPlatform DbPlatform
        {
            [DebuggerStepThrough]
            get { return dbPlatform; }
            [DebuggerStepThrough]
            set { dbPlatform = value; }
        }

        public IMigrationVersionInfoManager MigrationVersionInfoManager
        {
            [DebuggerStepThrough]
            get { return migrationVersionInfoManager; }
            [DebuggerStepThrough]
            set { migrationVersionInfoManager = value; }
        }

        public IMigrationScriptExecutive MigrationScriptExecutive
        {
            [DebuggerStepThrough]
            get { return migrationScriptExecutive; }
            [DebuggerStepThrough]
            set { migrationScriptExecutive = value; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationService"/> class.
        /// </summary>
        public MigrationService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationService"/> class.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="migrationVersionInfoManager"></param>
        /// <param name="migrationScriptExecutive"></param>
        public MigrationService(IDbPlatform dbPlatform, IMigrationVersionInfoManager migrationVersionInfoManager,
            IMigrationScriptExecutive migrationScriptExecutive)
        {
            this.dbPlatform = dbPlatform;
            this.migrationVersionInfoManager = migrationVersionInfoManager;
            this.migrationScriptExecutive = migrationScriptExecutive;
        }

        #region IMigrationService Members

        /// <summary>
        /// Occurs when starting migrating to a version <see cref="MigrationEventArgs.Version"/>.
        /// </summary>
        public event MigrationEventHandler Migrating;

        /// <summary>
        /// Occurs when successfully completed migrating to a version <see cref="MigrationEventArgs.Version"/>.
        /// </summary>
        public event MigrationEventHandler Migrated;

        public void Migrate(string connectionString, long? targetVersion, TextReader migrationDefinition)
        {
            if(connectionString == null) 
                throw new ArgumentNullException("connectionString");

            if(migrationDefinition == null) 
                throw new ArgumentNullException("migrationDefinition");

            long? currentVersion = GetCurrentVersion(dbPlatform, connectionString, migrationVersionInfoManager);

            MigrateDb(connectionString, migrationDefinition,
                currentVersion, targetVersion, GetMigrationMode(currentVersion, targetVersion));
        }

        public void Rollback(string connectionString, int step, TextReader migrationDefinition)
        {
            IList<long> registeredMigrationVersions = 
                GetRegisteredMigrationVersions(dbPlatform, connectionString, migrationVersionInfoManager);
            if(registeredMigrationVersions.Count == 0)
                return;

            long? targetVersion =
                registeredMigrationVersions.Count <= step ?
                    0 :
                    (long?)registeredMigrationVersions[
                        step > registeredMigrationVersions.Count ? 0 : registeredMigrationVersions.Count - step - 1];

            MigrateDb(MigrationMode.Downgrade, connectionString, 
                CompileMigrationScripts(dbPlatform, migrationDefinition, MigrationMode.Downgrade), GetCurrentVersion(dbPlatform, connectionString, migrationVersionInfoManager), targetVersion);
        }

        public void Redo(string connectionString, int step, TextReader migrationDefinition)
        {
            IList<long> registeredMigrationVersions = 
                GetRegisteredMigrationVersions(dbPlatform, connectionString, migrationVersionInfoManager);
            if(registeredMigrationVersions.Count == 0)
                return;

            //
            // Save original version
            long originalVersion = registeredMigrationVersions[registeredMigrationVersions.Count - 1];
            long? targetVersion =
                registeredMigrationVersions.Count <= step ?
                    0 :
                    (long?)registeredMigrationVersions[
                        step > registeredMigrationVersions.Count ? 0 : registeredMigrationVersions.Count - step - 1];

            string readToEnd = migrationDefinition.ReadToEnd();

            Migrate(connectionString, targetVersion, new StringReader(readToEnd));
            Migrate(connectionString, originalVersion, new StringReader(readToEnd));
        }

        #endregion

        private void MigrateDb(string connectionString, TextReader migrationDefinition, long? currentVersion, long? targetVersion, MigrationMode migrationMode)
        {
            //
            // First off, compile migrations
            MigrationScriptCollection migrationScripts = CompileMigrationScripts(dbPlatform, migrationDefinition, migrationMode);

            //
            // If we're downgrading (but when target version is still specified) remove the last migration
            /*if(migrationMode == MigrationMode.Downgrade && targetVersion.HasValue)
                migrationScripts.RemoveAt(migrationScripts.Count - 1);*/

            MigrateDb(migrationMode, connectionString, 
                migrationScripts, currentVersion, targetVersion);
        }

        private void MigrateDb(MigrationMode migrationMode, string connectionString, MigrationScriptCollection migrationScripts, long? currentVersion, long? targetVersion)
        {
            //
            // 
            IList<long> registeredMigrationVersions =
                GetRegisteredMigrationVersions(dbPlatform, connectionString, migrationVersionInfoManager);
            MigrationScriptCollection migrations = new MigrationScriptCollection();
            foreach(MigrationScript migrationScript in migrationScripts)
            {
                //
                // When upgrading, skip migrations we've already migrated
                if (migrationMode == MigrationMode.Upgrade && 
                    registeredMigrationVersions.Contains(migrationScript.MigrationVersion))
                    continue;

                //
                // When downgrading, do not revert migrations we've never migrated
                if (migrationMode == MigrationMode.Downgrade && 
                    !registeredMigrationVersions.Contains(migrationScript.MigrationVersion))
                    continue;

                migrations.Add(migrationScript);
            } // foreach

            //
            // Ok, we're all set. Run the migrations
            migrationScriptExecutive.ExecuteMigrationScripts(dbPlatform, migrationVersionInfoManager, 
                connectionString, migrations, currentVersion, targetVersion, migrationMode);
        }

        private static MigrationScriptCollection CompileMigrationScripts(IDbPlatform dbPlatform, TextReader migrationDefinition, 
            MigrationMode migrationMode)
        {
            MigrationScriptsCodeGenerator migrationScriptsCodeGenerator =
                new MigrationScriptsCodeGenerator(dbPlatform, migrationMode);
            
            IMdlCompiler mdlCompiler = new MdlCompiler(migrationScriptsCodeGenerator, new Compiler.Environment());
            mdlCompiler.AddCompilerStageAfter<AstFlattenerCompilerStage>(new DbNamingCompilerStage(dbPlatform.NamingStrategy));
            mdlCompiler.AddCompilerStageBefore<AstFlattenerCompilerStage>(new RefactoringStage(dbPlatform));

            mdlCompiler.Compile(migrationDefinition, MdlCompilationOptions.All);

            return migrationScriptsCodeGenerator.MigrationScripts;
        }

        private static MigrationMode GetMigrationMode(long? currentVersion, long? targetVersion)
        {
            if(!currentVersion.HasValue || !targetVersion.HasValue || currentVersion <= targetVersion)
                return MigrationMode.Upgrade;
            
            return MigrationMode.Downgrade;
        }

        private static IList<long> GetRegisteredMigrationVersions(IDbPlatform dbPlatform, string connectionString, 
            IMigrationVersionInfoManager migrationVersionInfoManager)
        {
            return Execute<IList<long>>(dbPlatform, connectionString, 
                delegate(IDbTransaction dbTransaction)
                    { return migrationVersionInfoManager.GetAllRegisteredMigrationVersions(dbTransaction); });
        }

        private static long? GetCurrentVersion(IDbPlatform dbPlatform, string connectionString, 
            IMigrationVersionInfoManager migrationVersionInfoManager)
        {
            return Execute<long?>(dbPlatform, connectionString,
                delegate(IDbTransaction dbTransaction)
                { return migrationVersionInfoManager.GetCurrentMigrationVersion(dbTransaction); });
        }

        private static T Execute<T>(IDbPlatform dbPlatform, string connectionString, Converter<IDbTransaction, T> action)
        {
            using(IDbConnection dbConnection = dbPlatform.ProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();

                using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    return action(dbTransaction);
            } // using
        }

        private void InvokeMigrated(MigrationEventArgs args)
        {
            MigrationEventHandler migratedHandler = Migrated;
            if (migratedHandler != null) migratedHandler(this, args);
        }

        private void InvokeMigrating(MigrationEventArgs args)
        {
            MigrationEventHandler migratingHandler = Migrating;
            if (migratingHandler != null) migratingHandler(this, args);
        }
    }
}
