using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace UnofficialCrusaderPatch
{
    static class Debug
    {
        public static void Show(string message, string title = "Info")
        {
            MessageBox.Show(message, title);
        }

        public static void Error(string message)
        {
            Show(message, Localization.Get("ui_error"));
        }

        public static void Error(Exception e)
        {
            string message = e.ToString();
            File.AppendAllText("icp_error_dump.txt", message + "\n\n\n");
            Error(message);
        }
    }
}
