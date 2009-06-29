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

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Core.Migration
{
    /// <summary>
    /// Represents an provider which abstracts access to Native SQL resources
    /// to be executed as a part of migration process for <c>execute native-sql</c>.
    /// </summary>
    public interface INativeSqlResourceProvider
    {
        /// <summary>
        /// Returns an array of <see cref="string"/> object which contains a Native SQL resource <paramref name="name"/>
        /// to be executed while upgrading to version <paramref name="version"/> 
        /// for platform <paramref name="dbPlatform"/>.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        string[] GetUpgradeResources(IDbPlatform dbPlatform, string name, long version);

        /// <summary>
        /// Returns an array of <see cref="string"/> object which contains a Native SQL resource <paramref name="name"/>
        /// to be executed while downgrading from version <paramref name="version"/> 
        /// for platform <paramref name="dbPlatform"/>.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        string[] GetDowngradeResources(IDbPlatform dbPlatform, string name, long version);
    }
}
