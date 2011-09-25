using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;
using MayhemCore;
using MayhemWpf.UserControls;
using MessagingToolkit.QRCode.Codec;

namespace PhoneModules.Wpf
{
    /// <summary>
    /// Interaction logic for ReactionFromWP7Config.xaml
    /// </summary>
    public partial class ReactionFromWP7Config : WpfConfiguration
    {
        private string id;

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
            textEvent.Text = id;
        }

        public override string Title
        {
            get { return "Reaction From WP7"; }
        }
    }
}
