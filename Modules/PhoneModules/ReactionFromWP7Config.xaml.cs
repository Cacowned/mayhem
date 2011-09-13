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
using MayhemDefaultStyles.UserControls;
using MessagingToolkit.QRCode.Codec;
using System.Net;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;

namespace PhoneModules
{
    /// <summary>
    /// Interaction logic for ReactionFromWP7Config.xaml
    /// </summary>
    public partial class ReactionFromWP7Config : IWpfConfiguration
    {
        string id;
        public ReactionFromWP7Config(string id)
        {
            InitializeComponent();
            this.id = id;
        }

        public override void OnLoad()
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;

            string localComputerName = Dns.GetHostName();
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            string text = "";
            text += "19283:";
            textPort.Text = "19283";
            for (int i = 0; i < localIPs.Length; i++)
            {
                if (localIPs[i].AddressFamily.ToString() == "InterNetwork")
                {
                    text += localIPs[i] + ":";
                    if (textIP.Text.Length > 0)
                        textIP.Text += ", ";
                    textIP.Text += localIPs[i];
                }
            }
            text += id;
            Bitmap image = null;
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
            textEvent.Text = id;
        }

        public override string Title
        {
            get { return "Reaction From WP7"; }
        }

        public override bool OnSave()
        {
            return true;
        }

        public override void OnCancel()
        {
        }
    }
}
