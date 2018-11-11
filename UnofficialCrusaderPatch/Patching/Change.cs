using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnofficialCrusaderPatch
{
    public enum ChangeType
    {
        Bugfix,
        AILords,
        Troops,
        Other,
    }

    public abstract class Change
    {
        string ident;
        public string Ident => ident;

        bool isChecked;
        public virtual bool IsChecked
        {
            get => this.isChecked;
            set => this.isChecked = value;
        }

        ChangeType type;
        public ChangeType Type => type;

        public Change(string ident, ChangeType type, bool checkedDefault)
        {
            this.ident = ident;
            this.isChecked = checkedDefault;
            this.type = type;
            this.uiContent = CreateUIContent();
        }

        UIElement uiContent;
        public UIElement UIContent { get { return this.uiContent; } }

        UIElement CreateUIContent()
        { 
            CheckBox header = new CheckBox()
            {
                IsChecked = this.IsChecked,
                Content = new TextBlock()
                {
                    Text = Localization.Get(this.ident),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, -1, 0, 0),
                    FontSize = 14,
                    Width = 400,
                }
            };

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
            };

            this.FillGrid(content);
            
            tvi.Items.Add(content);
            tvi.Items.Add(null); // spacing
            return tvi;
        }

        protected virtual void FillGrid(Grid grid)
        {
            TextBlock description = new TextBlock()
            {
                Margin = new Thickness(6, 5, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13,
                Width = grid.Width - 12,
            };
            TextReferencer.SetText(description, this.ident + "_descr");
            grid.Children.Add(description);
            
            grid.Loaded += (s, e) => grid.Height = description.ActualHeight + 10;
        }
        
    }
}
