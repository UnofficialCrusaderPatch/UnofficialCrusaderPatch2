using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UCP.Patching
{
    public class Change : IEnumerable<DefaultHeader>
    {
        public bool NoLocalization = false;

        string titleIdent;
        public string TitleIdent => titleIdent;
        public string GetTitle() { return NoLocalization ? titleIdent : Localization.Get(titleIdent); }

        ChangeType type;
        public ChangeType Type => type;

        bool exclusive, enabledDefault;
        public bool EnabledDefault => enabledDefault;
        List<DefaultHeader> headerList = new List<DefaultHeader>();

        public IEnumerator<DefaultHeader> GetEnumerator() => headerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Add(DefaultHeader header)
        {
            headerList.Add(header);
            header.SetParent(this);
        }

        public Change(string titleIdent, ChangeType type, bool enabledDefault = true, bool exclusive = true)
        {
            this.type = type;
            this.titleIdent = titleIdent;
            this.exclusive = exclusive;
            this.enabledDefault = enabledDefault;
        }

        public virtual void Activate(ChangeArgs args)
        {
            foreach (var header in headerList)
            {
                if (header.IsEnabled)
                    header.Activate(args);
            }
        }

        public override string ToString()
        {
            string str = TitleIdent + "={ ";
            foreach (DefaultHeader h in headerList)
            {
                str += h.ToString();
            }
            str += "}";
            return str;
        }

        #region UI

        public void SetUIEnabled(bool enabled)
        {
            titleBox.IsEnabled = enabled;
            headerList.ForEach(h => h.SetUIEnabled(enabled));
        }

        public bool IsChecked
        {
            get { return headerList.Exists(h => h.IsEnabled); }
            set { titleBox.IsChecked = value; }
        }

        protected UIElement uiElement;
        public UIElement UIElement { get { return this.uiElement; } }

        protected CheckBox titleBox;
        protected Grid grid;

        public virtual void InitUI()
        {
            this.titleBox = new CheckBox()
            {
                Content = new TextBlock()
                {
                    Text = this.GetTitle(),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, -1, 0, 0),
                    FontSize = 14,
                    Width = 400,
                },
                IsChecked = headerList.Exists(h => h.IsEnabled),
            };

            TreeViewItem tvi = new TreeViewItem()
            {
                IsExpanded = false,
                Focusable = false,
                Header = titleBox,
                MinHeight = 22,
            };

            if (headerList.Exists(h => h is ColorHeader))
            {
                titleBox.IsEnabled = false;
                tvi.MouseDown += (s, e) => tvi.IsExpanded = !tvi.IsExpanded;
            }
            else
            {
                titleBox.Checked += TitleBox_Checked;
                titleBox.Unchecked += TitleBox_Unchecked;
            }

            grid = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200)),
                Width = 420,
                Margin = new Thickness(-18, 5, 0, 0),
                Focusable = false,
            };
            
            FillGrid(grid);

            tvi.Items.Add(grid);
            tvi.Items.Add(null); // spacing

            this.uiElement = tvi;
        }

        protected virtual void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            headerList.ForEach(h => h.IsEnabled = false);

            Configuration.Save(this.titleIdent);
        }

        bool noCheck = false;
        protected virtual void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            if (noCheck) return;
            headerList.ForEach(h => h.IsEnabled = h.DefaultIsEnabled);
            noCheck = true;
            titleBox.IsChecked = this.IsChecked;
            noCheck = false;

            Configuration.Save(this.titleIdent);
        }

        protected void FillGrid(Grid grid)
        {
            bool singleDefault = headerList.Count == 1 && headerList[0].GetType() == typeof(DefaultHeader);

            double height = 5;
            for (int i = 0; i < headerList.Count; i++)
            {
                var header = headerList[i];

                if (!singleDefault)
                {
                    header.OnEnabledChange += Header_OnEnable;

                    // ui element
                    var uiElement = header.InitUI(headerList.Count > 1);
                    uiElement.HorizontalAlignment = HorizontalAlignment.Left;
                    uiElement.VerticalAlignment = VerticalAlignment.Top;
                    uiElement.Margin = new Thickness(6, height, 0, 0);
                    height += uiElement.Height + 5;

                    grid.Children.Add(uiElement);
                }
                
                string headerDescr = header.NoLocalization ? header.DescrIdent : Localization.Get(header.DescrIdent + "_descr");
                if (!string.IsNullOrWhiteSpace(headerDescr))
                {
                    // Description
                    TextBlock description = new TextBlock()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(6, height, 0, 0),
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 13,
                        Width = grid.Width - 12,
                    };
                    
                    TextReferencer.SetText(description, headerDescr);
                    grid.Children.Add(description);
                    height += description.MeasureHeight();

                    if (i != headerList.Count - 1)
                        height += 22;
                }
            }
            grid.Height = height + 10;
        }
        
        void Header_OnEnable(DefaultHeader header, bool enabled)
        {
            if (this.exclusive && enabled)
            {
                foreach (DefaultHeader h in headerList)
                    if (h != header)
                        h.IsEnabled = false;
            }

            bool newChecked = this.IsChecked;
            if (this.titleBox.IsChecked != newChecked)
            {
                noCheck = true;
                this.titleBox.IsChecked = newChecked;
                noCheck = false;
            }
        }

        #endregion
    }
}
