using System;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// User Control for updating the status of the current user.
    /// </summary>
    public partial class LyncUpdateStatusConfig : WpfConfiguration
    {
        public int StatusId
        {
            get;
            private set;
        }

        public string StatusText
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Lync: Update Status"; }
        }

        public LyncUpdateStatusConfig(int statusId, string statusText)
        {
            StatusId = statusId;
            StatusText = statusText;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            int index = -1;

            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                index++;
                if (Int32.Parse(item.Tag.ToString()) == StatusId)
                {
                    break;
                }
            }

            if (index >= 0 && index < StatusComboBox.Items.Count)
                StatusComboBox.SelectedIndex = index;
            else
                StatusComboBox.SelectedIndex = 0;

            CheckValidity();
        }

        public override void OnSave()
        {
            StatusText = (StatusComboBox.SelectedItem as ComboBoxItem).Content as string;
            StatusId = Int32.Parse((StatusComboBox.SelectedItem as ComboBoxItem).Tag.ToString());
        }

        private void CheckValidity()
        {
            CanSave = true;

            if (StatusComboBox.Items.Count == 0 || StatusComboBox.SelectedIndex == -1)
                CanSave = false;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
