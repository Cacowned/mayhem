using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// User Control for updating the personal note of the current user.
    /// </summary>
    public partial class LyncUpdatePersonalNoteConfig : WpfConfiguration
    {
        public string PersonalNote
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Lync: Update Personal Note"; }
        }

        public LyncUpdatePersonalNoteConfig(string personalNote)
        {
            PersonalNote = personalNote;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            PersonalNoteBox.Text = PersonalNote;

            CheckValidity();
        }

        public override void OnSave()
        {
            PersonalNote = PersonalNoteBox.Text;
        }

        private void CheckValidity()
        {
            CanSave = PersonalNoteBox.Text.Trim().Length > 0;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void PersonalNoteBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
