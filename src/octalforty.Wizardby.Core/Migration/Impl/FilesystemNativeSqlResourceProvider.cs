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
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using octalforty.Wizardby.Core.Db;

using System.Linq;

using octalforty.Wizardby.Core.Util;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    public class FileSystemNativeSqlResourceProvider : INativeSqlResourceProvider
    {
        private readonly string baseDirectory;

        public FileSystemNativeSqlResourceProvider(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        #region INativeSqlResourceProvider Members
        /// <summary>
        /// Returns an array of <see cref="string"/> object which contains a Native SQL resource <paramref name="name"/>
        /// to be executed while upgrading to version <paramref name="version"/> 
        /// for platform <paramref name="dbPlatform"/>.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public string[] GetUpgradeResources(IDbPlatform dbPlatform, string name, long version)
        {
            var resourceFileName = string.Format("{0}.sql", name);
            var alias = DbPlatformUtil.GetDbPlatformAlias(dbPlatform);
            var potentialPaths = new[]
                {
                    Path.Combine(Path.Combine(baseDirectory, Path.Combine(alias, version.ToString())), resourceFileName),
                    Path.Combine(Path.Combine(baseDirectory, Path.Combine("shared", version.ToString())), resourceFileName)
                };
            
            var resourceFilePath = potentialPaths.FirstOrDefault(File.Exists);

            if(string.IsNullOrEmpty(resourceFilePath)) return null;

            string nativeResource;
            using(var fileStream = File.Open(resourceFilePath, FileMode.Open, FileAccess.Read))
                using(var streamReader = new StreamReader(fileStream, true))
                    nativeResource = streamReader.ReadToEnd();

            var goRegex = new Regex(@"^\s*go\s*", RegexOptions.Multiline);

            return goRegex.Split(nativeResource);
        }

        /// <summary>
        /// Returns an array of <see cref="string"/> object which contains a Native SQL resource <paramref name="name"/>
        /// to be executed while downgrading from version <paramref name="version"/> 
        /// for platform <paramref name="dbPlatform"/>.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public string[] GetDowngradeResources(IDbPlatform dbPlatform, string name, long version)
        {
            return GetUpgradeResources(dbPlatform, name, version);
        }
        #endregion
    }
}
