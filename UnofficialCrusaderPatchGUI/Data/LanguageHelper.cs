using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace UCP.Data
{
    public class LanguageHelper
    {
        public enum Languages { Deutsch, English, Polski, Русский };
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
    }
}
