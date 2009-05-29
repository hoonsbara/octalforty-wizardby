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
using System.Reflection;
using System.Text;

using octalforty.Wizardby.Core.Migration;

namespace octalforty.Wizardby.Tests.Util
{
    public static class MigrationServiceUtil
    {
        public static void WithResource(string migrationDefinition, Action<Stream> action)
        {
            using(Stream resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(migrationDefinition))
            {
                action(resourceStream);
            } // using
        }

        public static void MigrateTo(IMigrationService migrationService, string connectionString, 
            string migrationDefinitionResourceName, int? targetVersion)
        {
            WithResource(migrationDefinitionResourceName,
                delegate(Stream stream)
                {
                    migrationService.Migrate(connectionString, targetVersion, new StreamReader(stream, Encoding.UTF8));
                });
        }
    }
}
