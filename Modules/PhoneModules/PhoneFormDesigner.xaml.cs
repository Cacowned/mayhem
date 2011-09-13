using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using PhoneModules.Controls;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System;
using MayhemDefaultStyles.UserControls;
using MessagingToolkit.QRCode.Codec;
using System.Net;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace PhoneModules
{
    public partial class PhoneFormDesigner : IWpfConfiguration
    {
        PhoneLayout phoneLayout = PhoneLayout.Instance;

        public PhoneUIElement SelectedElement
        {
            get;
            private set;
        }

        string selectedID;
        bool isCreatingForFirstTime;

        public PhoneFormDesigner(bool isCreatingForFirstTime)
        {
            this.isCreatingForFirstTime = isCreatingForFirstTime;
            InitializeComponent();
        }

        public void LoadFromData(string data, string selectedId)
        {
            phoneLayout.Deserialize(data);
            LoadFromData(selectedId);
        }

        public override void OnLoad()
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;

            string localComputerName = Dns.GetHostName();
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            string text = "";
            text += "19283:";
            for (int i = 0; i < localIPs.Length; i++)
            {
                if (localIPs[i].AddressFamily.ToString() == "InterNetwork")
                {
                    text += localIPs[i] + ":";
                }
            }
            System.Drawing.Bitmap image = null;
            try
            {
                image = qrCodeEncoder.Encode(text);
            }
            catch (Exception erf)
            {
                Debug.WriteLine(erf);
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

        public void LoadFromData(string selectedID)
        {
            this.selectedID = selectedID;
            for (int i = 0; i < phoneLayout.Buttons.Count; i++)
            {
                PhoneLayoutButton layout = phoneLayout.Buttons[i];
                if (layout.IsEnabled || layout.ID == selectedID)
                {
                    PhoneUIElementButton button = new PhoneUIElementButton();
                    button.Text = layout.Text;
                    button.ImageFile = layout.ImageFile;
                    button.Tag = layout.ID;
                    button.LayoutInfo = layout;
                    Canvas.SetLeft(button, layout.X);
                    Canvas.SetTop(button, layout.Y);
                    canvas1.Children.Add(button);
                    if (layout.ID == selectedID)
                    {
                        SelectedElement = button;
                        button.IsSelected = true;
                    }
                    if (!layout.IsEnabled)
                    {
                        textErrorButtonDisabled.Visibility = System.Windows.Visibility.Visible;
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

        public override bool OnSave()
        {
            for (int i = 0; i < canvas1.Children.Count; i++)
            {
                if (canvas1.Children[i] is PhoneUIElementButton)
                {
                    PhoneUIElementButton button = canvas1.Children[i] as PhoneUIElementButton;
                    button.LayoutInfo.ImageFile = button.ImageFile;
                    button.LayoutInfo.Text = button.Text;
                    button.LayoutInfo.X = Canvas.GetLeft(button);
                    button.LayoutInfo.Y = Canvas.GetTop(button);
                }
            }
            return true;
        }

        public override void OnCancel()
        {
            if (isCreatingForFirstTime)
            {
                phoneLayout.RemoveButton(selectedID);
            }
        }
    }
}
