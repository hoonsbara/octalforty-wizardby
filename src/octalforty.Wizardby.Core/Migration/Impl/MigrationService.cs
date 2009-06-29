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
using System.Diagnostics;
using System.IO;

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
        private INativeSqlResourceProvider nativeSqlResourceProvider;
        #endregion
        
        #region Public Properties
        /// <summary>
        /// Gets or sets a reference to the <see cref="IDbPlatform"/> used by this <see cref="MigrationService"/>.
        /// </summary>
        public IDbPlatform DbPlatform
        {
            [DebuggerStepThrough]
            get { return dbPlatform; }
            [DebuggerStepThrough]
            set { dbPlatform = value; }
        }

        /// <summary>
        /// Gets or sets a reference to the <see cref="IMigrationVersionInfoManager"/> that provides
        /// migration version information for this <see cref="MigrationService"/>.
        /// </summary>
        public IMigrationVersionInfoManager MigrationVersionInfoManager
        {
            [DebuggerStepThrough]
            get { return migrationVersionInfoManager; }
            [DebuggerStepThrough]
            set { migrationVersionInfoManager = value; }
        }

        /// <summary>
        /// Gets or sets a reference to the <see cref="IDbPlatform"/> used by this <see cref="MigrationService"/>.
        /// </summary>
        public IMigrationScriptExecutive MigrationScriptExecutive
        {
            [DebuggerStepThrough]
            get { return migrationScriptExecutive; }
            [DebuggerStepThrough]
            set { migrationScriptExecutive = value; }
        }

        /// <summary>
        /// Gets or sets a reference to the <see cref="INativeSqlResourceProvider"/> used by this <see cref="MigrationService"/>.
        /// </summary>
        public INativeSqlResourceProvider NativeSqlResourceProvider
        {
            [DebuggerStepThrough]
            get { return nativeSqlResourceProvider; }
            [DebuggerStepThrough]
            set { nativeSqlResourceProvider = value; }
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
        /// <param name="nativeSqlResourceProvider"></param>
        public MigrationService(IDbPlatform dbPlatform, IMigrationVersionInfoManager migrationVersionInfoManager,
            IMigrationScriptExecutive migrationScriptExecutive, INativeSqlResourceProvider nativeSqlResourceProvider)
        {
            this.dbPlatform = dbPlatform;
            this.migrationVersionInfoManager = migrationVersionInfoManager;
            this.migrationScriptExecutive = migrationScriptExecutive;
            this.nativeSqlResourceProvider = nativeSqlResourceProvider;

            this.migrationScriptExecutive.Migrating +=
                delegate(object sender, MigrationScriptExecutionEventArgs args)
                    { InvokeMigrating(new MigrationEventArgs(args.Version, args.Mode)); };
            this.migrationScriptExecutive.Migrated +=
                delegate(object sender, MigrationScriptExecutionEventArgs args)
                    { InvokeMigrated(new MigrationEventArgs(args.Version, args.Mode)); };
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

        /// <summary>
        /// Migrates the database identified by the <paramref name="connectionString"/> using
        /// <paramref name="migrationDefinition"/> to version <paramref name="targetVersion"/>.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the database</param>
        /// <param name="targetVersion">Required version of the database schema</param>
        /// <param name="migrationDefinition">Migration definition</param>
        public void Migrate(string connectionString, long? targetVersion, TextReader migrationDefinition)
        {
            if(connectionString == null) 
                throw new ArgumentNullException("connectionString");

            if(migrationDefinition == null) 
                throw new ArgumentNullException("migrationDefinition");

            InternalMigrate(connectionString, targetVersion, migrationDefinition);
        }

        /// <summary>
        /// Rolls back <paramref name="step"/> last versions of the the database identified by the 
        /// <paramref name="connectionString"/> using <paramref name="migrationDefinition"/>.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the database</param>
        /// <param name="step">Number of versions to roll back</param>
        /// <param name="migrationDefinition">Migration definition</param>
        public void Rollback(string connectionString, int step, TextReader migrationDefinition)
        {
            IList<long> registeredMigrationVersions = 
                GetRegisteredMigrationVersions(connectionString);
            if(registeredMigrationVersions.Count == 0)
                return;

            long targetVersion = GetVersionByOffset(registeredMigrationVersions, step);

            MigrateDb(MigrationMode.Downgrade, connectionString, 
                CompileMigrationScripts(dbPlatform, nativeSqlResourceProvider, migrationDefinition, MigrationMode.Downgrade), 
                GetCurrentMigrationVersion(connectionString), targetVersion);
        }

        /// <summary>
        /// Redoes <paramref name="step"/> last versions of the the database identified by the
        /// <paramref name="connectionString"/> using <paramref name="migrationDefinition"/>.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the database</param>
        /// <param name="step">Number of versions to reapply</param>
        /// <param name="migrationDefinition">Migration definition</param>
        public void Redo(string connectionString, int step, TextReader migrationDefinition)
        {
            IList<long> registeredMigrationVersions = GetRegisteredMigrationVersions(connectionString);
            if(registeredMigrationVersions.Count == 0)
                return;

            //
            // Calculate target version
            long originalVersion = GetCurrentMigrationVersion(connectionString);
            long targetVersion = GetVersionByOffset(registeredMigrationVersions, step);

            string readToEnd = migrationDefinition.ReadToEnd();

            Migrate(connectionString, targetVersion, new StringReader(readToEnd));
            Migrate(connectionString, originalVersion, new StringReader(readToEnd));
        }

        #endregion

        private void InternalMigrate(string connectionString, long? targetVersion, TextReader migrationDefinition)
        {
            long currentVersion = GetCurrentMigrationVersion(connectionString);

            MigrateDb(connectionString, migrationDefinition,
                currentVersion, targetVersion, GetMigrationMode(currentVersion, targetVersion));
        }

        private static long GetVersionByOffset(IList<long> versions, int offset)
        {
            return versions.Count > offset ?
                versions[versions.Count - offset - 1] :
                0;
        }

        private void MigrateDb(string connectionString, TextReader migrationDefinition, long currentVersion, long? targetVersion, MigrationMode migrationMode)
        {
            MigrationScriptCollection migrationScripts = CompileMigrationScripts(dbPlatform, 
                nativeSqlResourceProvider, migrationDefinition, migrationMode);
            MigrateDb(migrationMode, connectionString, migrationScripts, currentVersion, targetVersion);
        }

        private void MigrateDb(MigrationMode migrationMode, string connectionString, IEnumerable<MigrationScript> migrationScripts, 
            long currentVersion, long? targetVersion)
        {
            List<long> registeredMigrationVersions = new List<long>(GetRegisteredMigrationVersions(connectionString));

            //
            // When upgrading, set effective current version to 0 
            // to run all missing migrations
            long effectiveCurrentVersion =
                migrationMode == MigrationMode.Upgrade ?
                    0 :
                    currentVersion;

            MigrationScriptCollection migrations =
                new MigrationScriptSelector().SelectMigrationScripts(effectiveCurrentVersion, targetVersion, 
                    migrationMode, registeredMigrationVersions.ToArray(), migrationScripts);

            //
            // Ok, we're all set. Run the migrations
            migrationScriptExecutive.ExecuteMigrationScripts(dbPlatform, migrationVersionInfoManager, 
                connectionString, migrations, currentVersion, targetVersion, migrationMode);
        }

        private static MigrationScriptCollection CompileMigrationScripts(IDbPlatform dbPlatform, INativeSqlResourceProvider nativeSqlResourceProvider,
            TextReader migrationDefinition, MigrationMode migrationMode)
        {
            MigrationScriptCompiler migrationScriptCompiler = new MigrationScriptCompiler(dbPlatform, nativeSqlResourceProvider, migrationMode);
            return migrationScriptCompiler.CompileMigrationScripts(migrationDefinition);
        }

        private static MigrationMode GetMigrationMode(long currentVersion, long? targetVersion)
        {
            return new MigrationModeSelector().GetMigrationMode(currentVersion, targetVersion);
        }

        private IList<long> GetRegisteredMigrationVersions(string connectionString)
        {
            return MigrationVersionInfoManagerUtil.GetRegisteredMigrationVersions(
                MigrationVersionInfoManager, DbPlatform, connectionString);
        }

        private long GetCurrentMigrationVersion(string connectionString)
        {
            return MigrationVersionInfoManagerUtil.GetCurrentMigrationVersion(
                MigrationVersionInfoManager, DbPlatform, connectionString);
        }

        private void InvokeMigrated(MigrationEventArgs args)
        {
            MigrationEventHandler migratedHandler = Migrated;
            if(migratedHandler != null) 
                migratedHandler(this, args);
        }

        private void InvokeMigrating(MigrationEventArgs args)
        {
            MigrationEventHandler migratingHandler = Migrating;
            if(migratingHandler != null) 
                migratingHandler(this, args);
        }
    }
}
