using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        string description;
        bool IsValid = true;

        public static ResourceChange activeChange = null;

        public static List<Change> changes = new List<Change>();
        public static TreeView View;

        private static string selectedChange = String.Empty;
        private static object resourceBlockFile = "s_resource";
        private static object goldBlockFile = "s_gold";
        private static byte[] resourceBlock;
        private static byte[] goldBlock;

        public ResourceChange(string title, bool enabledDefault = false, bool isIntern = false)
            : base("res_" + title, ChangeType.Resource, enabledDefault, false)
        {
            this.NoLocalization = true;
        }

        public override void InitUI()
        {
            Localization.Add(this.TitleIdent + "_descr", this.description);
            base.InitUI();
            if (this.IsChecked)
            {
                activeChange = this;
            }
            ((TextBlock)this.titleBox.Content).Text = this.TitleIdent.Substring(4).StartsWith("UCP.") ? this.TitleIdent.Substring(4).Replace("UCP.", "") : this.TitleIdent.Substring(4);

            if (this.IsValid == false)
            {
                ((TextBlock)this.titleBox.Content).TextDecorations = TextDecorations.Strikethrough;
                this.titleBox.IsEnabled = false;
                this.titleBox.ToolTip = this.description;
                ((TextBlock)this.titleBox.Content).Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                this.titleBox.IsChecked = selectedChange.Equals(this.TitleIdent);
            }
            this.titleBox.Background = this.TitleIdent.Substring(4).StartsWith("UCP.") ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque);


            if (this.TitleIdent.Substring(4).StartsWith("UCP."))
            {
                Button exportButton = new Button()
                {
                    //Width = 40,
                    Height = 20,
                    Content = Localization.Get("ui_aicexport"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 5, 5),
                };
                exportButton.Click += (s, e) => this.ExportFile();
                grid.Height += 15;
                this.grid.Children.Add(exportButton);
            }
        }

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Checked(sender, e);

            if (activeChange != null)
                activeChange.IsChecked = false;

            selectedChange = this.TitleIdent;
            activeChange = this;
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Unchecked(sender, e);

            if (activeChange == this)
            {
                activeChange = null;
                selectedChange = String.Empty;
            }
        }

        private void ExportFile()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "goods", "exports", this.TitleIdent.Substring(4).Replace("UCP.", "")) + ".json";
            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            if (File.Exists(fileName))
            {
                File.Move(fileName, backupFileName);
            }
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "resources", "goods", "exports"));

            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(this.TitleIdent.Substring(4).Replace("UCP.", "UCP.Startup.Resources.Goods.") + ".json"), Encoding.UTF8))
            {
                File.WriteAllText(fileName, reader.ReadToEnd(), Encoding.UTF8);
            }

            Debug.Show(Localization.Get("ui_aicexport_success"), this.TitleIdent.Substring(4).Replace("UCP.", "") + ".json");
        }

        static ResourceChange()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            // check if code block file is there
            string file = string.Format("UCP.CodeBlocks.{0}.block", resourceBlockFile);
            if (!asm.GetManifestResourceNames().Contains(file))
                throw new Exception("MISSING BLOCK FILE " + file);

            // read code block file
            using (Stream stream = asm.GetManifestResourceStream(file))
                resourceBlock = new CodeBlox.CodeBlock(stream).Elements.ToArray().Select(x => x.Value).ToArray();

            // check if code block file is there
            file = string.Format("UCP.CodeBlocks.{0}.block", goldBlockFile);
            if (!asm.GetManifestResourceNames().Contains(file))
                throw new Exception("MISSING BLOCK FILE " + file);

            // read code block file
            using (Stream stream = asm.GetManifestResourceStream(file))
                goldBlock = new CodeBlox.CodeBlock(stream).Elements.ToArray().Select(x => x.Value).ToArray();
        }

        public static void LoadConfiguration(List<string> configuration = null)
        {
            if (configuration == null)
            {
                return;
            }

            foreach (string change in configuration)
            {
                string[] changeLine = change.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()).ToArray();
                if (changeLine.Length < 2)
                {
                    continue;
                }

                string changeKey = changeLine[0];
                string changeSetting = changeLine[1];

                bool selected = Regex.Replace(@"\s+", "", changeSetting.Split('=')[1]).Contains("True");
                if (selected == true)
                {
                    selectedChange = changeKey;
                }
            }
        }

        public static void Load()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            StreamReader vanilla = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.Startup.Resources.Goods.vanilla.json"), Encoding.UTF8);
            string vanillaText = vanilla.ReadToEnd();
            vanilla.Close();
            Dictionary<String, Dictionary<String, Object>> vanillaConfig = serializer.Deserialize<Dictionary<String, Dictionary<String, Object>>>(vanillaText);
            if (vanillaConfig != null)
            {
                string description = GetLocalizedDescription("UCP.vanilla", vanillaConfig);
                ResourceChange change = new ResourceChange("UCP.vanilla", false)
                        {
                            CreateResourceHeader("res_UCP.vanilla", vanillaConfig),
                        };
                change.description = description;
                changes.Add(change);
            }


            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "goods")))
            {
                return;
            }

            foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "resources", "goods"), "*.json", SearchOption.TopDirectoryOnly))
            {
                StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8);
                string resourceText = reader.ReadToEnd();
                reader.Close();

                Dictionary<String, Dictionary<String, Object>> resourceConfig;
                try
                {
                    resourceConfig = serializer.Deserialize<Dictionary<String, Dictionary<String, Object>>>(resourceText);
                }
                catch (Exception)
                {
                    CreateNullChange(Path.GetFileNameWithoutExtension(file), "Invalid JSON detected");
                    continue;
                }

                try
                {
                    string description = GetLocalizedDescription(file, resourceConfig);
                    ResourceChange change = new ResourceChange(Path.GetFileNameWithoutExtension(file), false)
                        {
                            CreateResourceHeader("res_" + Path.GetFileNameWithoutExtension(file), resourceConfig),
                        };
                    change.description = description;
                    changes.Add(change);
                }
                catch (Exception e)
                {
                    CreateNullChange(Path.GetFileNameWithoutExtension(file), e.Message);
                }
            }
        }

        public static void Refresh(object sender, RoutedEventArgs args)
        {
            changes.Clear();
            Load();

            Version.RemoveExternalChanges(ChangeType.AIV);
            Version.AdditionalExternalChanges.AddRange(changes);
        }

        static byte[] ParseResources(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            Type StartingResourceType = typeof(StartingResource);
            byte[] config = new byte[0x124];

            List<String> exceptions = new List<String>();
            foreach (int field in Enum.GetValues(StartingResourceType))
            {
                try
                {
                    AssignResourceBytes(normalOffset, field, resourceConfig["normal"], config);
                }
                catch (KeyNotFoundException)
                {
                    for (int i = 0; i < crusaderOffset; i++)
                    {
                        config[i] = resourceBlock[i];
                    }
                }
                catch (Exception)
                {
                    exceptions.Add("Invalid normal value for " + Enum.GetName(StartingResourceType, field));
                }

                try
                {
                    AssignResourceBytes(crusaderOffset, field, resourceConfig["crusader"], config);
                }
                catch (KeyNotFoundException)
                {
                    for (int i = crusaderOffset; i < deathmatchOffset; i++)
                    {
                        config[i] = resourceBlock[i];
                    }
                }
                catch (Exception)
                {
                    exceptions.Add("Invalid crusader value for " + Enum.GetName(StartingResourceType, field));
                }

                try
                {
                    AssignResourceBytes(deathmatchOffset, field, resourceConfig["deathmatch"], config);
                }
                catch (KeyNotFoundException)
                {
                    for (int i = deathmatchOffset; i < resourceBlock.Length; i++)
                    {
                        config[i] = resourceBlock[i];
                    }
                }
                catch (Exception)
                {
                    exceptions.Add("Invalid deathmatch value for " + Enum.GetName(StartingResourceType, field));
                }
            }

            if (exceptions.Count > 0)
            {
                throw new Exception(String.Join("\n", exceptions));
            }

            return config;
        }

        static byte[] ParseGold(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            byte[] config = new byte[0x78];

            List<String> exceptions = new List<String>();
            try
            {
                dynamic normalGold = resourceConfig["normal"]["gold"];
                AssignGoldBytes("normal", normalGold, 0, config);
            }
            catch (KeyNotFoundException)
            {
                for (int lvl = 0; lvl < fairnessLevels; lvl++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4] = goldBlock[i + (lvl * 2) * 4];
                        config[i + (lvl * 2 + 1)] = goldBlock[i + (lvl * 2 + 1) * 4];
                    }
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e.ToString());
            }


            try
            {
                dynamic crusaderGold = resourceConfig["crusader"]["gold"];
                AssignGoldBytes("crusader", crusaderGold, goldArrayLength, config);
            }
            catch (KeyNotFoundException)
            {
                for (int lvl = 0; lvl < fairnessLevels; lvl++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4 + goldArrayLength] = goldBlock[i + (lvl * 2) * 4 + goldArrayLength];
                        config[i + (lvl * 2 + 1) * 4 + goldArrayLength] = goldBlock[i + (lvl * 2 + 1) * 4 + goldArrayLength];
                    }
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e.ToString());
            }


            try
            {
                dynamic deathGold = resourceConfig["deathmatch"]["gold"];
                AssignGoldBytes("deathmatch", deathGold, goldArrayLength * 2, config);
            }
            catch (KeyNotFoundException)
            {
                for (int lvl = 0; lvl < fairnessLevels; lvl++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4 + goldArrayLength * 2] = goldBlock[i + (lvl * 2) * 4 + goldArrayLength * 2];
                        config[i + (lvl * 2 + 1) * 4 + goldArrayLength * 2] = goldBlock[i + (lvl * 2 + 1) * 4 + goldArrayLength * 2];
                    }
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e.ToString());
            }

            if (exceptions.Count > 0)
            {
                throw new Exception(String.Join("\n", exceptions));
            }
            return config;
        }

        static void AssignGoldBytes(String mode, dynamic goldDictionary, Int32 offset, byte[] config)
        {
            bool humanGoldException = false;
            bool aiGoldException = false;

            for (int lvl = 0; lvl < fairnessLevels; lvl++)
            {
                try
                {
                    byte[] humanGold = BitConverter.GetBytes(goldDictionary["human"][lvl]);
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4 + offset] = humanGold[i];
                    }
                }
                catch (Exception)
                {
                    humanGoldException = true;
                }

                try
                {
                    byte[] aiGold = BitConverter.GetBytes(goldDictionary["ai"][lvl]);
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2 + 1) * 4 + offset] = aiGold[i];
                    }
                }
                catch (Exception)
                {
                    aiGoldException = true;
                }
            }

            List<String> exceptions = new List<string>();
            if (humanGoldException == true)
            {
                exceptions.Add(mode + " mode human gold values incorrect");
            }
            if (aiGoldException == true)
            {
                exceptions.Add(mode + " mode AI gold values incorrect");
            }
            if (exceptions.Count > 0)
            {
                throw new Exception(String.Join("\n", exceptions));
            }
        }

        static void AssignResourceBytes(int offset, int field, Dictionary<String, Object> resourceConfig, byte[] config)
        {
            Type StartingResourceType = typeof(StartingResource);
            if (!resourceConfig.ContainsKey(Enum.GetName(StartingResourceType, field)))
            {
                return;
            }

            byte[] fieldValue = BitConverter.GetBytes(Convert.ToInt32(resourceConfig[Enum.GetName(StartingResourceType, field)]));
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
                description = description.Substring(0, Math.Min(description.Length, 1000));
            }

            return description;
        }


        static void CreateNullChange(string file, string message)
        {
            ResourceChange change = new ResourceChange(Path.GetFileNameWithoutExtension(file).Replace(" ", ""), false)
                        {
                            new DefaultHeader(file, true)
                            {
                                new BinaryEdit("s_resource")
                                {
                                    new BinSkip(0x124),
                                },
                                new BinaryEdit("s_gold")
                                {
                                    new BinSkip(0x58),
                                }
                            }
                        };
            change.description = message;
            change.IsValid = false;
            changes.Add(change);
        }


        #region Binary Edit

        internal static void DoChange(ChangeArgs args)
        {
            Change change = activeChange;
            if (!selectedChange.Equals(String.Empty))
            {
                if (activeChange == null)
                {
                    change = changes.Where(x => x.TitleIdent.Equals(selectedChange)).First();
                    foreach (var header in change)
                    {
                        header.Activate(args);
                    }
                    return;
                }
                foreach (var header in change)
                {
                    header.Activate(args);
                }
                return;
            }
        }

        static DefaultHeader CreateResourceHeader(String file, Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            try
            {
                byte[] resources = ParseResources(resourceConfig);
                byte[] gold = ParseGold(resourceConfig);

                return new DefaultHeader(file, true)
                {
                    BinBytes.CreateEdit("s_resource", resources),
                    BinBytes.CreateEdit("s_gold", gold)
                };
            }
            catch (Exception e)
            {
                throw new Exception("Errors found in " + file + ":\n" + e.Message);
            }
        }

        #endregion

    }
}
