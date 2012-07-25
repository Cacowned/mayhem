using System;
using System.Windows.Forms;

namespace GoogleModules
{
    /// <summary>
    /// A form for entering the authorization code received from Google+.
    /// </summary>
    public partial class InsertKeyWindow : Form
    {
        public string AuthorizationCode
        {
            get;
            set;
        }

        public InsertKeyWindow()
        {
            InitializeComponent();
        }        

        private void authorizationCodeBox_Click(object sender, EventArgs e)
        {
            if (this.authorizationCodeBox.Text.Length == 0)
            {
                MessageBox.Show("The authorization code must contain at least 1 character.");

                return;
            }

            AuthorizationCode = this.authorizationCodeBox.Text;
            
            Close();
        }
    }
}
