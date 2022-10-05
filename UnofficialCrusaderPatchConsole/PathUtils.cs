using System;
using System.Collections.Generic;

namespace UCP
{
    internal static class PathUtils
    {
        private static readonly Dictionary<String, String> InstallPaths;

        static PathUtils()
        {
            const string aicPath         = "aic";
            const string aivPath         = "aiv";
            const string mapsPath        = "maps";
            const string mapsExtremePath = "mapsExtreme";

            InstallPaths = new Dictionary<string, string>
                           {
                               { "aic", aicPath },
                               { "aiv", aivPath },
                               { "maps", mapsPath },
                               { "mapsExtreme", mapsExtremePath }
                           };
        }

        public static String Get(String path)
        {
            InstallPaths.TryGetValue(path, out string folder);
            return folder;
        }
    }
}
