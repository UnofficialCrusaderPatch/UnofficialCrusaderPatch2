using System;
using System.IO;
using UCP.Patching;
using UCP.Startup;

namespace UCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.Load();
            StartTroopChange.Load();
            ResourceChange.Load();
            Version.AddExternalChanges();
            ResolveLanguage(args);
            Localization.Load(Configuration.Language);
            ResolveArgs(args);
            SilentInstall();
        }

        static void ResolveLanguage(String[] args)
        {
            foreach (String arg in args)
            {
                if (arg.Contains("--language"))
                {
                    Int32.TryParse(arg.Split('=')[1].Trim(), out int parameter);
                    Configuration.Language = parameter;
                }
            }
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
                else if (arg.Contains("--path"))
                {
                    Configuration.Path = arg.Split('=')[1].Trim();
                }
            }

            if (!Patcher.CrusaderExists(Configuration.Path))
            {
                ResolvePath(silent);
            }

            foreach (String arg in args)
            {
                if (arg == "--no-output" || arg.Contains("--path") || arg.Contains("--language"))
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

        static void ResolvePath(bool silent = false)
        {
            if (Patcher.CrusaderExists(Environment.CurrentDirectory))
            {
                Configuration.Path = Environment.CurrentDirectory;
            }
            else if (Patcher.CrusaderExists(Path.Combine(Environment.CurrentDirectory, "..\\")))
            {
                Configuration.Path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\"));
            }
            else if (Patcher.CrusaderExists(Path.Combine(Environment.CurrentDirectory, "..\\..\\")))
            {
                Configuration.Path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\..\\"));
            }
            else
            {
                string input = "";
                while (!Patcher.CrusaderExists(input))
                {
                    Console.WriteLine(Localization.Get("ui_cli_ask_installation_folder"));
                    input = Console.ReadLine().Replace("\n", "");
                }
                Configuration.Path = input;
            }
        }

        static void SilentInstall()
        {
            Version.Changes.Contains(null);
            Patcher.Install(Configuration.Path, null, false, false);
            Console.WriteLine("UCP successfully installed");
            Console.WriteLine("Path to Stronghold Crusader is:" + Configuration.Path);
        }
    }
}
