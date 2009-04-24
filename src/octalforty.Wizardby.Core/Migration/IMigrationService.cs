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
using System.IO;

namespace octalforty.Wizardby.Core.Migration
{
    /// <summary>
    /// Provides core migration functionality.
    /// </summary>
    public interface IMigrationService
    {
        /// <summary>
        /// Occurs when starting migrating to a version <see cref="MigrationEventArgs.Version"/>.
        /// </summary>
        event MigrationEventHandler Migrating;

        /// <summary>
        /// Occurs when successfully completed migrating to a version <see cref="MigrationEventArgs.Version"/>.
        /// </summary>
        event MigrationEventHandler Migrated;

        /// <summary>
        /// Migrates the database identified by the <paramref name="connectionString"/> using
        /// <paramref name="migrationDefinition"/> to version <paramref name="targetVersion"/>.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the database</param>
        /// <param name="targetVersion">Required version of the database schema</param>
        /// <param name="migrationDefinition">Migration definition</param>
        void Migrate(string connectionString, long? targetVersion, TextReader migrationDefinition);

        /// <summary>
        /// Rolls back <paramref name="step"/> last versions of the the database identified by the 
        /// <paramref name="connectionString"/> using <paramref name="migrationDefinition"/>.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the database</param>
        /// <param name="step">Number of versions to roll back</param>
        /// <param name="migrationDefinition">Migration definition</param>
        void Rollback(string connectionString, int step, TextReader migrationDefinition);

        /// <summary>
        /// Redoes <paramref name="step"/> last versions of the the database identified by the
        /// <paramref name="connectionString"/> using <paramref name="migrationDefinition"/>.
        /// </summary>
        /// <param name="connectionString">The connection string used to connect to the database</param>
        /// <param name="step">Number of versions to reapply</param>
        /// <param name="migrationDefinition">Migration definition</param>
        void Redo(string connectionString, int step, TextReader migrationDefinition);
    }
}
