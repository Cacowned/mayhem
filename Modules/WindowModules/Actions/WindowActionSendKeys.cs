using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionSendKeys : IWindowAction
    {
        internal enum VirtualKeys : uint
        {
            Back = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Return = 0x0D,

            ShiftLeft = 0xA0,
            ControlLeft = 0xA2,
            ShiftRight = 0xA1,
            ControlRight = 0xA3,
            AltLeft = 0xA4,
            AltRight = 0xA5,

            Shift = 0x10,
            Control = 0x11,
            Menu = 0x12,
            Pause = 0x13,
            Capital = 0x14,
            Escape = 0x1B,
            Space = 0x20,
            Prior = 0x21,
            Next = 0x22,
            End = 0x23,
            Home = 0x24,
            Left = 0x25,
            Up = 0x26,
            Right = 0x27,
            Down = 0x28,
            Select = 0x29,
            Print = 0x2A,
            Execute = 0x2B,
            Snapshot = 0x2C,
            Insert = 0x2D,
            Delete = 0x2E,
            Help = 0x2F,

            D0 = 0x30,
            D1 = 0x31,
            D2 = 0x32,
            D3 = 0x33,
            D4 = 0x34,
            D5 = 0x35,
            D6 = 0x36,
            D7 = 0x37,
            D8 = 0x38,
            D9 = 0x39,

            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,

            LWindows = 0x5B,
            RWindows = 0x5C,
            Apps = 0x5D,
            NumPad0 = 0x60,
            NumPad1 = 0x61,
            NumPad2 = 0x62,
            NumPad3 = 0x63,
            NumPad4 = 0x64,
            NumPad5 = 0x65,
            NumPad6 = 0x66,
            NumPad7 = 0x67,
            NumPad8 = 0x68,
            NumPad9 = 0x69,

            Multiply = 0x6A,
            Add = 0x6B,
            Separator = 0x6C,
            Subtract = 0x6D,
            Decimal = 0x6E,
            Divide = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,

            NumLock = 0x90,
            Scroll = 0x91,
        }

        [DataMember]
        public List<Keys> KeyList
        {
            get;
            set;
        }

        public WindowActionSendKeys()
        {
            KeyList = new List<Keys>();
        }

        private const uint WM_KEYDOWN = 0x100;

        private const uint WM_KEYUP = 0x101;

        private byte GetVK(Keys key)
        {
            switch (key)
            {
                case Keys.Control:
                    return (byte)VirtualKeys.Control;
                case Keys.Alt:
                    return (byte)VirtualKeys.Menu;
                case Keys.Shift:
                    return (byte)VirtualKeys.Shift;
                default:
                    return (byte)key;
            }
        }

        public void Perform(IntPtr window)
        {
            Native.GetForegroundWindow();
            Native.SetForegroundWindow(window);
            Native.SetFocus(window);
            Native.SetActiveWindow(window);

            List<Native.INPUT> inputs = new List<Native.INPUT>();

            foreach (Keys t in KeyList)
            {
                if (t == Keys.Control || t == Keys.Shift || t == Keys.Alt)
                {
                    byte vk = GetVK(t);

                    var input = new Native.INPUT();

                    input.type = 1;
                    input.u.ki.wVk = vk;
                    input.u.ki.wScan = vk;
                    input.u.ki.time = 10;

                    inputs.Add(input);
                }
            }

            foreach (Keys t in KeyList)
            {
                if (t != Keys.Control && t != Keys.Shift && t != Keys.Alt)
                {
                    byte vk = GetVK(t);

                    var input = new Native.INPUT();

                    input.type = 1;
                    input.u.ki.wVk = vk;
                    input.u.ki.wScan = vk;
                    input.u.ki.time = 20;

                    inputs.Add(input);
                }
            }

            var inputArray = inputs.ToArray();

            Native.SendInput((uint)inputArray.Length, inputArray, Marshal.SizeOf(inputArray[0]));

            inputs.Clear();

            foreach (Keys t in KeyList)
            {
                byte vk = GetVK(t);

                var input = new Native.INPUT();

                input.type = 1;
                input.u.ki.wVk = vk;
                input.u.ki.wScan = vk;
                input.u.ki.dwFlags = 0x0002;

                inputs.Add(input);
            }

            inputArray = inputs.ToArray();

            Native.SendInput((uint)inputArray.Length, inputArray, Marshal.SizeOf(inputArray[0]));

            inputs.Clear();
        }
    }
}
