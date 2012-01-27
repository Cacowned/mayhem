using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class WeatherAlertConfig : WpfConfiguration
    {
        private XmlTextReader reader;
        private string city;
        private string icon;
        private DispatcherTimer timer;

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

        public override string Title
        {
            get
            {
                return "Weather Alert";
            }
        }

        public WeatherAlertConfig(string zipcode, int temp, bool checks)
        {
            ZipCodeProp = zipcode;
            TempProp = temp;
            CheckAbove = checks;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += CheckInternet;

            InitializeComponent();
        }

        private void CheckInternet(object sender, EventArgs e)
        {
            // don't want to add overloads to ConnectedToInternet()
            ConnectedToInternet();
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

            // done when text fields are set above
            // VerifyFields();
            UpdateImage();
        }

        public override void OnSave()
        {
            ZipCodeProp = ZipCode.Text;
            TempProp = Convert.ToInt32(Temperature.Text);
            CheckAbove = (bool)Above.IsChecked;
        }

        private void VerifyFields()
        {
            string error = "Invalid";
            if (ConnectedToInternet())
            {
                int temp;
                bool isTemp = int.TryParse(Temperature.Text, out temp) && (temp < 150 && temp > -50);

                error += isTemp ? "" : " temperature";
                error += IsValidXML() ? "" : " zip code or city name";

                CanSave = error.Equals("Invalid");
            }
            else
            {
                error = "Cannot connect to the Internet";
            }
            TextChanged(error);
        }

        private bool IsValidXML()
        {
            if (ConnectedToInternet() && ZipCode.Text.Length > 0)
            {
                try
                {
                    reader = new XmlTextReader("http://www.google.com/ig/api?weather=" + ZipCode.Text.Replace(" ", "%20"));

                    // check that there is xml data
                    for (int i = 0; i < 4; i++)
                    {
                        reader.Read();
                    }
                    if (reader.Name.Equals("problem_cause"))
                    {
                        ZipCity.Text = "City";
                        return false;
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        private void TempZip_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                VerifyFields();
            }
            else
            {
                ZipCity.Text = "City";
                TextChanged("Cannot connect to the Internet");
            }
        }

        private void TextChanged(string text)
        {
                textInvalid.Text = text;
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
                if (CanSave && !timer.IsEnabled)
                {
                    CheckWeather();
                }
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
            if (ConnectedToInternet())
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("http://www.google.com/" + icon, UriKind.Absolute);
                bi.EndInit();

                WeatherIcon.Source = bi;
            }
            else
            {
                TextChanged("Cannot connect to the Internet");
            }
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int desciption, int reservedValue);

        private bool ConnectedToInternet()
        {
            int desc;
            try
            {
                if (InternetGetConnectedState(out desc, 0))
                {
                    if (timer.IsEnabled)
                    {
                        timer.Stop();
                        VerifyFields();
                    }
                    return true;
                }
            }
            catch { }

            if (!timer.IsEnabled)
            {
                timer.Start();
            }
            return false;
        }
    }
}
