using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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

        protected virtual UIElement CreateUIContent()
        {
            TextBlock content = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, -1, 0, 0),
                FontSize = 14,
                Width = 400,
                Height = 22,
            };
            TextReferencer.SetText(content, ident);
            return content;
        }
    }
}
