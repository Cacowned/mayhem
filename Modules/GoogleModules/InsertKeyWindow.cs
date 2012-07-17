using System;
using System.Windows.Forms;

namespace GoogleModules
{
    /// <summary>
    /// A form for entering the authorization code received from Google+
    /// </summary>
    public partial class InsertKeyWindow : Form
    {
        /// <summary>
        /// The authorization code
        /// </summary>
        public string AuthorizationCode
        {
            get;
            set;
        }

        /// <summary>
        /// The constructor of the form
        /// </summary>
        public InsertKeyWindow()
        {
            InitializeComponent();
        }        

        /// <summary>
        /// This method is called when the user click the Ok button and saves the authorization code entered by the user and after that closes the window
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            AuthorizationCode = this.textBox1.Text;

            Close();
        }
    }
}
