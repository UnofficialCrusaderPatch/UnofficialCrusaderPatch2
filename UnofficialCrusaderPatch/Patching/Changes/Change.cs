using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;

namespace UnofficialCrusaderPatch
{
    public class Change : IEnumerable<DefaultHeader>
    {
        string titleIdent;
        public string TitleIdent => titleIdent;
        public string GetTitle() { return Localization.Get(titleIdent); }

        ChangeType type;
        public ChangeType Type => type;

        bool exclusive, enabledDefault;
        List<DefaultHeader> headerList = new List<DefaultHeader>();

        public IEnumerator<DefaultHeader> GetEnumerator() => headerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Add(DefaultHeader header)
        {
            headerList.Add(header);
        }

        public Change(string titleIdent, ChangeType type, bool enabledDefault = true, bool exclusive = true)
        {
            this.type = type;
            this.titleIdent = titleIdent;
            this.exclusive = exclusive;
            this.enabledDefault = enabledDefault;
        }

        public void Activate(ChangeArgs args)
        {
            foreach (var header in headerList)
            {
                if (header.IsEnabled)
                    header.Activate(args);
            }
        }

        #region UI

        public bool IsChecked => headerList.Exists(h => h.IsEnabled);

        UIElement uiElement;
        public UIElement UIElement { get { return this.uiElement; } }

        CheckBox titleBox;

        public void InitUI()
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
                }
            };
            titleBox.Checked += (s, e) => SetChecked(true);
            titleBox.Unchecked += (s, e) => SetChecked(false);

            TreeViewItem tvi = new TreeViewItem()
            {
                IsExpanded = false,
                Focusable = false,
                Header = titleBox,
                MinHeight = 22,
            };

            Grid content = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200)),
                Width = 420,
                Margin = new Thickness(-18, 5, 0, 0),
                Focusable = false,
            };


            FillGrid(content);

            tvi.Items.Add(content);
            tvi.Items.Add(null); // spacing

            this.uiElement = tvi;

            this.titleBox.IsChecked = enabledDefault;
        }

        void SetChecked(bool check)
        {
            if (check)
            {
                if (!this.IsChecked)
                    headerList.ForEach(h => h.IsEnabled = h.DefaultIsEnabled);
            }
            else
            {
                if (this.IsChecked)
                    headerList.ForEach(h => h.IsEnabled = false);
            }
        }

        void FillGrid(Grid grid)
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
                TextReferencer.SetText(description, header.DescrIdent + "_descr");
                grid.Children.Add(description);
                height += description.MeasureHeight();


                if (i != headerList.Count - 1)
                    height += 22;
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

            bool isChecked = this.IsChecked;
            if (titleBox.IsChecked != isChecked)
                this.titleBox.IsChecked = this.IsChecked;
        }

        #endregion
    }
}
