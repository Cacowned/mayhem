using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using WindowModules.Wpf;
using System.Diagnostics;

namespace WindowModules
{
    [DataContract]
    [MayhemModule("Window Sequence", "Window Sequence")]
    public class WindowSequence : ReactionBase, IWpfConfigurable
    {
        static readonly ulong TARGETWINDOW = Native.WS_BORDER | Native.WS_VISIBLE;

        WindowActionInfo actionInfo;

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

        static HashSet<int> processBlackList = new HashSet<int>();

        public WindowSequence()
        {
            actionInfo = new WindowActionInfo();
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new WindowSequenceConfig(actionInfo); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
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
                    if (actionInfo.WindowInfo.CheckFileName && !p.MainModule.FileName.ToLower().EndsWith(actionInfo.WindowInfo.FileName.ToLower()))
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
