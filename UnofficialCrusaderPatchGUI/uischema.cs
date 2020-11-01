using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UCP.Data;

namespace UCP.Views.TabViews
{
    /// <summary>
    /// Interaktionslogik für Bugfixes.xaml
    /// </summary>
    public partial class Bugfixes : UserControl
    {
        public Bugfixes()
        {
            InitializeComponent();
            List<Dictionary<string, object>> bugfixConfig = new JavaScriptSerializer().Deserialize<List<Dictionary<string, object>>>(File.ReadAllText("Bugfix.json"));
            CreateUI(bugfixConfig);
            //this.DataContextChanged += DataContextChangedEvent;
        }

        private void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
        }

        private void CreateUI(List<Dictionary<string, object>> bugfixConfig)
        {
            foreach(Dictionary<string, object> modConfig in bugfixConfig)
            {
                string modIdentifier = (string)modConfig["Identifier"];
                Expander currentMod = new Expander();
                CheckBox currentHeader = new CheckBox();
                currentHeader.Content = modIdentifier;
                currentMod.Header = currentHeader;
                currentMod.Content = new StackPanel();

                ArrayList changeConfig;
                if (modConfig.ContainsKey("Changes"))
                {
                    changeConfig = (ArrayList)modConfig["Changes"];
                    foreach (Dictionary<string, object> config in changeConfig)
                    {
                        string changeIdentifier = (string)config["Identifier"];
                        Dictionary<string, object> controlConfig = (Dictionary<string, object>)config["Control"];
                        string controlType = (string)controlConfig["Type"];

                        switch (controlType)
                        {
                            case "Slider":
                                Slider currentSlider = new Slider();
                                currentSlider.Minimum = Decimal.ToDouble(((Decimal)controlConfig["Minimum"]));
                                currentSlider.Maximum = Decimal.ToDouble(((Decimal)controlConfig["Maximum"]));
                                currentSlider.Interval = (int)controlConfig["Interval"];
                                TextBlock currentText = new TextBlock();
                                currentText.Text = changeIdentifier;
                                ((StackPanel)currentMod.Content).Children.Add(currentSlider);
                                ((StackPanel)currentMod.Content).Children.Add(currentText);
                                break;
                            case "CheckBox":
                                CheckBox currentCheckBox = new CheckBox();
                                currentCheckBox.Content = changeIdentifier;
                                ((StackPanel)currentMod.Content).Children.Add(currentCheckBox);
                                break;
                            case "Color":
                                Grid currentGrid = new Grid();
                                ArrayList colorList = (ArrayList)controlConfig["ColorList"];
                                foreach(string color in colorList)
                                {
                                    Button currentButton = new Button();
                                    currentButton.Margin = new Thickness(0, 0, 10, 0);
                                    currentButton.Width = 30;
                                    currentButton.Height = 30;
                                    currentButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                                    currentGrid.Children.Add(currentButton);
                                }
                                ((StackPanel)currentMod.Content).Children.Add(currentGrid);
                                break;
                            default:
                                throw new Exception(controlType);
                        }
                    }
                }
                this.BugfixesStackpanel.Children.Add(currentMod);
                //this.BugfixesStackpanel.Children.Add(currentMod);
            }
        }
    }
}
