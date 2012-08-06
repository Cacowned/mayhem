using System.Windows.Controls;
using SkypeModules.Resources;

namespace SkypeModules.Wpf
{
    /// <summary>
    /// User Control for setting the Skype ID of the user we want to interact with.
    /// </summary>
    public partial class SkypeIDConfig : SkypeBaseConfig
    {
        public SkypeIDConfig(string skypeID, string title)
        {
            SkypeID = skypeID;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            SkypeIDBox.Text = SkypeID;

            CheckValidity();
        }

        public override void OnSave()
        {
            SkypeID = SkypeIDBox.Text;
        }

        protected override void CheckValidity()
        {
            CheckValidityField(SkypeIDBox.Text, 100, Strings.SkypeID);
            DisplayErrorMessage(textInvalid);
        }

        private void SkypeIDBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
