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
using System.Collections.Generic;
using System.Data;

namespace octalforty.Wizardby.Core.Migration
{
    /// <summary>
    /// Provides functionality for managing information about currently registered migration versions
    /// and for registering versioning information.
    /// </summary>
    public interface IMigrationVersionInfoManager
    {
        /// <summary>
        /// Returns a collection of all registered versions for the given <paramref name="dbTransaction"/> or
        /// empty collection if no migration versions were registered.
        /// </summary>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        IList<long> GetAllRegisteredMigrationVersions(IDbTransaction dbTransaction);

        /// <summary>
        /// Returns a collection of all registered versions for the given <paramref name="connectionString"/> or
        /// empty collection if no migration versions were registered.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IList<long> GetAllRegisteredMigrationVersions(string connectionString);

        /// <summary>
        /// Returns a value which contains the maximum migration version for the given <paramref name="dbTransaction"/>
        /// or <c>null</c> if no versioning information is present.
        /// </summary>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        long? GetCurrentMigrationVersion(IDbTransaction dbTransaction);

        /// <summary>
        /// Returns a value which contains the maximum migration version for the given <paramref name="connectionString"/>
        /// or <c>null</c> if no versioning information is present.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        long? GetCurrentMigrationVersion(string connectionString);

        /// <summary>
        /// Registers the fact of migrating to version <paramref name="version"/> with mode <paramref name="migrationMode"/>
        /// for the given <paramref name="dbTransaction"/>.
        /// </summary>
        /// <param name="dbTransaction"></param>
        /// <param name="migrationMode"></param>
        /// <param name="version"></param>
        void RegisterMigrationVersion(IDbTransaction dbTransaction, MigrationMode migrationMode, long version);
    }
}
