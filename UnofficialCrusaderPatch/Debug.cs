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
        public static void Error(string message, string title)
        {
            File.AppendAllText("icp_error_dump.txt", message + "\n\n\n");
            MessageBox.Show(message, title);
        }

        public static void Error(string message)
        {
            Error(message, Localization.Get("ui_error"));
        }

        public static void Error(Exception e)
        {
            Error(e.ToString());
        }

        public static void Show(string message, string title)
        {
            MessageBox.Show(message, title);
        }
    }
}
