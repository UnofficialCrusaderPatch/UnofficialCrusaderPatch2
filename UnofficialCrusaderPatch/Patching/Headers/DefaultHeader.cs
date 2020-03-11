using System;
using System.Windows;
using System.Windows.Controls;

namespace UCP.Patching
{
    public class DefaultHeader : ChangeHeader
    {
        public bool NoLocalization = false;

        Change parent;
        public Change Parent => parent;
        public virtual void SetParent(Change change)
        {
            this.parent = change;
            this.isEnabled = parent.EnabledDefault && this.DefaultIsEnabled;
        }

        string descrIdent;
        public string DescrIdent => descrIdent;
        public string GetDescription() { return NoLocalization ? descrIdent : Localization.Get(descrIdent); }

        bool defaultIsEnabled;
        public bool DefaultIsEnabled => defaultIsEnabled;

        public DefaultHeader(string descrIdent, bool suggested = true, bool noLocalization = false)
        {
            this.descrIdent = descrIdent;
            this.defaultIsEnabled = suggested;
            this.NoLocalization = noLocalization;
        }

        #region UI

        public virtual void SetUIEnabled(bool enabled)
        {
            if (box != null)
                this.box.IsEnabled = enabled;
        }

        public event Action<DefaultHeader, bool> OnEnabledChange;

        CheckBox box;
        public CheckBox CheckBox => box;

        public FrameworkElement InitUI(bool createCheckBox)
        {
            FrameworkElement content = CreateUI();
            if (createCheckBox)
            {
                box = new CheckBox()
                {
                    IsChecked = isEnabled,
                    Content = content,
                    Height = content.Height,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
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
            {
                SetEnabled(false);
            }
        }

        void Box_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsEnabled)
            {
                SetEnabled(true);
            }
        }

        protected virtual FrameworkElement CreateUI()
        {
            return new TextBlock()
            {
                Text = this.GetDescription(),
                Height = 16,
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
            Configuration.Save(this.Parent.TitleIdent);
        }

        #endregion


        #region Load & Save

        public virtual string GetValueString()
        {
            return this.isEnabled.ToString();
        }

        public virtual void LoadValueString(string valueStr)
        {
            if (Boolean.TryParse(valueStr, out bool result))
            {
                this.isEnabled = result;
            }
        }

        public override string ToString()
        {
            return DescrIdent + "={" + GetValueString() + "} ";
        }

        #endregion
    }
}
