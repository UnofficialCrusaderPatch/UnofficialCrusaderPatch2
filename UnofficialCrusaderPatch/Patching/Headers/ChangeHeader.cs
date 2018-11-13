using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UnofficialCrusaderPatch
{
    public class ChangeHeader : ChangeElement
    {
        List<ChangeEdit> editList = new List<ChangeEdit>();
        public ReadOnlyCollection<ChangeEdit> EditList => new ReadOnlyCollection<ChangeEdit>(editList);

        string descrIdent;
        public string DescrIdent => descrIdent;
        public string GetDescription() { return Localization.Get(descrIdent); }

        public event Action<ChangeHeader, bool> OnEnabledChange;

        public ChangeHeader(string descrIdent)
        {
            this.descrIdent = descrIdent;
        }

        public void Add(ChangeEdit edit)
        {
            this.editList.Add(edit);
        }

        CheckBox box;
        public CheckBox CheckBox => box;

        public FrameworkElement InitUI(bool createCheckBox)
        {
            FrameworkElement content = CreateUI();
            if (createCheckBox)
            {
                box = new CheckBox()
                {
                    Content = content,
                    Height = content.Height,
                };

                box.Checked += Box_Checked;
                box.Unchecked += Box_Unchecked;

                return box;
            }
            return content;
        }

        void Box_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsEnabled)
                SetEnabled(false);
        }

        void Box_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsEnabled)
                SetEnabled(true);
        }

        protected virtual FrameworkElement CreateUI()
        {
            return new TextBlock()
            {
                Text = this.GetDescription(),
                Height = 15,
            };
        }

        bool isEnabled;
        public virtual bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value)
                    return;

                isEnabled = value;
                if (box != null)
                    box.IsChecked = value;
            }
        }
        protected void SetEnabled(bool enabled)
        {
            if (IsEnabled == enabled)
                return;

            IsEnabled = enabled;
            OnEnabledChange?.Invoke(this, enabled);
        }
    }
}
