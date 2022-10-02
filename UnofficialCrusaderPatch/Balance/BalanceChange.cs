using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UCP.Patching;

namespace UCP.Balance
{
    public class BalanceChange : Change
    {
        string description;
        bool IsValid = true;

        public static BalanceChange activeChange = null;

        public static List<Change> changes = new List<Change>();
        public static TreeView View;

        private static string selectedChange = String.Empty;

        public BalanceChange(string title, bool enabledDefault = false, bool isIntern = false)
            : base("bal_" + title, ChangeType.Balance, enabledDefault, false)
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
            ((TextBlock)this.titleBox.Content).Text = this.TitleIdent.Substring(4);

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
            this.titleBox.Background = new SolidColorBrush(Colors.White);
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

        static BalanceChange() {}

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

        /// <summary>
        /// Load built-in vanilla starting balance file and user-provided JSON balance files located in resources\balances subfolder
        /// </summary>
        public static void Load()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "balances")))
            {
                return;
            }

            foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "resources", "balances"), "*.json", SearchOption.TopDirectoryOnly))
            {
                StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8);
                string balanceText = reader.ReadToEnd();
                reader.Close();

                BalanceConfig balanceConfig;
                try
                {
                    balanceConfig = serializer.Deserialize<BalanceConfig>(balanceText);
                }
                catch (Exception e)
                {
                    CreateNullChange(Path.GetFileNameWithoutExtension(file), "Invalid JSON detected");
                    continue;
                }

                try
                {
                    string description = GetLocalizedDescription(file, balanceConfig);
                    BalanceChange change = new BalanceChange(Path.GetFileNameWithoutExtension(file), false)
                        {
                            CreateBalanceHeader("bal_" + Path.GetFileNameWithoutExtension(file), balanceConfig),
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

            Version.RemoveChanges(ChangeType.Balance);
            Version.Changes.AddRange(changes);
        }

        static String GetLocalizedDescription(String file, BalanceConfig balanceConfig)
        {
            String description = file;
            string currentLang = Localization.Translations.ToArray()[Configuration.Language].Ident;
            try
            {
                description = balanceConfig.description[currentLang].ToString();
            }
            catch (Exception)
            {
                foreach (var lang in Localization.Translations)
                {
                    try
                    {
                        description = balanceConfig.description[lang.Ident].ToString();
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
            BalanceChange change = new BalanceChange(Path.GetFileNameWithoutExtension(file).Replace(" ", ""), false)
                        {
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

        static DefaultHeader CreateBalanceHeader(String file, BalanceConfig balanceConfig)
        {
            try
            {
                DefaultHeader header = new DefaultHeader(file, true);
                foreach (ChangeEdit edit in BalanceHelper.GetBinaryEdits(balanceConfig))
                {
                    header.Add(edit);
                }
                return header;
            }
            catch (Exception e)
            {
                throw new Exception("Errors found in " + file + ":\n" + e.Message);
            }
        }

        #endregion

    }
}
