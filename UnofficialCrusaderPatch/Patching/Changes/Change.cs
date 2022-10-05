using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UCP.Patching
{
    public class Change : IEnumerable<DefaultHeader>
    {
        public  bool                                                                NoLocalization = false;
        private Func<Dictionary<string, Dictionary<string, object>>, DefaultHeader> multiChange;
        public  string                                                              TitleIdent { get; }

        public  string                                                              GetTitle() { return NoLocalization ? TitleIdent : Localization.Get(TitleIdent); }

        public  ChangeType Type { get; }

        private bool exclusive;
        public  bool EnabledDefault { get; }

        private List<DefaultHeader> headerList = new List<DefaultHeader>();

        public IEnumerator<DefaultHeader> GetEnumerator() => headerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(DefaultHeader header)
        {
            headerList.Add(header);
            header.SetParent(this);
        }

        public Change(string titleIdent, ChangeType type, bool enabledDefault = true, bool exclusive = true, Func<Dictionary<string, Dictionary<string, object>>, ParamHeader> multiChange = null)
        {
            Type = type;
            TitleIdent = titleIdent;
            this.exclusive = exclusive;
            EnabledDefault = enabledDefault;
            this.multiChange = multiChange;
        }

        public virtual void Activate(ChangeArgs args)
        {
            if (multiChange != null)
            {
                Dictionary<string, Dictionary<string, object>> parameters = headerList.ToDictionary(header => header.DescrIdent, header => new Dictionary<string, object> { { "isEnabled", header.IsEnabled }, { "value", (object)header is ValueHeader && header.IsEnabled ? (header as ValueHeader).Value : (object)header.IsEnabled } });
                multiChange(parameters).Activate(args);
                return;
            }

            foreach (DefaultHeader header in headerList.Where(header => header.IsEnabled))
            {
                header.Activate(args);
            }
        }

        public override string ToString()
        {
            string str = TitleIdent + "={ ";
            str =  headerList.Aggregate(str, (current, h) => current + h);
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
            set => titleBox.IsChecked = value;
        }

        protected UIElement uiElement;
        public    UIElement UIElement => uiElement;

        protected CheckBox titleBox;
        protected Grid grid;

        public virtual void InitUI()
        {
            titleBox = new CheckBox
                       {
                Content = new TextBlock
                          {
                    Text = GetTitle(),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, -1, 0, 0),
                    FontSize = 14,
                    Width = 400,
                },
                IsChecked = headerList.Exists(h => h.IsEnabled),
            };

            TreeViewItem tvi = new TreeViewItem
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

            grid = new Grid
                   {
                Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200)),
                Width = 420,
                Margin = new Thickness(-18, 5, 0, 0),
                Focusable = false,
            };
            
            FillGrid(grid);

            tvi.Items.Add(grid);
            tvi.Items.Add(null); // spacing

            uiElement = tvi;
        }

        protected virtual void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            headerList.ForEach(h => h.IsEnabled = false);

            Configuration.Save(TitleIdent);
        }

        private bool noCheck;
        protected virtual void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            if (noCheck)
            {
                return;
            }

            headerList.ForEach(h => h.IsEnabled = h.DefaultIsEnabled);
            noCheck = true;
            titleBox.IsChecked = IsChecked;
            noCheck = false;

            Configuration.Save(TitleIdent);
        }

        protected void FillGrid(Grid grid)
        {
            bool singleDefault = headerList.Count == 1 && headerList[0].GetType() == typeof(DefaultHeader);

            double height = 5;
            for (int i = 0; i < headerList.Count; i++)
            {
                DefaultHeader header = headerList[i];

                if (!singleDefault)
                {
                    header.OnEnabledChange += Header_OnEnable;

                    // ui element
                    FrameworkElement uiElement = header.InitUI(headerList.Count > 1);
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
                    TextBlock description = new TextBlock
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
                    {
                        height += 22;
                    }
                }
            }
            grid.Height = height + 10;
        }

        private void Header_OnEnable(DefaultHeader header, bool enabled)
        {
            if (exclusive && enabled)
            {
                foreach (DefaultHeader h in headerList)
                    if (h != header)
                    {
                        h.IsEnabled = false;
                    }
            }

            bool newChecked = IsChecked;
            if (titleBox.IsChecked != newChecked)
            {
                noCheck = true;
                titleBox.IsChecked = newChecked;
                noCheck = false;
            }
        }

        #endregion
    }
}
