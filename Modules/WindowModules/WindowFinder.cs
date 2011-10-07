﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using MayhemCore;
using System.Collections.Generic;

namespace WindowModules
{
    public static class WindowFinder
    {
        public delegate void WindowActionResult(IntPtr hwnd);
        static WindowActionResult action = null;
        static WindowActionInfo actionInfo = null;

        public static void Find(WindowActionInfo windowInfo, WindowActionResult actionOnResult)
        {
            actionInfo = windowInfo;
            action = actionOnResult;

            Native.EnumWindows(new Native.EnumWindowsCallback(Report), 0);
        }

        static bool CheckWindow(IntPtr hwnd)
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
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        static bool Report(IntPtr hwnd, int lParam)
        {
            if (IsTaskbarWindow(hwnd.ToInt32()))
            {
                if (CheckWindow(hwnd))
                    return false;
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
