using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemDefaultStyles.UserControls;
using System.Timers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for MoveWindowConfig.xaml
    /// </summary>
    public partial class MoveWindowConfig : IWpfConfiguration
    {
        Timer timer;
        Process thisProcess;
        IntPtr thisWindowHandle;
        public WindowInfo LastWindow
        {
            get;
            private set;
        }
        
        public WindowActionInfo SelectedWindow
        {
            get;
            private set;
        }

        public MoveWindowConfig(WindowActionInfo windowActionInfo)
        {
            InitializeComponent();
            this.SelectedWindow = windowActionInfo;
            LastWindow = SelectedWindow.WindowInfo;
            textBoxApplication.Text = SelectedWindow.WindowInfo.FileName;
            textBoxWindowTitle.Text = SelectedWindow.WindowInfo.Title;
            textBoxClass.Text = SelectedWindow.WindowInfo.ClassName;
            /*
            checkBoxMove.IsChecked = SelectedWindow.ShouldMove;
            textBoxMoveX.Text = SelectedWindow.MoveX.ToString();
            textBoxMoveY.Text = SelectedWindow.MoveY.ToString();
            */
        }

        public override void OnLoad()
        {
            thisProcess = Process.GetCurrentProcess();
            thisWindowHandle = thisProcess.MainWindowHandle;
            timer = new Timer(500);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public static int GetProcessThreadFromWindow(IntPtr hwnd)
        {
            int procid = 0;
            int threadid = Native.GetWindowThreadProcessId(hwnd, ref procid);
            return procid;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Obtain the handle of the active window.
            IntPtr handle = Native.GetForegroundWindow();

            if (handle == IntPtr.Zero || handle == thisWindowHandle)
                return;

            int procID = GetProcessThreadFromWindow(handle);
            Process p = Process.GetProcessById(procID);

            if (p.MainWindowHandle == IntPtr.Zero || p.MainWindowHandle == thisWindowHandle )
                return;

            int nRet;
            StringBuilder ClassName = new StringBuilder(100);
            //Get the window class name
            nRet = Native.GetClassName(handle, ClassName, ClassName.Capacity);
            string filename = p.MainModule.FileName;
            string title = p.MainWindowTitle;
            string className = ClassName.ToString();

            if (LastWindow == null ||
                LastWindow.FileName != filename ||
                LastWindow.Title != title ||
                LastWindow.ClassName != className)
            {
                if (LastWindow == null)
                    LastWindow = new WindowInfo();
                FileInfo fi = new FileInfo(filename);
                filename = fi.Name;
                LastWindow.FileName = filename;
                LastWindow.Title = title;
                LastWindow.ClassName = className;
                Dispatcher.Invoke((Action)delegate
                {
                    textBoxApplication.Text = filename;
                    textBoxWindowTitle.Text = title;
                    textBoxClass.Text = className;
                    //textBoxApplication.SelectionStart = textBoxApplication.Text.Length;
                    textBoxApplication.CaretIndex = textBoxApplication.Text.Length;
                });
            }
        }

        public override bool OnSave()
        {
            timer.Stop();
            timer = null;
            /*
            SelectedWindow.WindowInfo = LastWindow;
            SelectedWindow.ShouldMove = checkBoxMove.IsChecked == true;
            SelectedWindow.MoveX = Int32.Parse(textBoxMoveX.Text);
            SelectedWindow.MoveY = Int32.Parse(textBoxMoveY.Text);
            */
            return true;
        }

        public override void OnCancel()
        {
            timer.Stop();
            timer = null;
        }

        public override string Title
        {
            get { return "Move Window"; }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
