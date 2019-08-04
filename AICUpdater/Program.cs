using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UCP.AICharacters;

namespace AICUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "AIC-Updater";
            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.aic");
            if (files.Count() == 0)
            {
                Console.WriteLine("No .aic files found!");
            }
            else
            {
                Console.WriteLine("The property names of the following .aic files will be updated.");
                Console.WriteLine("Any custom comments will be lost in the process!");
                Console.WriteLine();
                foreach(string file in files)
                {
                    Console.WriteLine(Path.GetFileName(file));
                }
                Console.WriteLine();
                Console.WriteLine("Press ENTER to continue.");
                Console.ReadLine();

                foreach (string file in files)
                {
                    AICCollection aicc;
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        aicc = new AICCollection(fs);
                    }


                    using (FileStream fs = new FileStream(file, FileMode.Create))
                    {
                        aicc.Write(fs);
                    }
                }
                Console.WriteLine("Done.");
            }
            Console.ReadLine();
        }
    }
}
