using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace UnofficialCrusaderPatch
{
    public class ChangeHeader : ChangeElement
    {
        public bool IsActive { get; set; }

        List<ChangeEdit> editList = new List<ChangeEdit>();
        public ReadOnlyCollection<ChangeEdit> EditList => new ReadOnlyCollection<ChangeEdit>(editList);

        string descrIdent;
        public string DescrIdent => descrIdent;
        public string GetDescription() { return Localization.Get(descrIdent); }

        public ChangeHeader(string descrIdent)
        {
            this.IsActive = true;
            this.descrIdent = descrIdent;
            this.uiControl = CreateControl();
        }

        public void Add(ChangeEdit edit)
        {
            this.editList.Add(edit);
        }

        Control uiControl;
        public Control UIControl => uiControl;
        protected virtual Control CreateControl() => null;
    }
}
