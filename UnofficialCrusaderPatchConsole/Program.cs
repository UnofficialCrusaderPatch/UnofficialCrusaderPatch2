using System;
using UCP;
using UCP.Patching;

namespace UCP
{
    class Program
    {
        static void Main(string[] args)
        {
                SilentInstall();
        }

        static void SilentInstall()
        {
            Configuration.LoadGeneral();
            Configuration.LoadChanges();

            if (!Patcher.CrusaderExists(Configuration.Path) && Patcher.CrusaderExists(Environment.CurrentDirectory))
            {
                Configuration.Path = Environment.CurrentDirectory;
            }
            AICChange.LoadFiles();
            Configuration.LoadChanges();
            Version.Changes.Contains(null);
            Patcher.Install(Configuration.Path, null);
            Console.WriteLine("UCP successfully installed");
        }
    }
}
