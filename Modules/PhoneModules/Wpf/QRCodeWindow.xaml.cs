using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessagingToolkit.QRCode.Codec;
using System.Net;
using MayhemCore;
using System.IO;
using System.Drawing.Imaging;

namespace PhoneModules.Wpf
{
	/// <summary>
	/// Interaction logic for QRCodeWindow.xaml
	/// </summary>
	public partial class QRCodeWindow : Window
	{
		public QRCodeWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
			qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;

			IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
			string text = string.Empty;
			text += "19283";
			textPort.Text = "19283";
			foreach (IPAddress t in localIPs)
			{
				if (t.AddressFamily.ToString() == "InterNetwork")
				{
					text += ":" + t;
					if (textIP.Text.Length > 0)
						textIP.Text += ", ";
					textIP.Text += t;
				}
			}

			System.Drawing.Bitmap image = null;
			try
			{
				image = qrCodeEncoder.Encode(text);
			}
			catch (Exception erf)
			{
				Logger.WriteLine(erf);
			}

			MemoryStream ms = new MemoryStream();
			image.Save(ms, ImageFormat.Png);
			ms.Position = 0;
			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = ms;
			bi.EndInit();
			imageQR.Source = bi;
		}
	}
}
