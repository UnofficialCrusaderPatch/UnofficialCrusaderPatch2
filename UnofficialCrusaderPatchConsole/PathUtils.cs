using System;
using System.Collections.Generic;

namespace UCP
{
    class PathUtils
    {
        static Dictionary<String, String> installPaths;

        static PathUtils()
        {
            String aicPath = "aic";
            String aivPath = "aiv";
            String mapsPath = "maps";
            String mapsExtremePath = "mapsExtreme";

            installPaths = new Dictionary<string, string>();
            installPaths.Add("aic", aicPath);
            installPaths.Add("aiv", aivPath);
            installPaths.Add("maps", mapsPath);
            installPaths.Add("mapsExtreme", mapsExtremePath);
        }

        public static String Get(String path)
        {
            String folder;
            installPaths.TryGetValue(path, out folder);
            return folder;
        }
    }
}
