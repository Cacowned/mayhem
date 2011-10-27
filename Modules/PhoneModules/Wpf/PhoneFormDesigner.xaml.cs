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

        public PhoneFormDesigner(bool isCreatingForFirstTime)
        {
            phoneLayout = PhoneLayout.Instance;
            this.isCreatingForFirstTime = isCreatingForFirstTime;
            InitializeComponent();
        }

        public void LoadFromData(string data, string selectedId)
        {
            LoadFromData(selectedId);
        }

        public override void OnLoad()
        {
            CanSave = true;
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
            get { return "Phone"; }
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
    }
}
