using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using WindowModules.Wpf;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WindowModules
{
    [DataContract]
    [MayhemModule("Window Sequence", "Window Sequence")]
    public class WindowSequence : ReactionBase, IWpfConfigurable
    {
        static readonly ulong TARGETWINDOW = Native.WS_BORDER | Native.WS_VISIBLE;

        private WindowActionInfo actionInfo;

        [DataMember]
        public WindowActionInfo ActionInfo
        {
            get
            {
                return actionInfo;
            }
            set
            {
                actionInfo = value;
            }
        }

        private static HashSet<int> processBlackList = new HashSet<int>();

        public WindowSequence()
        {
            actionInfo = new WindowActionInfo();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new WindowSequenceConfig(actionInfo); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            WindowSequenceConfig config = (WindowSequenceConfig)configurationControl;
            actionInfo = config.ActionInfo;
            ConfigString = "";
        }

        private static bool IsTaskbarWindow(int hWnd)
        {
            int lExStyle;
            int hParent;
            lExStyle = Native.GetWindowLongPtr(hWnd, Native.GWL_EXSTYLE);
            hParent = Native.GetParent(hWnd);
            bool fTaskbarWindow = ((Native.IsWindowVisible(hWnd) != 0) & (Native.GetWindow(hWnd, Native.GW_OWNER) == 0) & (hParent == 0 | hParent == Native.GetDesktopWindow().ToInt32()));
            if ((lExStyle & Native.WS_EX_TOOLWINDOW) == Native.WS_EX_TOOLWINDOW)
            {
                fTaskbarWindow = false;
            }
            if ((lExStyle & Native.WS_EX_APPWINDOW) == Native.WS_EX_APPWINDOW)
            {
                fTaskbarWindow = true;
            }
            return fTaskbarWindow;
        }

        bool Report(IntPtr hwnd, int lParam)
        {
            if (IsTaskbarWindow(hwnd.ToInt32()))
            {
                int procid = 0;
                int threadid = Native.GetWindowThreadProcessId(hwnd, ref procid);
                try
                {
                    Process p = Process.GetProcessById(procid);
                    bool isMatch = true;
                    string filename;
                    try
                    {
                        filename = p.MainModule.FileName;
                    }
                    catch
                    {
                        filename = WMIProcess.GetFilename(procid);
                    }
                    if (filename != null)
                    {
                        FileInfo fi = new FileInfo(filename);
                        filename = fi.Name;
                    }
                    if (actionInfo.WindowInfo.CheckFileName && !filename.ToLower().EndsWith(actionInfo.WindowInfo.FileName.ToLower()))
                        isMatch = false;
                    if (isMatch)
                    {
                        if (actionInfo.WindowInfo.CheckTitle)
                        {
                            StringBuilder sb = new StringBuilder(200);
                            Native.GetWindowText(hwnd, sb, sb.Capacity);
                            string title = sb.ToString();
                            if (title != actionInfo.WindowInfo.Title)
                                isMatch = false;
                        }
                        if (isMatch)
                        {
                            Logger.WriteLine("Found: " + hwnd);
                            foreach (WindowAction action in actionInfo.WindowActions)
                            {
                                action.Perform(hwnd);
                                Thread.Sleep(50);
                            }
                        }
                    }
                }
                catch { }
            }
            return true;
        }

        //void FindWindowAndPerform(string className, IntPtr prevWindow)
        //{
        //    IntPtr hwnd = Native.FindWindowEx(IntPtr.Zero, prevWindow, className, actionInfo.WindowInfo.Title);
        //    Report(hwnd, 0);
        //}

        public override void Perform()
        {
            //if (actionInfo.WindowInfo.CheckTitle)
            //{
            //    FindWindowAndPerform(null, IntPtr.Zero);
            //}
            //else
            //{
                Native.EnumWindows(new Native.EnumWindowsCallback(Report), 0);
            //}
        }
    }
}
