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
using MayhemWpf.UserControls;
using System.Timers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
    /// <summary>
    /// Interaction logic for MoveWindowConfig.xaml
    /// </summary>
    public partial class WindowSequenceConfig : WpfConfiguration
    {
        public WindowInfo SelectedWindow
        {
            get;
            private set;
        }

        public WindowActionInfo ActionInfo
        {
            get;
            private set;
        }

        private Timer timer;
        private Process thisProcess;
        private IntPtr thisWindowHandle;

        private Dictionary<UserControl, WindowAction> controlMap = new Dictionary<UserControl, WindowAction>();

        public static IntPtr CurrentlySelectedWindow = IntPtr.Zero;

        public WindowSequenceConfig(WindowActionInfo windowActionInfo)
        {
            InitializeComponent();
            this.ActionInfo = windowActionInfo;
            SelectedWindow = ActionInfo.WindowInfo;
            textBoxApplication.Text = ActionInfo.WindowInfo.FileName;
            textBoxWindowTitle.Text = ActionInfo.WindowInfo.Title;
            checkBoxApplication.IsChecked = ActionInfo.WindowInfo.CheckFileName;
            checkBoxTitle.IsChecked = ActionInfo.WindowInfo.CheckTitle;

            foreach (WindowAction action in windowActionInfo.WindowActions)
            {
                Add(action);
            }
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
            CheckCanSave();
        }

        public static int GetProcessThreadFromWindow(IntPtr hwnd)
        {
            int procid = 0;
            int threadid = Native.GetWindowThreadProcessId(hwnd, ref procid);
            return procid;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IntPtr handle = Native.GetForegroundWindow();

            if (handle == IntPtr.Zero || handle == thisWindowHandle)
                return;

            int procID = GetProcessThreadFromWindow(handle);
            Process p = Process.GetProcessById(procID);

            if (p.MainWindowHandle == IntPtr.Zero || p.MainWindowHandle == thisWindowHandle )
                return;

            string filename;

            CurrentlySelectedWindow = handle;

            try
            {
                filename = p.MainModule.FileName;
            }
            catch
            {
                filename = WMIProcess.GetFilename(procID);
            }
            if (filename != null)
            {
                FileInfo fi = new FileInfo(filename);
                filename = fi.Name;
            }

            StringBuilder sb = new StringBuilder(200);
            Native.GetWindowText(handle, sb, sb.Capacity);
            string title = sb.ToString();

            if (SelectedWindow == null ||
                SelectedWindow.FileName != filename ||
                SelectedWindow.Title != title)
            {
                SelectedWindow.FileName = filename;
                SelectedWindow.Title = title;
                Dispatcher.Invoke((Action)delegate
                {
                    textBoxApplication.Text = filename;
                    textBoxWindowTitle.Text = title;
                    textBoxApplication.CaretIndex = textBoxApplication.Text.Length;
                });
            }
        }

        public override void OnSave()
        {
            timer.Stop();
            timer = null;

            ActionInfo.WindowInfo.CheckFileName = checkBoxApplication.IsChecked == true;
            ActionInfo.WindowInfo.CheckTitle = checkBoxTitle.IsChecked == true;
            ActionInfo.WindowActions.Clear();
            foreach (WindowActionControl wac in stackPanelActions.Children)
            {
                ((WindowActionConfigControl)wac.Config).Save();
                ActionInfo.WindowActions.Add(controlMap[wac.Config]);
            }
        }

        public override void OnCancel()
        {
            timer.Stop();
            timer = null;
        }

        public override string Title
        {
            get { return "Window Sequence"; }
        }

        void Add(WindowAction action)
        {
            UserControl newControl = null;
            if (action is WindowActionBringToFront)
                newControl = new WindowBringToFront((WindowActionBringToFront)action);
            else if (action is WindowActionClose)
                newControl = new WindowClose((WindowActionClose)action);
            else if (action is WindowActionMaximize)
                newControl = new WindowMaximize((WindowActionMaximize)action);
            else if (action is WindowActionMinimize)
                newControl = new WindowMinimize((WindowActionMinimize)action);
            else if (action is WindowActionMove)
                newControl = new WindowMove((WindowActionMove)action);
            else if (action is WindowActionResize)
                newControl = new WindowResize((WindowActionResize)action);
            else if (action is WindowActionRestore)
                newControl = new WindowRestore((WindowActionRestore)action);
            else if (action is WindowActionSendKeys)
                newControl = new WindowSendKeys((WindowActionSendKeys)action);
            else if (action is WindowActionWait)
                newControl = new WindowWait((WindowActionWait)action);

            WindowActionControl wac = new WindowActionControl(newControl);
            wac.Deleted += new EventHandler(wac_Deleted);
            stackPanelActions.Children.Add(wac);
            wac.Index = stackPanelActions.Children.Count;

            controlMap.Add(newControl, action);
        }

        void wac_Deleted(object sender, EventArgs e)
        {
            WindowActionControl wac = sender as WindowActionControl;
            stackPanelActions.Children.Remove(wac);
            controlMap.Remove(wac);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowAction action = null;
            string name = ((ComboBoxItem)comboBoxActions.SelectedItem).Content as string;
            switch (name)
            {
                case "Bring to front":
                    action = new WindowActionBringToFront();
                    break;
                case "Close":
                    action = new WindowActionClose();
                    break;
                case "Maximize":
                    action = new WindowActionMaximize();
                    break;
                case "Minimize":
                    action = new WindowActionMinimize();
                    break;
                case "Move":
                    action = new WindowActionMove();
                    break;
                case "Restore":
                    action = new WindowActionRestore();
                    break;
                case "Resize":
                    action = new WindowActionResize();
                    break;
                case "Send keys":
                    action = new WindowActionSendKeys();
                    break;
                case "Wait":
                    action = new WindowActionWait();
                    break;
                default:
                    return;
            }
            Add(action);
            CheckCanSave();
        }

        void CheckCanSave()
        {
            CanSave = ((textBoxApplication.Text.Length > 0 && checkBoxApplication.IsChecked == true) ||
                      (textBoxWindowTitle.Text.Length > 0 && checkBoxTitle.IsChecked == true)) && stackPanelActions.Children.Count > 0;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckCanSave();
        }
    }
}
