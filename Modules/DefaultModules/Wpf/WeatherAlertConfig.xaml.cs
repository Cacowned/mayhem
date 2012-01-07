using MayhemWpf.UserControls;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Xml;
using MayhemCore;
using DefaultModules.Resources;

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

        public WeatherAlertConfig(string zipcode, int temp, bool checks)
        {
            ZipCodeProp = zipcode;
            TempProp = temp;
            CheckAbove = checks;
            InitializeComponent();
        }

        private void ZipCode_TextChanged(object sender, RoutedEventArgs e)
        {
            CanSave = (ZipCode.Text.Length > 0 && IsValid());
            //CanSave = true;
            TextChanged("Invalid zip code or city name");
        }

        private bool IsValid()
        {
            reader = new XmlTextReader("http://www.google.com/ig/api?weather=" + ZipCode.Text.Replace(" ", "%20"));

            // check valid url (has internet)
            try
            {
                reader.Read();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Internet_NotConnected);
            }
            // check valid zip
            for (int i = 0; i < 3; i++)
            {
                reader.Read();
            }
            return !reader.Name.Equals("problem_cause");
        }

        private void Temperature_TextChanged(object sender, RoutedEventArgs e)
        {
            CanSave = true;
            int temp;
            bool isInt = int.TryParse(Temperature.Text, out temp) && (temp < 150 && temp > -50);
            if (!isInt)
            {
                CanSave = false;
            }
            TextChanged("Invalid temperature");
        }

        private void TextChanged(string text)
        {
            textInvalid.Text = text;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            if (CanSave)
            {
                UpdateImage();
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
            UpdateImage();
            
        }

        public override void OnSave()
        {
            ZipCodeProp = ZipCode.Text;
            TempProp = Convert.ToInt32(Temperature.Text);
            CheckAbove = (bool)Above.IsChecked;
        }

        private string CheckWeather()
        {
            // Read nodes one at a time  
            while (reader.Read())
            {
                if (reader.Name.Equals("city"))
                {
                    city = reader.GetAttribute("data");
                }
                else if (reader.Name.Equals("icon"))
                {
                    icon = reader.GetAttribute("data");
                }
            }
            return "";
        }

        private void UpdateImage()
        {
            CheckWeather();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("http://www.google.com/" + icon, UriKind.Absolute);
            bi.EndInit();
            WeatherIcon.Source = bi;

            ZipCity.Text = city;
        }

        private bool HaveInternet(XmlTextReader reader)
        {
            try
            {
                reader.Read();
                return true;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Internet_NotConnected);
                return false;
            }
        }
    }
}
