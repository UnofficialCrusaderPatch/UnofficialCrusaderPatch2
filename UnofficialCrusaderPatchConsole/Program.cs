using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using UCP.API;
using UCP.Model;

namespace UCP
{
    static class Program
    {
        public static readonly JsonSerializer Writer = new JsonSerializer()
        {
            Formatting = Formatting.Indented
        };
        static readonly List<string> VALID_ARGS = new List<string>() { "--aiv", "--aiv-overwrite", "--no-output", "--cfg", "--path" };
        static void Main(string[] args)
        {
            Dictionary<string, object> preferences = Resolver.GetExistingOrWriteEmptyPreference();
            Dictionary<string, string> resolvedArgsDictionary = ResolveArgs(args.ToList());
            UCPConfig ucpConfig = null;

            if (resolvedArgsDictionary.TryGetValue("cfgPath", out string cfgPath))
            {
                using (StreamReader file = File.OpenText(cfgPath))
                {
                    if (cfgPath.EndsWith(".json"))
                    {
                        try
                        {
                            ucpConfig = (UCPConfig)Writer.Deserialize(file, typeof(UCPConfig));
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Failed to parse JSON");
                        }
                    }
                    else if (cfgPath.EndsWith(".cfg"))
                    {
                        try
                        {
                            ucpConfig = Resolver.GetUCPConfigFromUncovertedCfg(cfgPath);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Failed to parse " + cfgPath);
                        }
                    }
                }
            }
            else if (File.Exists("ucp.json"))
            {
                using (StreamReader file = File.OpenText("ucp.json"))
                {
                    try
                    {
                        ucpConfig = (UCPConfig)Writer.Deserialize(file, typeof(UCPConfig));
                    } catch (Exception)
                    {
                        Console.WriteLine("Failed to parse JSON");
                    }
                }
            }
            else
            {
                try
                {
                    ucpConfig = Resolver.GetUCPConfigFromUncovertedCfg("ucp.cfg");
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to install");
                }
            }

            if (ucpConfig == null)
            {
                Console.WriteLine("Failed to read configuration file");
                return;
            }

            string resolvedPath = null;
            if (resolvedArgsDictionary.ContainsKey("path"))
            {
                resolvedPath = resolvedArgsDictionary["path"];
            }
            else if (preferences.ContainsKey("preferPathInConfig") && bool.Parse(preferences["preferPathInConfig"].ToString()) && ucpConfig.Path != null && Resolver.VerifyHash(ucpConfig.hash) && Resolver.isValidSHCPath((ucpConfig.Path)))
            {
                resolvedPath = ucpConfig.Path;
            }
            else if ((!preferences.ContainsKey("preferPathInConfig") || preferences.ContainsKey("preferPathInConfig") && !bool.Parse(preferences["preferPathInConfig"].ToString())))
            {
                if (preferences.ContainsKey("path") && preferences["path"] != null)
                {
                    resolvedPath = preferences["path"].ToString();
                }
            }

            if (resolvedPath != null)
            {
                ModAPIContract.Install(ucpConfig.withPath(resolvedPath), resolvedArgsDictionary.ContainsKey("aivOverridePath") && bool.Parse(resolvedArgsDictionary["aivOverridePath"]));
            }
        }

        static Dictionary<string, string> ResolveArgs(List<string> args)
        {
            Dictionary<string, string> resolvedArgsDictionary = new Dictionary<string, string>();
            if (args.Exists(arg => !VALID_ARGS.Contains(arg.Split('=')[0]) || arg.Count(c => c == '=') > 1))
            {
                Console.WriteLine("Install failed. Invalid arguments provided:");
                Console.WriteLine(String.Join(",", args.Where(arg => !VALID_ARGS.Contains(arg.Split('=')[0]) || arg.Count(c => c == '=') > 1).Select(x => x.Split('=')[0])));
            }

            bool silent = args.Contains("--no-output");
            bool overwrite = args.Contains("--aiv-overwrite");

            string path = null;
            string pathArg = args.SingleOrDefault(x => x.StartsWith("--path") && x.Count(c => c == '=') == 1);
            if (pathArg != null)
            {
                path = pathArg.Split('=')[1];
                resolvedArgsDictionary.Add("path", path);
                if (!silent)
                {
                    Console.WriteLine(pathArg.Substring(2));
                }
            }

            string cfgArg = args.SingleOrDefault(x => x.StartsWith("--cfg") && x.Count(c => c == '=') == 1);
            if (cfgArg != null)
            {
                string cfgPath = cfgArg.Split('=')[1];
                resolvedArgsDictionary.Add("cfgPath", cfgPath);
                if (!silent)
                {
                    Console.WriteLine(cfgArg.Substring(2));
                }
            }

            string aivArg = args.SingleOrDefault(x => x.StartsWith("--aiv") && x.Count(c => c == '=') == 1);
            if (aivArg != null && path != null)
            {
                string aivPath = aivArg.Split('=')[1];
                if (overwrite)
                {
                    resolvedArgsDictionary.Add("aivOverwritePath", aivPath);
                }
                else
                {
                    resolvedArgsDictionary.Add("aivPath", aivPath);
                }
                if (!silent)
                {
                    Console.WriteLine(aivArg.Substring(2));
                }
            }

            return resolvedArgsDictionary;
        }
    }
}
