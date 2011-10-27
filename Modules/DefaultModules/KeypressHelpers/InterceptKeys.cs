using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MayhemCore;

namespace DefaultModules.KeypressHelpers
{
    internal class InterceptKeys : IDisposable
    {
        // windows intercept addresses
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        private static InterceptKeys instance;

        private readonly LowLevelKeyboardProc proc;
        private IntPtr hookId; 

        public delegate void KeyCombinationHandler();

        public delegate void KeyDownHandler(Keys key);

        public event KeyDownHandler OnKeyDown;

        public delegate void KeyUpHandler(Keys key);

        public event KeyUpHandler OnKeyUp;

        private readonly HashSet<Keys> keysDown;
        private readonly Dictionary<Keys, DateTime> keysDownTimes;

        private readonly Dictionary<HashSet<Keys>, List<KeyCombinationHandler>> keyCombinationHandlerMap;

        public static InterceptKeys Instance
        {
            get
            {
                if (instance == null)
                    instance = new InterceptKeys();

                return instance;
            }
        }

        private int refCount;

        private InterceptKeys()
        {
            keyCombinationHandlerMap = new Dictionary<HashSet<Keys>, List<KeyCombinationHandler>>();
            keysDown = new HashSet<Keys>();
            keysDownTimes = new Dictionary<Keys, DateTime>();

            hookId = IntPtr.Zero;
            proc = HookCallback;
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

        private void CheckCombinations()
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
            if (hookId == IntPtr.Zero)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                {
                    using (ProcessModule curModule = curProcess.MainModule)
                    {
                        Logger.WriteLine("Setting hook");
                        hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                    }
                }
            }
        }

        public void RemoveHook()
        {
            keysDown.Clear();
            if (hookId != IntPtr.Zero)
            {
                Logger.WriteLine("Removing hook");
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                
                var key = (Keys)vkCode;
                
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
                var key = (Keys)vkCode;
                keysDown.Remove(key);
                if (OnKeyUp != null)
                {
                    OnKeyUp(key);
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
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
