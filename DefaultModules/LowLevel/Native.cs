using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ZuneTest
{
	public class Native
	{
		#region USER32

		[DllImport("user32.dll")]
		public static extern bool SetProp(IntPtr hWnd, string lpString, IntPtr hData);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr ptr);

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int abc);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr ptr);

		[DllImport("user32.dll")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out Native.RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

		[DllImport("user32.dll")]
		public static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
		   int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);

		[DllImport("user32.dll")]
		public static extern IntPtr GetFocus();

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ClientToScreen(IntPtr hwnd, ref POINT point);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(uint nInputs, INPUT_64[] pInputs, int cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(uint nInputs, INPUT_86[] pInputs, int cbSize);

		[DllImport("user32.dll")]
		public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

		[DllImport("user32.dll")]
		public static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);

		public delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool IsWindowEnabled(IntPtr hWnd);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongA")]
		public static extern int GetWindowLongPtr(int hwnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int GetParent(int hwnd);

		[DllImport("user32.dll")]
		public static extern int GetWindow(int hwnd, int wCmd);

		[DllImport("user32.dll")]
		public static extern int IsWindowVisible(int hwnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetGUIThreadInfo(IntPtr idThread, ref GUITHREADINFO gui);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern bool AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetCurrentThreadId();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetCaretPos(out POINT point);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hWnd) {
			WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
			placement.length = Marshal.SizeOf(placement);
			GetWindowPlacement(hWnd, ref placement);
			return placement;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		public enum SetWindowPosFlags : uint
		{
			/// <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
			/// the system posts the request to the thread that owns the window. This prevents the calling thread from 
			/// blocking its execution while other threads process the request.</summary>
			/// <remarks>SWP_ASYNCWINDOWPOS</remarks>
			SynchronousWindowPosition = 0x4000,
			/// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
			/// <remarks>SWP_DEFERERASE</remarks>
			DeferErase = 0x2000,
			/// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
			/// <remarks>SWP_DRAWFRAME</remarks>
			DrawFrame = 0x0020,
			/// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
			/// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
			/// is sent only when the window's size is being changed.</summary>
			/// <remarks>SWP_FRAMECHANGED</remarks>
			FrameChanged = 0x0020,
			/// <summary>Hides the window.</summary>
			/// <remarks>SWP_HIDEWINDOW</remarks>
			HideWindow = 0x0080,
			/// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
			/// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
			/// parameter).</summary>
			/// <remarks>SWP_NOACTIVATE</remarks>
			DoNotActivate = 0x0010,
			/// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
			/// contents of the client area are saved and copied back into the client area after the window is sized or 
			/// repositioned.</summary>
			/// <remarks>SWP_NOCOPYBITS</remarks>
			DoNotCopyBits = 0x0100,
			/// <summary>Retains the current position (ignores X and Y parameters).</summary>
			/// <remarks>SWP_NOMOVE</remarks>
			IgnoreMove = 0x0002,
			/// <summary>Does not change the owner window's position in the Z order.</summary>
			/// <remarks>SWP_NOOWNERZORDER</remarks>
			DoNotChangeOwnerZOrder = 0x0200,
			/// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
			/// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
			/// window uncovered as a result of the window being moved. When this flag is set, the application must 
			/// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
			/// <remarks>SWP_NOREDRAW</remarks>
			DoNotRedraw = 0x0008,
			/// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
			/// <remarks>SWP_NOREPOSITION</remarks>
			DoNotReposition = 0x0200,
			/// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
			/// <remarks>SWP_NOSENDCHANGING</remarks>
			DoNotSendChangingEvent = 0x0400,
			/// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
			/// <remarks>SWP_NOSIZE</remarks>
			IgnoreResize = 0x0001,
			/// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
			/// <remarks>SWP_NOZORDER</remarks>
			IgnoreZOrder = 0x0004,
			/// <summary>Displays the window.</summary>
			/// <remarks>SWP_SHOWWINDOW</remarks>
			ShowWindow = 0x0040,
		}

		#endregion

		#region GDI32

		[DllImport("gdi32.dll")]
		public static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdcDest, int xDest,
			int yDest, int wDest, int hDest, IntPtr hdcSource,
			int xSrc, int ySrc, int RasterOp);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
			int nWidth, int nHeight);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect,
		   int nBottomRect);

		#endregion

		#region SHELL32

		public const uint SHGFI_ICON = 0x100;
		public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
		public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath,
									uint dwFileAttributes,
									ref SHFILEINFO psfi,
									uint cbSizeFileInfo,
									uint uFlags);
		#endregion

		#region structs

		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};


		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWINFO
		{
			public uint cbSize;
			public Native.RECT rcWindow;
			public Native.RECT rcClient;
			public uint dwStyle;
			public uint dwExStyle;
			public uint dwWindowStatus;
			public uint cxWindowBorders;
			public uint cyWindowBorders;
			public ushort atomWindowType;
			public ushort wCreatorVersion;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public Int32 Left;
			public Int32 Top;
			public Int32 Right;
			public Int32 Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public Int32 x;
			public Int32 y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT
		{
			public int dx;
			public int dy;
			public int mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT
		{
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT
		{
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT_86
		{
			[FieldOffset(0)]
			public int type;
			[FieldOffset(4)]
			public MOUSEINPUT mi;
			[FieldOffset(4)]
			public KEYBDINPUT ki;
			[FieldOffset(4)]
			public HARDWAREINPUT hi;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT_64
		{
			[FieldOffset(0)]
			public int type;
			[FieldOffset(8)]
			public MOUSEINPUT mi;
			[FieldOffset(8)]
			public KEYBDINPUT ki;
			[FieldOffset(8)]
			public HARDWAREINPUT hi;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct GUITHREADINFO
		{
			public void Initialize() {
				cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(GUITHREADINFO));
			}
			public uint cbSize;
			public uint flags;
			public IntPtr hwndActive;
			public IntPtr hwndFocus;
			public IntPtr hwndCapture;
			public IntPtr hwndMenuOwner;
			public IntPtr hwndMoveSize;
			public IntPtr hwndCaret;
			public RECT rcCaret;
		};

		public struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		};

		#endregion

		#region enums

		public enum WindowShowStyle : uint
		{
			Hide = 0,
			ShowNormal = 1,
			ShowMinimized = 2,
			ShowMaximized = 3,
			Maximize = 3,
			ShowNormalNoActivate = 4,
			Show = 5,
			Minimize = 6,
			ShowMinNoActivate = 7,
			ShowNoActivate = 8,
			Restore = 9,
			ShowDefault = 10,
			ForceMinimized = 11
		}

		public enum InputType
		{
			INPUT_MOUSE = 0,
			INPUT_KEYBOARD = 1,
			INPUT_HARDWARE = 2,
		}

		[Flags()]
		public enum MOUSEEVENTF
		{
			MOVE = 0x0001,  // mouse move 
			LEFTDOWN = 0x0002,  // left button down
			LEFTUP = 0x0004,  // left button up
			RIGHTDOWN = 0x0008,  // right button down
			RIGHTUP = 0x0010,  // right button up
			MIDDLEDOWN = 0x0020,  // middle button down
			MIDDLEUP = 0x0040,  // middle button up
			XDOWN = 0x0080,  // x button down 
			XUP = 0x0100,  // x button down
			WHEEL = 0x0800,  // wheel button rolled
			VIRTUALDESK = 0x4000,  // map to entire virtual desktop
			ABSOLUTE = 0x8000,  // absolute move
		}

		[Flags()]
		private enum KEYEVENTF
		{
			EXTENDEDKEY = 0x0001,
			KEYUP = 0x0002,
			UNICODE = 0x0004,
			SCANCODE = 0x0008,
		}

		#endregion

		#region Constants

		public const int CAPTUREBLT = 0x40000000;
		public const int SRCCOPY = 0x00CC0020;

		public const int INPUT_MOUSE = 0;
		public const int INPUT_KEYBOARD = 1;
		public const int INPUT_HARDWARE = 2;
		public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		public const uint KEYEVENTF_KEYUP = 0x0002;
		public const uint KEYEVENTF_UNICODE = 0x0004;
		public const uint KEYEVENTF_SCANCODE = 0x0008;
		public const uint XBUTTON1 = 0x0001;
		public const uint XBUTTON2 = 0x0002;
		public const uint MOUSEEVENTF_MOVE = 0x0001;
		public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		public const uint MOUSEEVENTF_LEFTUP = 0x0004;
		public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
		public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
		public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
		public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
		public const uint MOUSEEVENTF_XDOWN = 0x0080;
		public const uint MOUSEEVENTF_XUP = 0x0100;
		public const uint MOUSEEVENTF_WHEEL = 0x0800;
		public const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
		public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

		public const int MA_NOACTIVATE = 3;
		public const int CLOSE = 0x0010;
		public const int WM_MOUSEACTIVATE = 0x0021;
		public const int WM_NCACTIVATE = 0x0086;
		public const int WM_ACTIVATEAPP = 0x001C;
		public const int WM_ACTIVATE = 0x0006;

		public const int WM_MOVING = 0x0216;
		public const int GWL_STYLE = (-16);
		public const int GWL_EXSTYLE = (-20);
		public const int GW_OWNER = 4;
		public const int WS_EX_NOACTIVATE = 0x08000000;
		public const int WS_DISABLED = 0x08000000;
		public const int WS_CLIPSIBLINGS = 0x04000000;
		public const int WS_POPUP = unchecked((int)0x80000000);
		public const int WS_EX_TOPMOST = 0x00000008;
		public const int WS_EX_TOOLWINDOW = 0x00000080;
		public const int WS_EX_APPWINDOW = 0x40000;
		public const int WS_EX_TRANSPARENT = 0x00000020;
		public const ulong WS_VISIBLE = 0x10000000L;
		public const ulong WS_BORDER = 0x00800000L;
		public const ulong TARGETWINDOW = WS_BORDER | WS_VISIBLE;

		public const int WM_TOUCHMOVE = 0x0240;
		public const int WM_TOUCHDOWN = 0x0241;
		public const int WM_TOUCHUP = 0x0242;
		public const int WM_TABLET_QUERYSYSTEMGESTURESTATUS = (0x02C0 + 12);
		public const int TABLET_DISABLE_PRESSANDHOLD = 0x00000001;
		public const int TABLET_DISABLE_PENTAPFEEDBACK = 0x00000008;
		public const int TABLET_DISABLE_PENBARRELFEEDBACK = 0x00000010;
		public const int TABLET_DISABLE_TOUCHUIFORCEON = 0x00000100;
		public const int TABLET_DISABLE_TOUCHUIFORCEOFF = 0x00000200;
		public const int TABLET_DISABLE_TOUCHSWITCH = 0x00008000;
		public const int TABLET_DISABLE_FLICKS = 0x00010000;
		public const int TABLET_ENABLE_FLICKSONCONTEXT = 0x00020000;
		public const int TABLET_ENABLE_FLICKLEARNINGMODE = 0x00040000;
		public const int TABLET_DISABLE_SMOOTHSCROLLING = 0x00080000;
		public const int TABLET_DISABLE_FLICKFALLBACKKEYS = 0x00100000;
		public const int TABLET_ENABLE_MULTITOUCHDATA = 0x01000000;

		public const uint GUI_CARETBLINKING = 0x00000001;
		public const uint GUI_INMOVESIZE = 0x00000002;
		public const uint GUI_INMENUMODE = 0x00000004;
		public const uint GUI_SYSTEMMENUMODE = 0x00000008;
		public const uint GUI_POPUPMENUMODE = 0x00000010;

		#endregion

		#region Helper Functions

		public static KEYBDINPUT createKeybdInput(ushort wVK, uint flag) {
			KEYBDINPUT i = new KEYBDINPUT();
			i.wVk = wVK;
			i.wScan = 0;
			i.time = 0;
			i.dwExtraInfo = IntPtr.Zero;
			i.dwFlags = flag;
			return i;
		}

		public static MOUSEINPUT createMouseInput(int x, int y, int data, uint t, uint flag) {
			MOUSEINPUT mi = new MOUSEINPUT();
			mi.dx = x;
			mi.dy = y;
			mi.mouseData = data;
			mi.time = t;
			mi.dwFlags = flag;
			return mi;
		}

		#endregion

		#region IShellLinkW

		[DllImport("shfolder.dll", CharSet = CharSet.Auto)]
		internal static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

		[Flags()]
		enum SLGP_FLAGS
		{
			/// <summary>Retrieves the standard short (8.3 format) file name</summary>
			SLGP_SHORTPATH = 0x1,
			/// <summary>Retrieves the Universal Naming Convention (UNC) path name of the file</summary>
			SLGP_UNCPRIORITY = 0x2,
			/// <summary>Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded</summary>
			SLGP_RAWPATH = 0x4
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct WIN32_FIND_DATAW
		{
			public uint dwFileAttributes;
			public long ftCreationTime;
			public long ftLastAccessTime;
			public long ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint dwReserved0;
			public uint dwReserved1;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string cFileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			public string cAlternateFileName;
		}

		[Flags()]

		enum SLR_FLAGS
		{
			/// <summary>
			/// Do not display a dialog box if the link cannot be resolved. When SLR_NO_UI is set,
			/// the high-order word of fFlags can be set to a time-out value that specifies the
			/// maximum amount of time to be spent resolving the link. The function returns if the
			/// link cannot be resolved within the time-out duration. If the high-order word is set
			/// to zero, the time-out duration will be set to the default value of 3,000 milliseconds
			/// (3 seconds). To specify a value, set the high word of fFlags to the desired time-out
			/// duration, in milliseconds.
			/// </summary>
			SLR_NO_UI = 0x1,
			/// <summary>Obsolete and no longer used</summary>
			SLR_ANY_MATCH = 0x2,
			/// <summary>If the link object has changed, update its path and list of identifiers.
			/// If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine
			/// whether or not the link object has changed.</summary>
			SLR_UPDATE = 0x4,
			/// <summary>Do not update the link information</summary>
			SLR_NOUPDATE = 0x8,
			/// <summary>Do not execute the search heuristics</summary>
			SLR_NOSEARCH = 0x10,
			/// <summary>Do not use distributed link tracking</summary>
			SLR_NOTRACK = 0x20,
			/// <summary>Disable distributed link tracking. By default, distributed link tracking tracks
			/// removable media across multiple devices based on the volume name. It also uses the
			/// Universal Naming Convention (UNC) path to track remote file systems whose drive letter
			/// has changed. Setting SLR_NOLINKINFO disables both types of tracking.</summary>
			SLR_NOLINKINFO = 0x40,
			/// <summary>Call the Microsoft Windows Installer</summary>
			SLR_INVOKE_MSI = 0x80
		}


		/// <summary>The IShellLink interface allows Shell links to be created, modified, and resolved</summary>
		[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
		interface IShellLinkW
		{
			/// <summary>Retrieves the path and file name of a Shell link object</summary>
			void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);
			/// <summary>Retrieves the list of item identifiers for a Shell link object</summary>
			void GetIDList(out IntPtr ppidl);
			/// <summary>Sets the pointer to an item identifier list (PIDL) for a Shell link object.</summary>
			void SetIDList(IntPtr pidl);
			/// <summary>Retrieves the description string for a Shell link object</summary>
			void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
			/// <summary>Sets the description for a Shell link object. The description can be any application-defined string</summary>
			void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
			/// <summary>Retrieves the name of the working directory for a Shell link object</summary>
			void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
			/// <summary>Sets the name of the working directory for a Shell link object</summary>
			void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
			/// <summary>Retrieves the command-line arguments associated with a Shell link object</summary>
			void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
			/// <summary>Sets the command-line arguments for a Shell link object</summary>
			void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
			/// <summary>Retrieves the hot key for a Shell link object</summary>
			void GetHotkey(out short pwHotkey);
			/// <summary>Sets a hot key for a Shell link object</summary>
			void SetHotkey(short wHotkey);
			/// <summary>Retrieves the show command for a Shell link object</summary>
			void GetShowCmd(out int piShowCmd);
			/// <summary>Sets the show command for a Shell link object. The show command sets the initial show state of the window.</summary>
			void SetShowCmd(int iShowCmd);
			/// <summary>Retrieves the location (path and index) of the icon for a Shell link object</summary>
			void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
				int cchIconPath, out int piIcon);
			/// <summary>Sets the location (path and index) of the icon for a Shell link object</summary>
			void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
			/// <summary>Sets the relative path to the Shell link object</summary>
			void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
			/// <summary>Attempts to find the target of a Shell link, even if it has been moved or renamed</summary>
			void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);
			/// <summary>Sets the path and file name of a Shell link object</summary>
			void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);

		}

		[ComImport, Guid("0000010c-0000-0000-c000-000000000046"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IPersist
		{
			[PreserveSig]
			void GetClassID(out Guid pClassID);
		}


		[ComImport, Guid("0000010b-0000-0000-C000-000000000046"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IPersistFile : IPersist
		{
			new void GetClassID(out Guid pClassID);
			[PreserveSig]
			int IsDirty();

			[PreserveSig]
			void Load([In, MarshalAs(UnmanagedType.LPWStr)]
            string pszFileName, uint dwMode);

			[PreserveSig]
			void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
				[In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

			[PreserveSig]
			void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

			[PreserveSig]
			void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
		}

		const uint STGM_READ = 0;
		const int MAX_PATH = 260;

		// CLSID_ShellLink from ShlGuid.h 
		[
			ComImport(),
			Guid("00021401-0000-0000-C000-000000000046")
		]
		public class ShellLink
		{
		}

		public static string ResolveLnk(string filename) {
			ShellLink link = new ShellLink();
			((IPersistFile)link).Load(filename, STGM_READ);
			// TODO: if I can get hold of the hwnd call resolve first. This handles moved and renamed files.  
			// ((IShellLinkW)link).Resolve(hwnd, 0) 
			StringBuilder sb = new StringBuilder(MAX_PATH);
			WIN32_FIND_DATAW data = new WIN32_FIND_DATAW();
			((IShellLinkW)link).GetPath(sb, sb.Capacity, out data, 0);
			return sb.ToString();
		}

		#endregion

		[DllImport("dwmapi.dll")]
		public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

		[DllImport("dwmapi.dll")]
		public static extern int DwmUnregisterThumbnail(IntPtr thumb);

		[DllImport("dwmapi.dll")]
		public static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out PSIZE size);

		[StructLayout(LayoutKind.Sequential)]
		public struct PSIZE
		{
			public int x;
			public int y;
		}

		[DllImport("dwmapi.dll")]
		public static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

		[StructLayout(LayoutKind.Sequential)]
		public struct DWM_THUMBNAIL_PROPERTIES
		{
			public int dwFlags;
			public Rect rcDestination;
			public Rect rcSource;
			public byte opacity;
			public bool fVisible;
			public bool fSourceClientAreaOnly;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
			public Rect(int left, int top, int right, int bottom) {
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
	}

}
