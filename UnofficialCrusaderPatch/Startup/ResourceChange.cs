using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using UCP.Patching;

namespace UCP.Startup
{
    public class ResourceChange : Change
    {
        const int normalOffset = 0;
        const int crusaderOffset = 0x64;
        const int deathmatchOffset = 0xc8;

        const int goldArrayLength = 0x28;
        const int aiGoldOffset = 0x10; 
        const int fairnessLevels = 5;

        static ResourceChange activeChange = null;

        public static List<Change> changes = new List<Change>();
        public static TreeView View;
        private string description;

        public ResourceChange(string title, bool enabledDefault = false, bool isIntern = false)
            : base(title, ChangeType.Resource, enabledDefault, false)
        {
            this.NoLocalization = true;
        }

        public override void InitUI()
        {
            Localization.Add(this.TitleIdent + "_descr", this.description);
            base.InitUI();
            if (this.IsChecked)
                activeChange = this;
        }

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Checked(sender, e);

            if (activeChange != null)
                activeChange.IsChecked = false;

            activeChange = this;
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Unchecked(sender, e);

            if (activeChange == this)
                activeChange = null;
        }

        static ResourceChange()
        {
            Load();
        }
        

        public static void Load()
        {
            List<String> exceptionList = new List<string>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "resources", "goods"), "*.json", SearchOption.TopDirectoryOnly))
            {
                StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8);
                string resourceText = reader.ReadToEnd();
                reader.Close();

                Dictionary<String, Dictionary<String, Object>> resourceConfig = serializer.Deserialize<Dictionary<String, Dictionary<String, Object>>>(resourceText);

                try
                {
                    string description = GetLocalizedDescription(file, resourceConfig);
                    ResourceChange change = new ResourceChange(Path.GetFileNameWithoutExtension(file).Replace(" ", ""), false)
                        {
                            CreateResourceHeader(Path.GetFileNameWithoutExtension(file).Replace(" ", ""), resourceConfig),
                        };
                    change.description = description;
                    changes.Add(change);
                }
                    catch (Exception)
                {
                    exceptionList.Add("Error loading " + Path.GetFileNameWithoutExtension(file));
                }
            }

            if (exceptionList.Count > 0)
            {
                Debug.Show(String.Join(",\n", exceptionList));
            }
        }

        public static void Refresh(object sender, RoutedEventArgs args)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                ((TreeView)((Grid)((Button)sender).Parent).Children[0]).Items.Remove(changes.ElementAt(i).UIElement);
                Localization.Remove(changes.ElementAt(i).TitleIdent + "_descr");
            }
            changes.Clear();
            Load();
        }

        static byte[] ParseResources(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            Type StartingResourceType = typeof(StartingResource);
            byte[] config = new byte[0x124];
            foreach (int field in Enum.GetValues(StartingResourceType))
            {
                try
                {
                    AssignBytes("normal", normalOffset, field, resourceConfig, config);
                    AssignBytes("crusader", crusaderOffset, field, resourceConfig, config);
                    AssignBytes("deathmatch", deathmatchOffset, field, resourceConfig, config);
                }
                catch (KeyNotFoundException) { }
            }
            return config;
        }

        static byte[] ParseGold(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            dynamic normalGold = resourceConfig["normal"]["gold"];
            dynamic crusaderGold = resourceConfig["crusader"]["gold"];
            dynamic deathGold = resourceConfig["deathmatch"]["gold"];

            byte[] config = new byte[0x78];
            for (int lvl = 0; lvl < fairnessLevels; lvl++)
            {
                try
                {   
                    byte[] normalHumanGold = BitConverter.GetBytes(normalGold["human"][lvl]);
                    byte[] normalAIGold = BitConverter.GetBytes(normalGold["ai"][lvl]);

                    
                    byte[] crusaderHumanGold = BitConverter.GetBytes(crusaderGold["human"][lvl]);
                    byte[] crusaderAIGold = BitConverter.GetBytes(crusaderGold["ai"][lvl]);

                    
                    byte[] deathHumanGold = BitConverter.GetBytes(deathGold["human"][lvl]);
                    byte[] deathAIGold = BitConverter.GetBytes(deathGold["ai"][lvl]);

                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4] = normalHumanGold[i];
                        config[i + (lvl * 2 + 1) * 4] = normalAIGold[i];

                        config[i + (lvl * 2) * 4 + goldArrayLength] = crusaderHumanGold[i];
                        config[i + (lvl * 2 + 1) * 4 + goldArrayLength] = crusaderAIGold[i];

                        config[i + (lvl * 2) * 4 + goldArrayLength * 2] = deathHumanGold[i];
                        config[i + (lvl * 2 + 1) * 4 + goldArrayLength * 2] = deathAIGold[i];
                    }
                }
                catch (KeyNotFoundException) { }
            }
            return config;
        }

        static void AssignBytes(String mode, int offset, int field, Dictionary<String, Dictionary<String, Object>> resourceConfig, byte[] config)
        {
            Type StartingResourceType = typeof(StartingResource);
            byte[] fieldValue = BitConverter.GetBytes(Convert.ToInt32(resourceConfig[mode][Enum.GetName(StartingResourceType, field)]));
            for (var i = 0; i < 4; i++)
            {
                config[field * 4 + offset + i] = fieldValue[i];
            }
        }

        static String GetLocalizedDescription(String file, Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            String description = file;
            string currentLang = Localization.Translations.ToArray()[Configuration.Language].Ident;
            try
            {
                description = resourceConfig["description"][currentLang].ToString();
            }
            catch (Exception)
            {
                foreach (var lang in Localization.Translations)
                {
                    try
                    {
                        description = resourceConfig["description"][lang.Ident].ToString();
                        break;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            if (!description.Equals(file))
            {
                description = description.Substring(0, Math.Min(description.Length, 100));
            }

            return description;
        }


        #region Binary Edit

        static DefaultHeader CreateResourceHeader(String file, Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            byte[] resources = ParseResources(resourceConfig);
            byte[] gold = ParseGold(resourceConfig);

            return new DefaultHeader(file, true)
                {
                    BinBytes.CreateEdit("s_resource", resources),
                    BinBytes.CreateEdit("s_gold", gold)
                };
        }

        #endregion

    }
}
