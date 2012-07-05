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
        /// <summary>
        /// The new personal note.
        /// </summary>
        public string PersonalNote
        {
            get;
            private set;
        }

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return "Lync: Update Personal Note"; }
        }

        /// <summary>
        /// The constructor of the LyncUpdatePersonalNoteConfig class.
        /// </summary>
        /// <param name="personalNote">The personal note</param>
        public LyncUpdatePersonalNoteConfig(string personalNote)
        {
            PersonalNote = personalNote;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            PersonalNoteBox.Text = PersonalNote;

            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the user clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            PersonalNote = PersonalNoteBox.Text;
        }

        /// <summary>
        /// This method will check the validity of the information provided by the user.
        /// </summary>
        private void CheckValidity()
        {
            CanSave = PersonalNoteBox.Text.Trim().Length > 0;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// This method will be called when the text from the PersonalNoteBox changes.
        /// </summary>
        private void PersonalNoteBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
