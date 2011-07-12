using System;
using System.Runtime.InteropServices;
using ZuneTest;

namespace DefaultModules.LowLevel
{
	public static class Utils
	{
		public static bool Is64BitProcess 
        {
			get { return IntPtr.Size == 8; }
		}

		public static void SendKey(ushort key, byte scanCode = 0) 
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
            Native.keybd_event((byte)key, scanCode, 1, UIntPtr.Zero);
            Native.keybd_event((byte)key, scanCode, 3, UIntPtr.Zero);
		}

	}
}
