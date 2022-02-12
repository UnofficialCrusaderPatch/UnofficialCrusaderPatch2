using System;
using System.IO;
using UCP.Model;
using UCP.Patching;
using static UCP.API.Startup;

namespace UCP
{
    class Program
    {
        static void Main(string[] args)
        {
            UCPConfig config = ResolveConfig();
            string resolvedPath = ResolvePath(config);
            config.withPath(resolvedPath);
            ResolveArgs(args, resolvedPath);
            SilentInstall(config);
        }

        // Parse user config file
        static UCPConfig ResolveConfig()
        {
            UCPConfig config = new UCPConfig();
            return config;
        }

        static void ResolveArgs(string[] args, string configPath)
        {

            Func<string, string, bool, bool, bool> fileTransfer = (src, dest, overwrite, log) =>
            {
                return FileUtils.Transfer(src, dest, overwrite, log);
            };

            bool silent = false;
            foreach (string arg in args)
            {
                if (arg == "--no-output")
                {
                    silent = true;
                }
            }

            foreach (string arg in args)
            {
                if (arg == "--no-output")
                {
                    continue;
                } else if (!arg.StartsWith("--") || !arg.Contains("="))
                {
                    Console.WriteLine("Install failed. Invalid arguments provided.");
                    return;
                } else if (arg.Contains("aic"))
                {
                    continue;
                }
                string srcPath = arg.Split('=')[1];
                string rawOpt = arg.Split('=')[0].Substring(2);
                bool overwrite = false;
                if (rawOpt.Split('-').Length > 1 && rawOpt.Split('-')[1].ToLower().Contains("overwrite"))
                {
                    overwrite = true;
                }
                string opt = rawOpt.Split('-')[0];

                fileTransfer(Path.Combine(Environment.CurrentDirectory, srcPath), Path.GetFullPath(Path.Combine(configPath, PathUtils.Get(opt))), overwrite, !silent);   
            }
        }

        static string ResolvePath(UCPConfig config)
        {
            string resolvedPath = config.Path;
            if (!PathUtils.CrusaderExists(resolvedPath))
            {
                if (PathUtils.CrusaderExists(Environment.CurrentDirectory))
                {
                    resolvedPath = Environment.CurrentDirectory;
                }
                else if (PathUtils.CrusaderExists(Path.Combine(Environment.CurrentDirectory, "..\\")))
                {
                    resolvedPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\"));
                }
            }
            return resolvedPath;
        }

        static void SilentInstall(UCPConfig config)
        {
            Install(config, false, false);
            Console.WriteLine("UCP successfully installed");
            Console.WriteLine("Path to Stronghold Crusader is:" + config.Path);
        }
    }
}
