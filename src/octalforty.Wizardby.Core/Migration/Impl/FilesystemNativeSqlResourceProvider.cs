using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using octalforty.Wizardby.Core.Db;

namespace octalforty.Wizardby.Core.Migration.Impl
{
    public class FilesystemNativeSqlResourceProvider : INativeSqlResourceProvider
    {
        #region INativeSqlResourceProvider Members
        /// <summary>
        /// Returns a <see cref="TextReader"/> object which contains a Native SQL resource <paramref name="name"/>
        /// to be executed while upgrading to version <paramref name="version"/> 
        /// for platform <paramref name="dbPlatform"/>.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public TextReader GetUpgradeResource(IDbPlatform dbPlatform, string name, int version)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a <see cref="TextReader"/> object which contains a Native SQL resource <paramref name="name"/>
        /// to be executed while downgrading from version <paramref name="version"/> 
        /// for platform <paramref name="dbPlatform"/>.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public TextReader GetDowngradeResource(IDbPlatform dbPlatform, string name, int version)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
