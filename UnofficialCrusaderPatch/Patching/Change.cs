using System;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace UnofficialCrusaderPatch
{
    public enum ChangeType
    {
        Balancing,
        Bugfix,
    }

    public abstract class Change
    {
        string ident;
        public string Ident { get { return this.ident; } }
        public string Description { get { return Localization.Get(ident); } }
        public bool IsChecked { get; set; }

        ChangeType type;
        public ChangeType Type { get { return this.type; } }

        public bool Marked { get; set; }
        public Brush Background { get { return this.Marked ? Brushes.Red : Brushes.White; } }

        public Change(string ident, ChangeType type, bool checkedDefault)
        {
            this.ident = ident;
            this.IsChecked = checkedDefault;
            this.type = type;
        }
    }
}
