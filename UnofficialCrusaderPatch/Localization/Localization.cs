using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace UCP
{
    public static class Localization
    {
        public class Language
        {
            string name;
            public string Name => name;

            string ident;
            public string Ident => ident;

            string culture;
            public string Culture => culture;

            public Language(string name, string ident, string culture)
            {
                this.name = name;
                this.ident = ident;
                this.culture = culture;
            }
        }

        static int[] loadOrder = { 1, 0, 2, 3, 4};
        public static IEnumerable<int> IndexLoadOrder => loadOrder;

        static List<Language> translations = new List<Language>()
        {
            new Language("Deutsch", "German", "de"),
            new Language("English", "English", "en"),
            new Language("Polski", "Polish", "pl"),
            new Language("Русский", "Russian", "ru"),
            new Language("中文", "Chinese", "ch"),
            new Language("Magyar", "Hungarian", "hu")
        };
        public static IEnumerable<Language> Translations => translations;
        public static int GetLangByCulture(string culture)
        {
            return translations.FindIndex(l => l.Culture == culture);
        }


        static Dictionary<string, string> localStrs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static void Add(string identifier, string text)
        {
            if (localStrs.ContainsKey(identifier.ToLower()))
            {
                localStrs[identifier.ToLower()] = text;
            } 
            else
            {
                localStrs.Add(identifier.ToLower(), text);
            }
        }

        public static void Remove(string identifier)
        {
            if (localStrs.ContainsKey(identifier.ToLower()))
            {
                localStrs.Remove(identifier.ToLower());
            }
        }

        public static string Get(string identifier)
        {
            if (localStrs.TryGetValue(identifier.ToLower(), out string text))
            {
                return text;
            }
            return string.Format("{{Unknown Identifier: {0}}}", identifier.ToLower());

        }

        static int langIndex;
        public static int LanguageIndex => langIndex;

        public static void Load(int index)
        {
            try
            {
                langIndex = index;

                string path = string.Format("UCP.Localization.{0}.json", translations[index].Ident);
                Assembly asm = Assembly.GetExecutingAssembly();
                using (var s = asm.GetManifestResourceStream(path))
                using (StreamReader r = new StreamReader(s, Encoding.UTF8))
                {
                    localStrs.Clear();
                    localStrs = new JavaScriptSerializer().Deserialize<Dictionary<String, String>>(r.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.Error(e);
            }
        }
    }
}
