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

        bool isChecked;
        public bool IsChecked
        {
            get => this.isChecked;
            set
            {
                headerList.ForEach(h => h.IsActive = false);
                if (value == false || headerList.Count <= 1)
                {
                    headerList[0].IsActive = true;
                }
                else
                {
                    headerList[1].IsActive = true;
                }
                this.isChecked = value;
            }
        }

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

        public ChangeHeader ActiveHeader => headerList.Find(h => h.IsActive);
        public int ActiveHeaderIndex => headerList.FindIndex(h => h.IsActive);

        public Change(string titleIdent, ChangeType type, bool checkedDefault = true)
        {
            this.type = type;
            this.titleIdent = titleIdent;
            this.isChecked = checkedDefault;
            this.headerList = new List<ChangeHeader>() { new ChangeHeader(this.titleIdent + "_descr") };
            this.uiContent = CreateUIContent();
        }

        public void Activate(ChangeArgs args)
        {
            ChangeHeader header = this.ActiveHeader;

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

        public CheckBox CheckBox { get { return (CheckBox)((TreeViewItem)uiContent).Header; } }

        UIElement uiContent;
        public UIElement UIContent { get { return this.uiContent; } }

        UIElement CreateUIContent()
        {
            CheckBox header = new CheckBox()
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
            header.Checked += (s, e) => this.IsChecked = true;
            header.Unchecked += (s, e) => this.IsChecked = false;

            TreeViewItem tvi = new TreeViewItem()
            {
                IsExpanded = false,
                Focusable = false,
                Header = header,
                MinHeight = 22,
            };


            Grid content = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200)),
                Width = 400,
                Margin = new Thickness(-18, 5, 0, 0),
                Focusable = false,
            };

            this.FillGrid(content);

            tvi.Items.Add(content);
            tvi.Items.Add(null); // spacing
            return tvi;
        }

        void FillGrid(Grid grid)
        {
            double height = 15;
            int startIndex = headerList.Count > 1 ? 1 : 0;
            for (int i = startIndex; i < headerList.Count; i++)
            {
                var header = headerList[i];

                // Description
                TextBlock description = new TextBlock()
                {
                    Margin = new Thickness(6, 5, 0, 0),
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    Width = grid.Width - 12,
                };
                TextReferencer.SetText(description, header.DescrIdent);
                grid.Children.Add(description);
                height += description.MeasureHeight();

                // ui element
                Control control = header.UIControl;
                if (control != null)
                {
                    grid.Children.Add(control);
                    height += control.Height;
                }
            }
            grid.Height = height;
        }

        #endregion
    }
}
