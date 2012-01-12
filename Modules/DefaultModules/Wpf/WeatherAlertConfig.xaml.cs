using MayhemWpf.UserControls;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Xml;
using MayhemCore;
using DefaultModules.Resources;
using System.Runtime.InteropServices;

namespace DefaultModules.Wpf
{

    public partial class WeatherAlertConfig : WpfConfiguration
    {
        public string ZipCodeProp
        {
            get;
            private set;
        }

        public int TempProp
        {
            get;
            private set;
        }

        public bool CheckAbove
        {
            get;
            private set;
        }

        private XmlTextReader reader;
        private string city;
        private string icon;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Desciption, int ReservedValue);

        public WeatherAlertConfig(string zipcode, int temp, bool checks)
        {
            ZipCodeProp = zipcode;
            TempProp = temp;
            CheckAbove = checks;
            InitializeComponent();
        }

        private bool IsValid()
        {
            if (ConnectedToInternet())
            {
                reader = new XmlTextReader("http://www.google.com/ig/api?weather=" + ZipCode.Text.Replace(" ", "%20"));

                // check that there is xml data
                for (int i = 0; i < 4; i++)
                {
                    reader.Read();
                }
                return !reader.Name.Equals("problem_cause");
            }
            return false;
        }

        private void TempZip_TextChanged(object sender, RoutedEventArgs e)
        {
            if (ConnectedToInternet())
            {
                string error = "Invalid";

                int temp;
                bool isTemp = int.TryParse(Temperature.Text, out temp) && (temp < 150 && temp > -50);
                bool isZip = (ZipCode.Text.Length > 0 && IsValid());

                if (!isTemp)
                {
                    error += " temperature";
                }

                if (!isZip)
                {
                    error += " zip code or city name";
                }

                CanSave = isZip && isTemp;
                TextChanged(error);
            }
            else
            {
                CanSave = false;
                TextChanged("Not connected to the Internet");
            }
        }

        private void TextChanged(string text)
        {
            textInvalid.Text = text;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            if (CanSave)
            {
                CheckWeather();
            }
        }

        public override string Title
        {
            get
            {
                return "Weather Alert";
            }
        }

        public override void OnLoad()
        {
            ZipCode.Text = ZipCodeProp;
            Temperature.Text = TempProp.ToString();
            if (CheckAbove)
            {
                Above.IsChecked = true;
            }
            else
            {
                Below.IsChecked = true;
            }
            IsValid();
            UpdateImage();

        }

        public override void OnSave()
        {
            ZipCodeProp = ZipCode.Text;
            TempProp = Convert.ToInt32(Temperature.Text);
            CheckAbove = (bool)Above.IsChecked;
        }

        private void CheckWeather()
        {
            reader.ReadToFollowing("city");
            city = reader.GetAttribute("data");
            reader.ReadToFollowing("icon");
            icon = reader.GetAttribute("data");

            ZipCity.Text = city;
            UpdateImage();
        }

        private void UpdateImage()
        {
            if (ConnectedToInternet()) // excessive?
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("http://www.google.com/" + icon, UriKind.Absolute);
                bi.EndInit();

                WeatherIcon.Source = bi;
            }
            else
            {
                TextChanged("Not connected to the Internet");
            }
        }

        public static bool ConnectedToInternet()
        {
            int Desc;
            try
            {
                return InternetGetConnectedState(out Desc, 0);
            }
            catch
            {
                return false;
            }
        }
    }
}
