using System;
using System.Collections.Generic;
using MayhemWpf.UserControls;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Wpf
{
    /// <summary>
    /// User Control for setting the new value of the Skype status.
    /// </summary>
    public partial class SkypeStatusConfig : WpfConfiguration
    {
        public TUserStatus Status
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return Strings.ChangeStatus_Title; }
        }

        private Dictionary<string, TUserStatus> statuses;

        public SkypeStatusConfig(TUserStatus status)
        {
            // The value of CanSave can be setted to true because no check is needed since this window contains only a combo box 
            // which is setted automatically on the first element if other values are not selected.
            CanSave = true;
            Status = status;

            InitializeComponent();

            statuses = new Dictionary<string, TUserStatus>();

            foreach (TUserStatus sts in Enum.GetValues(typeof(TUserStatus)))
            {
                // Even though these 3 statuses are on the enum they are not displayed on Skype so the user can't use them.
                if (!sts.Equals(TUserStatus.cusUnknown) && !sts.Equals(TUserStatus.cusNotAvailable) && !sts.Equals(TUserStatus.cusLoggedOut))
                {
                    statuses.Add(sts.ToString().Replace("cus", ""), sts);
                }
            }

            StatusComboBox.ItemsSource = statuses.Keys;
        }

        public override void OnLoad()
        {
            StatusComboBox.SelectedItem = Status.ToString().Replace("cus", "");
        }

        public override void OnSave()
        {
            Status = statuses[StatusComboBox.SelectedItem as string];
        }
    }
}
