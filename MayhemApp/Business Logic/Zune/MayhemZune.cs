using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MayhemApp.Business_Logic.Zune
{
    class MayhemZune
    {
        private static MayhemZune Instance_ = null;

        public static MayhemZune Instance
        {
            get
            {
                if (Instance_ == null)
                {
                    Instance_ = new MayhemZune();
                }
                return Instance_;
            }

        }
        //http://pinvoke.net/default.aspx/user32.sendinput
        // ^^ Sending keyboard commands natively

        // http://www.zuneboards.com/forums/gen-1-gen-2/19081-keyboard-shortcuts.html
        // ^^ Zune commands

        #region DLL Imports
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        // Get a handle to an application window.
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Activate an application window. 
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        public MayhemZune()
        {
        }

        public bool SendCommand(string command)
        {
            // Current Window
            IntPtr currentWindow = GetForegroundWindow();

            IntPtr zuneHandle = FindWindow(null, "Zune");

            // Verify that Calculator is a running process.

            if (zuneHandle == IntPtr.Zero)
            {
                Console.WriteLine("Not Running!");
                return false;
            }

            // Make Zune the foreground application temporarily
            SetForegroundWindow(zuneHandle);

            // play pause
            //SendKeys.SendWait("^p");
            // next song
            //SendKeys.SendWait("^f");
            // back track
            //SendKeys.SendWait("^b");

            SendKeys.SendWait(command);

            // Change our foreground window back
            SetForegroundWindow(currentWindow);
            return true;
        }
    }
}
