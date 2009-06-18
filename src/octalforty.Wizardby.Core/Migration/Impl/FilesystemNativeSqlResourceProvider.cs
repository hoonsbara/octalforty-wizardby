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
        public TextReader GetUpgradeResource(IDbPlatform dbPlatform, int version)
        {
            throw new NotImplementedException();
        }

        public TextReader GetDowngradeResource(IDbPlatform dbPlatform, int version)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
