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
        /// <summary>
        /// The ID of the status.
        /// </summary>
        public int StatusId
        {
            get;
            private set;
        }

        /// <summary>
        /// The text of the status.
        /// </summary>
        public string StatusText
        {
            get;
            private set;
        }

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return "Lync: Update Status"; }
        }

        /// <summary>
        /// The constructor of the LyncUpdateStatusConfig class.
        /// </summary>
        /// <param name="statusId">The ID of the status</param>
        /// <param name="statusText">The text of the status</param>
        public LyncUpdateStatusConfig(int statusId, string statusText)
        {
            StatusId = statusId;
            StatusText = statusText;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the user clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            StatusText = (StatusComboBox.SelectedItem as ComboBoxItem).Content as string;
            StatusId = Int32.Parse((StatusComboBox.SelectedItem as ComboBoxItem).Tag.ToString());
        }

        /// <summary>
        /// This method will check the validity of the information provided by the user.
        /// </summary>
        private void CheckValidity()
        {
            CanSave = true;

            if (StatusComboBox.Items.Count == 0 || StatusComboBox.SelectedIndex == -1)
                CanSave = false;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// This method will be called when the selection of the StatusComboBox changes.
        /// </summary>
        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
