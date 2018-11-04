using System;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;

namespace UnofficialCrusaderPatch
{
    public enum ChangeType
    {
        Bugfix,
        AILords,
        Troops,
    }

    public abstract class Change
    {
        string ident;
        public string Ident { get { return this.ident; } }
        
        public bool IsChecked { get; set; }

        ChangeType type;
        public ChangeType Type { get { return this.type; } }

        public Change(string ident, ChangeType type, bool checkedDefault)
        {
            this.ident = ident;
            this.IsChecked = checkedDefault;
            this.type = type;
        }
    }
}
