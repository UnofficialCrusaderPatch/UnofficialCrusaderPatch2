using System;
using System.IO;
using UCP.Patching;
using UCP.Startup;

namespace UCP
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Configuration.Load();
            StartTroopChange.Load();
            ResourceChange.Load();
            Version.AddExternalChanges();
            ResolvePath();
            ResolveArgs(args);
            SilentInstall();
        }

        private static void ResolveArgs(String[] args)
        {

            Func<String, String, bool, bool, bool> fileTransfer = FileUtils.Transfer;

            bool silent = false;
            foreach (String arg in args)
            {
                if (arg == "--no-output")
                {
                    silent = true;
                }
            }

            foreach (String arg in args)
            {
                if (arg == "--no-output")
                {
                    continue;
                }

                if (!arg.StartsWith("--") || !arg.Contains("="))
                {
                    Console.WriteLine("Install failed. Invalid arguments provided.");
                    return;
                }

                if (arg.Contains("aic"))
                {
                    continue;
                }
                String srcPath   = arg.Split('=')[1];
                String rawOpt    = arg.Split('=')[0].Substring(2);
                bool   overwrite = rawOpt.Split('-').Length > 1
                                && rawOpt.Split('-')[1].ToLower().Contains("overwrite");
                String opt       = rawOpt.Split('-')[0];

                fileTransfer(Path.Combine(Environment.CurrentDirectory, srcPath), Path.GetFullPath(Path.Combine(Configuration.Path, PathUtils.Get(opt))), overwrite, !silent);
            }
        }

        private static void ResolvePath()
        {
            if (Patcher.CrusaderExists(Configuration.Path))
            {
                return;
            }

            if (Patcher.CrusaderExists(Environment.CurrentDirectory))
            {
                Configuration.Path = Environment.CurrentDirectory;
            }
            else if (Patcher.CrusaderExists(Path.Combine(Environment.CurrentDirectory, "..\\")))
            {
                Configuration.Path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\"));
            }
        }

        private static void SilentInstall()
        {
            _ = Version.Changes.Contains(null);
            Patcher.Install(Configuration.Path, null, false, false);
            Console.WriteLine("UCP successfully installed");
            Console.WriteLine("Path to Stronghold Crusader is:" + Configuration.Path);
        }
    }
}
