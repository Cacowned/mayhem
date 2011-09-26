using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using MayhemCore;

namespace WindowModules
{
    public static class WindowFinder
    {
        public delegate void WindowActionResult(IntPtr hwnd);
        static WindowActionResult action = null;
        static WindowActionInfo actionInfo = null;
        static AutoResetEvent resetEvent = new AutoResetEvent(true);

        public static void Find(WindowActionInfo windowInfo, WindowActionResult actionOnResult)
        {
            resetEvent.WaitOne();
            actionInfo = windowInfo;
            action = actionOnResult;
            Native.EnumWindows(new Native.EnumWindowsCallback(Report), 0);
        }

        static bool Report(IntPtr hwnd, int lParam)
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
                            action(hwnd);
                            resetEvent.Set();
                            return false;
                        }
                    }
                }
                catch { }
            }
            return true;
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
    }
}
