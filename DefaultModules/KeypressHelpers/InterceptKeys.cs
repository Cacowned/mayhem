using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DefaultModules.KeypressHelpers
{
    class InterceptKeys
    {
        private static InterceptKeys instance = null;

        // windows intercept addresses
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public delegate void KeyCombinationHandler();

        public delegate void KeyDownHandler(Keys key);
        public event KeyDownHandler OnKeyDown;

        public delegate void KeyUpHandler(Keys key);
        public event KeyUpHandler OnKeyUp;

        private HashSet<Keys> keys_down = new HashSet<Keys>();

        Dictionary<HashSet<Keys>,List<KeyCombinationHandler>> keyCombinationHandlerMap;

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
            keyCombinationHandlerMap = new Dictionary<HashSet<Keys>, List<KeyCombinationHandler>>();
            _hookID = SetHook();
        }

        ~InterceptKeys()
        {
            UnhookWindowsHookEx(_hookID);
        }

        public void AddCombinationHandler(HashSet<Keys> keys, KeyCombinationHandler handler)
        {
            List<KeyCombinationHandler> listHandlers = null;
            foreach (HashSet<Keys> k in keyCombinationHandlerMap.Keys)
            {
                if (AreKeysetsEqual(keys, k))
                {
                    listHandlers = keyCombinationHandlerMap[k];
                    break;
                }
            }
            if (listHandlers == null)
            {
                listHandlers = new List<KeyCombinationHandler>();
                keyCombinationHandlerMap[keys] = listHandlers;
            }
            if (!listHandlers.Contains(handler))
            {
                listHandlers.Add(handler);
            }
        }

        public void RemoveCombinationHandler(HashSet<Keys> keys, KeyCombinationHandler handler)
        {
            foreach (HashSet<Keys> k in keyCombinationHandlerMap.Keys)
            {
                if (AreKeysetsEqual(keys, k))
                {
                    keyCombinationHandlerMap[k].Remove(handler);
                    break;
                }
            }
        }

        void CheckCombinations()
        {
            foreach (HashSet<Keys> k in keyCombinationHandlerMap.Keys)
            {
                if (AreKeysetsEqual(k, keys_down))
                {
                    List<KeyCombinationHandler> listHandlers = keyCombinationHandlerMap[k];
                    for (int i = 0; i < listHandlers.Count; i++)
                    {
                        listHandlers[i]();
                    }
                    break;
                }
            }
        }

        private bool AreKeysetsEqual(HashSet<Keys> keys, HashSet<Keys> keys2)
        {
            if (keys.Count != keys2.Count)
                return false;

            return keys.SetEquals(keys2);
        }

        private IntPtr SetHook()
        {
            _proc = new LowLevelKeyboardProc(HookCallback);
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, _proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;
                keys_down.Add(key);
                CheckCombinations();
                if (OnKeyDown != null)
                {
                    OnKeyDown(key);
                }
            }
            else if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;
                keys_down.Remove(key);
                if (OnKeyUp != null)
                {
                    OnKeyUp(key);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
