using System.Linq;
using System.IO;

namespace octalforty.Wizardby.Tests.Util
{
    public static class PathUtil
    {
        public static string Combine(params string[] paths)
        {
            return paths.Aggregate(Path.Combine);
        }
    }
}
