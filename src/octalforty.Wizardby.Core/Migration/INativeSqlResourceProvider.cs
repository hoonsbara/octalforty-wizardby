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
        /// Returns a <see cref="TextReader"/> object which contains a Native SQL resource <paramref name="name"/>
        /// to be executed while upgrading to version <paramref name="version"/> 
        /// for platform <paramref name="dbPlatform"/>.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        TextReader GetUpgradeResource(IDbPlatform dbPlatform, string name, int version);

        /// <summary>
        /// Returns a <see cref="TextReader"/> object which contains a Native SQL resource <paramref name="name"/>
        /// to be executed while downgrading from version <paramref name="version"/> 
        /// for platform <paramref name="dbPlatform"/>.
        /// </summary>
        /// <param name="dbPlatform"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        TextReader GetDowngradeResource(IDbPlatform dbPlatform, string name, int version);
    }
}
