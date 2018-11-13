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
    public enum ChangeType
    {
        Bugfix,
        AILords,
        Troops,
        Other,
    }

    public class Change : IEnumerable<ChangeElement>
    {
        string titleIdent;
        public string TitleIdent => titleIdent;
        public string GetTitle() { return Localization.Get(titleIdent); }

        ChangeType type;
        public ChangeType Type => type;

        List<ChangeHeader> headerList;

        public IEnumerator<ChangeElement> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        public void Add(ChangeElement edit)
        {
            if (edit is ChangeHeader header)
            {
                headerList.Add(header);
            }
            else
            {
                headerList.Last().Add((ChangeEdit)edit);
            }
        }

        int defaultActiveIndex;
        int activeHeaderIndex;
        public int ActiveHeaderIndex => activeHeaderIndex;

        public ChangeHeader GetActiveHeader() => IsChecked ? headerList[activeHeaderIndex] : null;
        public bool IsChecked => activeHeaderIndex >= 0;

        public Change(string titleIdent, ChangeType type, bool checkedDefault = true, int defaultActiveIndex = 0)
        {
            this.type = type;
            this.titleIdent = titleIdent;

            this.defaultActiveIndex = defaultActiveIndex;
            this.activeHeaderIndex = checkedDefault ? defaultActiveIndex : -1;

            var defaultHeader = new ChangeHeader(this.titleIdent);
            headerList = new List<ChangeHeader>() { defaultHeader };
        }

        public void Activate(ChangeArgs args)
        {
            ChangeHeader header = GetActiveHeader();

            /*foreach (var edit in editList)
                foreach (var element in edit)
                    if (element is BinValue value)
                    {
                        value.Set(valueDict[value.ValueIdent].Value);
                    }*/

            var list = header.EditList;
            for (int i = 0; i < list.Count; i++)
            {
                var result = list[i].Activate(args);
                if (result != EditResult.NoErrors)
                {
                    const string str = "Your version is currently unsupported: {0} for {1}/{2}/{3}.";
                    string message = string.Format(str, result, TitleIdent, ActiveHeaderIndex, i);
                    throw new Exception(message);
                }
            }
        }

        #region UI


        UIElement uiElement;
        public UIElement UIElement { get { return this.uiElement; } }

        CheckBox titleBox;

        public void InitUI()
        {
            this.titleBox = new CheckBox()
            {
                IsChecked = this.IsChecked,
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
                Width = 400,
                Margin = new Thickness(-18, 5, 0, 0),
                Focusable = false,
            };

            FillGrid(content);

            tvi.Items.Add(content);
            tvi.Items.Add(null); // spacing

            this.uiElement = tvi;

            if (IsChecked)
                GetActiveHeader().IsEnabled = true;
        }

        void SetChecked(bool check)
        {
            if (check)
            {
                if (activeHeaderIndex >= 0)
                    return;

                activeHeaderIndex = defaultActiveIndex;
                GetActiveHeader().IsEnabled = true;
            }
            else
            {
                ChangeHeader header = GetActiveHeader();
                if (header != null)
                    header.IsEnabled = false;
                activeHeaderIndex = -1;
            }
        }

        void FillGrid(Grid grid)
        {
            if (headerList.Count > 1)
            {
                headerList.RemoveAt(0); // default one is not needed
            }

            double height = 5;
            for (int i = 0; i < headerList.Count; i++)
            {
                var header = headerList[i];
                header.OnEnabledChange += Header_OnEnable;

                // ui element
                var uiElement = header.InitUI(headerList.Count > 1);
                uiElement.HorizontalAlignment = HorizontalAlignment.Left;
                uiElement.VerticalAlignment = VerticalAlignment.Top;
                uiElement.Margin = new Thickness(6, height, 0, 0);
                height += uiElement.Height + 5;

                grid.Children.Add(uiElement);



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
                    height += 18;
            }
            grid.Height = height + 10;
        }

        void Header_OnEnable(ChangeHeader header, bool enabled)
        {
            if (enabled)
            {
                var current = GetActiveHeader();

                activeHeaderIndex = headerList.IndexOf(header);
                if (current != null)
                    current.IsEnabled = false;
                else
                    titleBox.IsChecked = true;
            }
            else
            {
                titleBox.IsChecked = false;
            }
        }

        #endregion
    }
}
