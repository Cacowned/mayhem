using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using WindowModules.Actions;

namespace WindowModules.Wpf
{
	/// <summary>
	/// Interaction logic for MoveWindowConfig.xaml
	/// </summary>
	public partial class WindowSequenceConfig : WpfConfiguration
	{
		public WindowInfo SelectedWindow
		{
			get;
			private set;
		}

		public WindowActionInfo ActionInfo
		{
			get;
			private set;
		}

		private Timer timer;
		private Process thisProcess;
		private IntPtr thisWindowHandle;

		private readonly Dictionary<UserControl, IWindowAction> controlMap;

		public static IntPtr CurrentlySelectedWindow;

		// True if we are starting up and shouldn't verify till after everything is set.
		private readonly bool isStartingUp;

		public WindowSequenceConfig(WindowActionInfo windowActionInfo)
		{
			isStartingUp = true;

			InitializeComponent();
			ActionInfo = windowActionInfo;

			controlMap = new Dictionary<UserControl, IWindowAction>();
			CurrentlySelectedWindow = IntPtr.Zero;

			SelectedWindow = ActionInfo.WindowInfo;
			textBoxApplication.Text = ActionInfo.WindowInfo.FileName;
			textBoxWindowTitle.Text = ActionInfo.WindowInfo.Title;
			checkBoxApplication.IsChecked = ActionInfo.WindowInfo.CheckFileName;
			checkBoxTitle.IsChecked = ActionInfo.WindowInfo.CheckTitle;

			foreach (IWindowAction action in windowActionInfo.WindowActions)
			{
				Add(action);
			}

			isStartingUp = false;
		}

		public override void OnLoad()
		{
			thisProcess = Process.GetCurrentProcess();
			thisWindowHandle = thisProcess.MainWindowHandle;
			timer = new Timer(500);
			timer.Elapsed += TimerElapsed;
			timer.Start();
			CheckCanSave();

			if (textBoxApplication.Text != null || textBoxWindowTitle.Text != null)
			{
				WindowFinder.Find(ActionInfo, hwnd =>
												  {
													  CurrentlySelectedWindow = hwnd;
												  });
			}
		}

		public static int GetProcessThreadFromWindow(IntPtr hwnd)
		{
			int procid = 0;
			Native.GetWindowThreadProcessId(hwnd, ref procid);
			return procid;
		}

		private void TimerElapsed(object sender, ElapsedEventArgs e)
		{
			IntPtr handle = Native.GetForegroundWindow();

			if (handle == IntPtr.Zero || handle == thisWindowHandle)
				return;

			int procId = GetProcessThreadFromWindow(handle);
			Process p = Process.GetProcessById(procId);

			if (p.MainWindowHandle == IntPtr.Zero || p.MainWindowHandle == thisWindowHandle)
				return;

			string filename;

			CurrentlySelectedWindow = handle;

			try
			{
				filename = p.MainModule.FileName;
			}
			catch
			{
				filename = WMIProcess.GetFilename(procId);
			}

			if (filename != null)
			{
				FileInfo fi = new FileInfo(filename);
				filename = fi.Name;
			}

			StringBuilder sb = new StringBuilder(200);
			Native.GetWindowText(handle, sb, sb.Capacity);
			string title = sb.ToString();

			if (SelectedWindow == null ||
				SelectedWindow.FileName != filename ||
				SelectedWindow.Title != title)
			{
				SelectedWindow.FileName = filename;
				SelectedWindow.Title = title;
				Dispatcher.Invoke((Action)delegate
				{
					textBoxApplication.Text = filename;
					textBoxWindowTitle.Text = title;
					textBoxApplication.CaretIndex = textBoxApplication.Text.Length;
				});
			}
		}

		public override void OnSave()
		{
			timer.Stop();
			timer = null;

			ActionInfo.WindowInfo.CheckFileName = checkBoxApplication.IsChecked == true;
			ActionInfo.WindowInfo.CheckTitle = checkBoxTitle.IsChecked == true;
			ActionInfo.WindowActions.Clear();
			foreach (WindowActionControl wac in stackPanelActions.Children)
			{
				((IWindowActionConfigControl)wac.Config).Save();
				ActionInfo.WindowActions.Add(controlMap[wac.Config]);
			}
		}

		public override void OnCancel()
		{
			timer.Stop();
			timer = null;
		}

		public override string Title
		{
			get { return "Window Sequence"; }
		}

		private void Add(IWindowAction action)
		{
			UserControl newControl = null;
			if (action is WindowActionBringToFront)
				newControl = new WindowBringToFront();
			else if (action is WindowActionClose)
				newControl = new WindowClose();
			else if (action is WindowActionMaximize)
				newControl = new WindowMaximize();
			else if (action is WindowActionMinimize)
				newControl = new WindowMinimize();
			else if (action is WindowActionMove)
				newControl = new WindowMove((WindowActionMove)action);
			else if (action is WindowActionResize)
				newControl = new WindowResize((WindowActionResize)action);
			else if (action is WindowActionRestore)
				newControl = new WindowRestore();
			else if (action is WindowActionSendKeys)
				newControl = new WindowSendKeys((WindowActionSendKeys)action);
			else if (action is WindowActionWait)
				newControl = new WindowWait((WindowActionWait)action);

			WindowActionControl wac = new WindowActionControl(newControl);
			wac.Deleted += ContDeleted;
			stackPanelActions.Children.Add(wac);
			wac.Index = stackPanelActions.Children.Count;

			// scroll to the bottom of the viewer.
			actionScroller.ScrollToBottom();

			controlMap.Add(newControl, action);
		}

		private void ContDeleted(object sender, EventArgs e)
		{
			WindowActionControl wac = sender as WindowActionControl;

			int removedIndex = wac.Index;
			stackPanelActions.Children.Remove(wac);
			controlMap.Remove(wac);

			// we removed an item, decrement all the following items by 1
			// start down by one because we are presenting at one based index
			for (int i = removedIndex-1; i < stackPanelActions.Children.Count; i++)
			{
				((WindowActionControl)stackPanelActions.Children[i]).Index--; 
			}

			CheckCanSave();
		}

		private void ButtonAdd_Click(object sender, RoutedEventArgs e)
		{
			IWindowAction action;
			string name = ((ComboBoxItem)comboBoxActions.SelectedItem).Content as string;
			switch (name)
			{
				case "Bring to front":
					action = new WindowActionBringToFront();
					break;
				case "Close":
					action = new WindowActionClose();
					break;
				case "Maximize":
					action = new WindowActionMaximize();
					break;
				case "Minimize":
					action = new WindowActionMinimize();
					break;
				case "Move":
					action = new WindowActionMove();
					break;
				case "Restore":
					action = new WindowActionRestore();
					break;
				case "Resize":
					action = new WindowActionResize();
					break;
				case "Send keys":
					action = new WindowActionSendKeys();
					break;
				case "Wait":
					action = new WindowActionWait();
					break;
				default:
					return;
			}

			Add(action);
			CheckCanSave();
		}

		private void CheckCanSave()
		{
			if (!isStartingUp)
			{
				CanSave = ((textBoxApplication.Text.Length > 0 && checkBoxApplication.IsChecked == true) ||
						   (textBoxWindowTitle.Text.Length > 0 && checkBoxTitle.IsChecked == true)) &&
						  stackPanelActions.Children.Count > 0;
			}
		}

		private void textBox_TextChanged(object sender, TextChangedEventArgs e)
		{

			CheckCanSave();
		}

		private void checkBox_Checked(object sender, RoutedEventArgs e)
		{
			CheckCanSave();

		}
	}
}
