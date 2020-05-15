using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UCP.Patching;

namespace UCP
{
    public static class Utility
    {
        public enum Languages { Deutsch, English, Polski, Русский  };
        public static void SelectCulture(Languages language)
        {
            var dictionaryList = Application.Current.Resources.MergedDictionaries;
            var resourceDictionary = dictionaryList.FirstOrDefault(d => d.Contains(language.ToString()) == true);
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }

        public static String GetText(String key)
        {
            var dummy = Application.Current.Resources[key];
            if (dummy == null) return "";
            return dummy.ToString();
        }

      

        public static String CheckCrusaderPath()
        {
            if (!Directory.Exists(Configuration.Path))
            {
                // check if we can already find the steam path
                const string key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 40970";
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey(key, false);
                if (myKey != null && myKey.GetValue("InstallLocation") is string path
                    && !string.IsNullOrWhiteSpace(path) && Patcher.CrusaderExists(path))
                {
                    return path;
                }
                else if (Patcher.CrusaderExists(Environment.CurrentDirectory))
                {
                    return Environment.CurrentDirectory;
                }
            }

            return Configuration.Path;

        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;
            T foundChild = null;
            var children = LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>();
            foreach (var child in children)
            {
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);
                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }
        
            return foundChild;
        }
    }


}
