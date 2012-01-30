﻿using System;
using System.Runtime.InteropServices;

namespace DefaultModules.LowLevel
{
    public static class Utilities
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint uCode, MapVirtualKeyMapTypes uMapType);

        public const int KEYEVENTF_KEYUP = 0x2;

        public enum MapVirtualKeyMapTypes : uint
        {
            MAPVK_VK_TO_VSC = 0x00,
            MAPVK_VSC_TO_VK = 0x01,
            MAPVK_VK_TO_CHAR = 0x02,
            MAPVK_VSC_TO_VK_EX = 0x03,
            MAPVK_VK_TO_VSC_EX = 0x04
        }

        public static bool Is64BitProcess
        {
            get { return IntPtr.Size == 8; }
        }

        public static void SendKey(ushort key)
        {
            /*if (Is64BitProcess) {
                Native.INPUT_64[] inputStruct = new Native.INPUT_64[1];
                inputStruct[0] = new Native.INPUT_64();
                inputStruct[0].type = Native.INPUT_KEYBOARD;

                // Key down the actual key-code
                inputStruct[0].ki = Native.createKeybdInput(key, 0);
                Native.SendInput(1, inputStruct, Marshal.SizeOf(inputStruct[0]));
                // Key up the actual key-code
                inputStruct[0].ki = Native.createKeybdInput(key, Native.KEYEVENTF_KEYUP);
                Native.SendInput(1, inputStruct, Marshal.SizeOf(inputStruct[0]));
            } else {
                Native.INPUT_86[] inputStruct = new Native.INPUT_86[1];
                inputStruct[0] = new Native.INPUT_86();
                inputStruct[0].type = Native.INPUT_KEYBOARD;

                // Key down the actual key-code
                inputStruct[0].ki = Native.createKeybdInput(key, 0);
                Native.SendInput(1, inputStruct, Marshal.SizeOf(inputStruct[0]));
                // Key up the actual key-code
                inputStruct[0].ki = Native.createKeybdInput(key, Native.KEYEVENTF_KEYUP);
                Native.SendInput(1, inputStruct, Marshal.SizeOf(inputStruct[0]));
            }*/
            keybd_event((byte)key, (byte)MapVirtualKey(key, 0), 0, UIntPtr.Zero);
            keybd_event((byte)key, (byte)MapVirtualKey(key, 0), KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public static bool ConnectedToInternet()
        {
            try
            {
                System.Net.IPHostEntry obj = System.Net.Dns.GetHostEntry("www.google.com");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
