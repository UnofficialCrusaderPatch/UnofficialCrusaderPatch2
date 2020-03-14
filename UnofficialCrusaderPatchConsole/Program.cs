using System;
using System.IO;
using UCP.Patching;

namespace UCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.Load();
            ResolvePath();
            ResolveArgs(args);
            SilentInstall();
        }

        static void ResolveArgs(String[] args)
        {

            Func<String, String, bool, bool, bool> fileTransfer = (src, dest, overwrite, log) =>
            {
                return FileUtils.Transfer(src, dest, overwrite, log);
            };

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
                } else if (!arg.StartsWith("--") || !arg.Contains("="))
                {
                    Console.WriteLine("Install failed. Invalid arguments provided.");
                    return;
                } else if (arg.Contains("aic"))
                {
                    continue;
                }
                String srcPath = arg.Split('=')[1];
                String rawOpt = arg.Split('=')[0].Substring(2);
                bool overwrite = false;
                if (rawOpt.Split('-').Length > 1 && rawOpt.Split('-')[1].ToLower().Contains("overwrite"))
                {
                    overwrite = true;
                }
                String opt = rawOpt.Split('-')[0];

                fileTransfer(Path.Combine(Environment.CurrentDirectory, srcPath), Path.GetFullPath(Path.Combine(Configuration.Path, PathUtils.Get(opt))), overwrite, !silent);   
            }
        }

        static void ResolvePath()
        {
            if (!Patcher.CrusaderExists(Configuration.Path))
            {
                if (Patcher.CrusaderExists(Environment.CurrentDirectory))
                {
                    Configuration.Path = Environment.CurrentDirectory;
                }
                else if (Patcher.CrusaderExists(Path.Combine(Environment.CurrentDirectory, "..\\")))
                {
                    Configuration.Path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\"));
                }
            }
        }

        static void SilentInstall()
        {
            Version.Changes.Contains(null);
            Patcher.Install(Configuration.Path, null);
            Console.WriteLine("UCP successfully installed");
            Console.WriteLine("Path to Stronghold Crusader is:" + Configuration.Path);
        }
    }
}
