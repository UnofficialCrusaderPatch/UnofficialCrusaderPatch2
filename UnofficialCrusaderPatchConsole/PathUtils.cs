﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCP
{
    class PathUtils
    {
        static Dictionary<String, String> installPaths;

        static PathUtils()
        {
            string aicPath = "aic";
            string aivPath = "aiv";
            string mapsPath = "maps";
            string mapsExtremePath = "mapsExtreme";

            installPaths = new Dictionary<string, string>();
            installPaths.Add("aic", aicPath);
            installPaths.Add("aiv", aivPath);
            installPaths.Add("maps", mapsPath);
            installPaths.Add("mapsExtreme", mapsExtremePath);
        }

        public static string Get(string path)
        {
            String folder;
            installPaths.TryGetValue(path, out folder);
            return folder;
        }

        public static bool CrusaderExists(string path)
        {
            const string crusaderPath = "Stronghold Crusader.exe";
            const string extremePath = "Stronghold_Crusader_Extreme.exe";
            return File.Exists(Path.Combine(path, crusaderPath)) || File.Exists(Path.Combine(path, extremePath));
        }
    }
}
