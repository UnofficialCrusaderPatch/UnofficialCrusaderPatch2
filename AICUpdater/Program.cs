﻿using System;
using System.IO;
using System.Linq;

namespace AICUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "AIC-Updater";
            var files = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.aic");
            if (files.Count() == 0)
            {
                Console.WriteLine("No .aic files found!");
            }
            else
            {
                Console.WriteLine("The property names of the following .aic files will be updated.");
                Console.WriteLine("Any custom comments will be lost in the process!");
                Console.WriteLine();
                foreach (string file in files)
                {
                    Console.WriteLine(Path.GetFileName(file));
                }
                Console.WriteLine();
                Console.WriteLine("Press ENTER to continue.");
                Console.ReadLine();

                foreach (string file in files)
                {
                    string newfile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".json");
                    try
                    {
                        string result = AICUpdaterHelper.Convert(file, newfile);
                        File.WriteAllText(newfile, Format(result));
                    }
                    catch (Exception e)
                    {
                        File.AppendAllText("AICLoading.log", e.ToString());
                    }
                }
                Console.WriteLine("Done.");
            }
            Console.ReadLine();
        }

        private static String Format(String aicJson)
        {
            return aicJson.Replace(",\"", ",\n\t\"").Replace("{", "{\n\t").Replace("}", "\n}");
        }
    }
}
