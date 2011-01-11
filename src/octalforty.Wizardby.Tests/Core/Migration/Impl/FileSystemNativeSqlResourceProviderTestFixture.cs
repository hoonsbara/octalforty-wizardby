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

using NUnit.Framework;

using octalforty.Wizardby.Core.Migration;
using octalforty.Wizardby.Core.Migration.Impl;
using octalforty.Wizardby.Db.SqlServer2005;

namespace octalforty.Wizardby.Tests.Core.Migration.Impl
{
    [TestFixture()]
    public class FileSystemNativeSqlResourceProviderTestFixture
    {
        [Test()]
        public void GetUpgradeResources()
        {
            INativeSqlResourceProvider nativeSqlResourceProvider =
                new FileSystemNativeSqlResourceProvider(
                        Path.Combine(GetAssemblyLocation(Assembly.GetExecutingAssembly()), "Resources"));

            string[] upgradeResources =
                nativeSqlResourceProvider.GetUpgradeResources(new SqlServer2005Platform(), "Upgrade", 20090331140131);

            Assert.AreEqual(1, upgradeResources.Length);
        }

        [Test()]
        public void GetUpgradeResources2()
        {
            INativeSqlResourceProvider nativeSqlResourceProvider =
                new FileSystemNativeSqlResourceProvider(
                        Path.Combine(GetAssemblyLocation(Assembly.GetExecutingAssembly()), "Resources"));

            Assert.IsNull(
                nativeSqlResourceProvider.GetUpgradeResources(new SqlServer2005Platform(), "Upgrade", 321));
        }

        private static string GetAssemblyLocation(Assembly assembly)
        {
            return Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
        }
    }
}
