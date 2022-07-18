using System;
using System.Windows;
using System.Windows.Controls;

namespace UCP.Patching
{
    public class DefaultHeader : ChangeHeader
    {
        public bool NoLocalization;

        public  Change Parent { get; private set; }

        public virtual void SetParent(Change change)
        {
            Parent = change;
            isEnabled = Parent.EnabledDefault && DefaultIsEnabled;
        }

        public  string DescrIdent { get; }

        public  string GetDescription() { return NoLocalization ? DescrIdent : Localization.Get(DescrIdent); }

        public  bool DefaultIsEnabled { get; }

        public DefaultHeader(string descrIdent, bool suggested = true, bool noLocalization = false)
        {
            DescrIdent = descrIdent;
            DefaultIsEnabled = suggested;
            NoLocalization = noLocalization;
        }

        #region UI

        public virtual void SetUIEnabled(bool enabled)
        {
            if (CheckBox != null)
            {
                CheckBox.IsEnabled = enabled;
            }
        }

        public event Action<DefaultHeader, bool> OnEnabledChange;

        public  CheckBox CheckBox { get; private set; }

        public FrameworkElement InitUI(bool createCheckBox)
        {
            FrameworkElement content = CreateUI();
            if (createCheckBox)
            {
                CheckBox = new CheckBox
                      {
                    IsChecked = isEnabled,
                    Content = content,
                    Height = content.Height,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                };

                CheckBox.Checked += Box_Checked;
                CheckBox.Unchecked += Box_Unchecked;

                return CheckBox;
            }
            return content;
        }

        private void Box_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                SetEnabled(false);
            }
        }

        private void Box_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsEnabled)
            {
                SetEnabled(true);
            }
        }

        protected virtual FrameworkElement CreateUI()
        {
            return new TextBlock
                   {
                Text = GetDescription(),
                Height = 16,
            };
        }

        private bool isEnabled;
        public virtual bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value)
                {
                    return;
                }

                isEnabled = value;

                if (CheckBox != null)
                {
                    CheckBox.IsChecked = value;
                }
            }
        }

        protected void SetEnabled(bool enabled)
        {
            if (IsEnabled == enabled)
            {
                return;
            }

            IsEnabled = enabled;
            OnEnabledChange?.Invoke(this, enabled);
            Configuration.Save(Parent.TitleIdent);
        }

        #endregion


        #region Load & Save

        public virtual string GetValueString()
        {
            return isEnabled.ToString();
        }

        public virtual void LoadValueString(string valueStr)
        {
            if (Boolean.TryParse(valueStr, out bool result))
            {
                isEnabled = result;
            }
        }

        public override string ToString()
        {
            return DescrIdent + "={" + GetValueString() + "} ";
        }

        #endregion
    }
}
