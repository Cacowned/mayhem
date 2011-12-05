using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MayhemCore;
using MayhemWpf.UserControls;
using MessagingToolkit.QRCode.Codec;
using PhoneModules.Controls;

namespace PhoneModules.Wpf
{
	public partial class PhoneFormDesigner : WpfConfiguration
	{
		public PhoneUIElement SelectedElement
		{
			get;
			private set;
		}

		private readonly PhoneLayout phoneLayout;

		private string selectedId;
		private readonly bool isCreatingForFirstTime;

		// This should keep track of whether the user has ever been
		// inside of this configuration
		private static bool beenInConfig = false;

		public PhoneFormDesigner(bool isCreatingForFirstTime)
		{
			phoneLayout = PhoneLayout.Instance;
			this.isCreatingForFirstTime = isCreatingForFirstTime;

			// If we are creating for the first time, then disable the Save button
			// until they click the pair with phone button. If they have created before
			// then the save will be enabled.
			CanSave = beenInConfig;

			InitializeComponent();
		}

		public void LoadFromData(string data, string selectedId)
		{
			LoadFromData(selectedId);
		}

		public override void OnLoad()
		{
			if (!PhoneConnector.Instance.IsServiceRunning)
			{
				ThreadPool.QueueUserWorkItem(o => PhoneConnector.Instance.Enable(false));
			}
		}

		public void LoadFromData(string selectedID)
		{
			selectedId = selectedID;
			foreach (PhoneLayoutButton layout in phoneLayout.Buttons)
			{
				if (layout.IsEnabled || layout.Id == selectedID)
				{
					PhoneUIElementButton button = new PhoneUIElementButton();
					button.Text = layout.Text;
					button.ImageFile = layout.ImageFile;
					button.Tag = layout.Id;
					button.LayoutInfo = layout;
					Canvas.SetLeft(button, layout.X);
					Canvas.SetTop(button, layout.Y);
					canvas1.Children.Add(button);
					if (layout.Id == selectedID)
					{
						SelectedElement = button;
						button.IsSelected = true;
					}

					if (!layout.IsEnabled)
					{
						textErrorButtonDisabled.Visibility = Visibility.Visible;
						button.Opacity = 0.5;
					}
				}
			}
		}

		private void canvas1_MouseDown(object sender, MouseButtonEventArgs e)
		{
			foreach (UIElement el in canvas1.Children)
			{
				if (el is PhoneUIElementButton)
				{
					((PhoneUIElementButton)el).CanvasClicked();
				}
			}
		}

		public override string Title
		{
			get { return "Phone Remote"; }
		}

		public override void OnSave()
		{
			for (int i = 0; i < canvas1.Children.Count; i++)
			{
				if (canvas1.Children[i] is PhoneUIElementButton)
				{
					PhoneUIElementButton button = canvas1.Children[i] as PhoneUIElementButton;
					button.LayoutInfo.ImageFile = button.ImageFile;
					button.LayoutInfo.Text = button.Text;
					button.LayoutInfo.X = Canvas.GetLeft(button) + (button.IsGridOnRight ? 0 : button.gridEdit.ActualWidth);
					button.LayoutInfo.Y = Canvas.GetTop(button);
					button.LayoutInfo.Width = button.border1.ActualWidth;
					button.LayoutInfo.Height = button.border1.ActualHeight;
				}
			}
		}

		public override void OnCancel()
		{
			if (isCreatingForFirstTime)
			{
				phoneLayout.RemoveButton(selectedId);
			}
		}

		private void PairButton_Click(object sender, RoutedEventArgs e)
		{
			var qrWindow = new QRCodeWindow();
			qrWindow.ShowDialog();

			CanSave = beenInConfig = true;
		}
	}
}
