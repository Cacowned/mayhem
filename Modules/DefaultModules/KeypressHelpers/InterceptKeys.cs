using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MayhemCore;

namespace DefaultModules.KeypressHelpers
{
    class InterceptKeys : IDisposable
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

        private HashSet<Keys> keysDown = new HashSet<Keys>();
        Dictionary<Keys, DateTime> keysDownTimes = new Dictionary<Keys, DateTime>();

        Dictionary<HashSet<Keys>, List<KeyCombinationHandler>> keyCombinationHandlerMap;

        public static InterceptKeys Instance
        {
            get
            {
                if (instance == null)
                    instance = new InterceptKeys();

                return instance;
            }
        }

        private int refCount = 0;

        InterceptKeys()
        {
            keyCombinationHandlerMap = new Dictionary<HashSet<Keys>, List<KeyCombinationHandler>>();
            _proc = HookCallback;
        }

        ~InterceptKeys()
        {
            RemoveHook();
        }

        public void Dispose()
        {
            RemoveHook();
            GC.SuppressFinalize(this);
        }

        public void AddRef()
        {
            refCount++;
            SetHook();
        }

        public void RemoveRef()
        {
            refCount--;
            if (refCount == 0)
            {
                RemoveHook();
            }
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
            AddRef();
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
            RemoveRef();
        }

        void CheckCombinations()
        {
            List<Keys> removeList = null;
            DateTime now = DateTime.Now;
            foreach (Keys key in keysDown)
            {
                if (now.Subtract(keysDownTimes[key]).TotalSeconds > 2)
                {
                    Logger.WriteLine("Removing a key: " + key);
                    if (removeList == null)
                        removeList = new List<Keys>();
                    removeList.Add(key);
                }
            }
            if (removeList != null)
            {
                foreach (Keys key in removeList)
                {
                    keysDownTimes.Remove(key);
                    keysDown.Remove(key);
                }
            }
            foreach (HashSet<Keys> k in keyCombinationHandlerMap.Keys)
            {
                if (AreKeysetsEqual(k, keysDown))
                {
                    List<KeyCombinationHandler> listHandlers = keyCombinationHandlerMap[k];
                    foreach (KeyCombinationHandler t in listHandlers)
                    {
                        t();
                    }
                    break;
                }
            }
        }

        private static bool AreKeysetsEqual(HashSet<Keys> keys, HashSet<Keys> keys2)
        {
            if (keys.Count != keys2.Count)
                return false;

            return keys.SetEquals(keys2);
        }

        public void SetHook()
        {
            keysDown.Clear();
            if (_hookID == IntPtr.Zero)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                {
                    using (ProcessModule curModule = curProcess.MainModule)
                    {
                        Logger.WriteLine("Setting hook");
                        _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
                    }
                }
            }
        }

        public void RemoveHook()
        {
            keysDown.Clear();
            if (_hookID != IntPtr.Zero)
            {
                Logger.WriteLine("Removing hook");
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;
                keysDownTimes[key] = DateTime.Now;
                if (!keysDown.Contains(key))
                {
                    keysDown.Add(key);
                    CheckCombinations();
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
