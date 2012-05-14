using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Policy;
using System.Threading;

namespace MayhemVisionModules.Wpf
{


    public class ImagePopup : Form
    {
        public ImagePopup()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
        }

        public void ShowPopup(int millisecondTimeout, string filename)
        {
            if (filename != null)
            {
                this.BackgroundImage = Image.FromFile(filename);
                this.Size = this.BackgroundImage.Size;
                this.Show();
                Thread.Sleep(millisecondTimeout);
                this.Close();
            }
        }

        public void ShowPopup(int millisecondTimeout, Bitmap bmp)
        {
            if (bmp != null)
            {
                this.BackgroundImage = Image.FromFile(@"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg"); ;
                this.Size = this.BackgroundImage.Size;
                this.Show();
                Thread.Sleep(millisecondTimeout);
                this.Close();
            }
        }
    }
}
