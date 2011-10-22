using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowModules
{
    internal class InterceptKeys
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // windows intercept addresses
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        private LowLevelKeyboardProc proc;
        private IntPtr hookId = IntPtr.Zero;

        public delegate void KeyDownHandler(Keys key);

        public event KeyDownHandler OnKeyDown;

        public delegate void KeyUpHandler(Keys key);

        public event KeyUpHandler OnKeyUp;

        private readonly HashSet<Keys> keysDown;

        private static InterceptKeys instance = null;

        public static InterceptKeys Instance
        {
            get
            {
                if (instance == null)
                    instance = new InterceptKeys();

                return instance;
            }
        }

        InterceptKeys()
        {
            keysDown = new HashSet<Keys>();
        }

        ~InterceptKeys()
        {
            RemoveHook();
        }

        public void SetHook()
        {
            if (hookId == IntPtr.Zero)
            {
                proc = HookCallback;
                using (Process curProcess = Process.GetCurrentProcess())
                {
                    using (ProcessModule curModule = curProcess.MainModule)
                    {
                        hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                    }
                }
            }
        }

        public void RemoveHook()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;
                if (!keysDown.Contains(key))
                {
                    keysDown.Add(key);
                    if (OnKeyDown != null)
                    {
                        OnKeyDown(key);
                    }
                }
            }
            else if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;
                keysDown.Remove(key);
                if (OnKeyUp != null)
                {
                    OnKeyUp(key);
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }
    }
}
