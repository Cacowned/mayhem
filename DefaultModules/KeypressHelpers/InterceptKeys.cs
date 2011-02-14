using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// TODO: I really don't like needing a reference to Sys.windows.forms. 
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DefaultModules.KeypressHelpers
{
    class InterceptKeys
    {
        public static InterceptKeys instance = null;

        // windows intercept addresses
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public delegate void KeyDownHandler(object sender, KeyEventArgs e);
        public static event KeyDownHandler OnInterceptKeyDown;

        public delegate void KeyUpHandler(object sender, KeyEventArgs e);
        public static event KeyUpHandler OnInterceptKeyUp;

        public static InterceptKeys GetInstance()
        {
            if (instance == null)
                instance = new InterceptKeys();

            return instance;
        }


        public InterceptKeys() {
            _hookID = SetHook(_proc);
        }

        ~InterceptKeys() {
            UnhookWindowsHookEx(_hookID);
        }


        /*
        public static void Main()
        {
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }*/

        private static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)) {
                int vkCode = Marshal.ReadInt32(lParam);

                if (OnInterceptKeyDown != null) {
                    KeyEventArgs e = new KeyEventArgs((Keys)vkCode);
                    OnInterceptKeyDown(InterceptKeys.instance, e);
                }
            } else if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)) {
                int vkCode = Marshal.ReadInt32(lParam);

                if (OnInterceptKeyUp != null) {
                    KeyEventArgs e = new KeyEventArgs((Keys)vkCode);
                    OnInterceptKeyUp(InterceptKeys.instance, e);
                }

            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
