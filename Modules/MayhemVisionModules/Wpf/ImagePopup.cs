using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Policy;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace MayhemVisionModules.Wpf
{


    public class ImagePopup : Form
    {
        public ImagePopup()
        {
            
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(640, 480);
        }

        public void ShowPopup(int millisecondTimeout, string filename)
        {
            if (filename != null)
            {
                //IntPtr windowHandle = new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle;
                //NativeWindow parent = NativeWindow.FromHandle(windowHandle);

                this.BackgroundImage = Image.FromFile(filename);
                IntPtr wHnd = FindWindow("Mayhem", null);
                NativeWindow parent = NativeWindow.FromHandle(wHnd);
                SetParent(this.Handle, wHnd);
                this.TopMost = true;
                this.Show();
                Thread.Sleep(millisecondTimeout);
                this.Close();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ImagePopup
            // 
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImagePopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string ClassName, string WindowText);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /*public void ShowPopup(int millisecondTimeout, Bitmap bmp)
        {
            if (bmp != null)
            {
                this.BackgroundImage = Image.FromFile(@"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg"); ;
                this.Size = this.BackgroundImage.Size;
                this.Show();
                Thread.Sleep(millisecondTimeout);
                this.Close();
            }
        }*/
    }
}
