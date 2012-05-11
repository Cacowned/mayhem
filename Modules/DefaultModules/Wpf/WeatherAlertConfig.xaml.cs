using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// This is the configuration file for the WeatherAlert event
    /// The event is explained in WeatherAlert.cs, the configuration
    /// file checks for valid zip code, city name, and temperature fields.
    /// </summary>
    public partial class WeatherAlertConfig : WpfConfiguration
    {
        private XmlReader reader;
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
            Temperature.Text = TempProp.ToString(CultureInfo.InvariantCulture);
            if (CheckAbove)
            {
                Above.IsChecked = true;
            }
            else
            {
                Below.IsChecked = true;
            }
        }

        public override void OnSave()
        {
            ZipCodeProp = ZipCode.Text;
            TempProp = Convert.ToInt32(Temperature.Text);
            CheckAbove = (bool)Above.IsChecked;
        }

        /// <summary>
        /// Verifies that the entered zip code, city name, and temperature to watch are valid
        /// and within range (-80 -> 180 deg F for temp)
        /// </summary>
        private void VerifyFields()
        {
            string error = "Invalid";
            if (!timer.IsEnabled)
            {
                int temp;
                bool isTemp = int.TryParse(Temperature.Text, out temp) && (temp < 180 && temp > -80);

                error += isTemp ? string.Empty : " temperature";
                error += ZipCode.Text.Length > 0 && IsValidXML() ? string.Empty : " zip code or city name";

                CanSave = error.Equals("Invalid");
            }
            else
            {
                error = "Cannot connect to the Internet";
            }

            TextChanged(error);
        }

        /// <summary>
        /// If there is an Internet connection, verifies that the xml is valid, I.E. the entered zip
        /// code or city name returns valid xml
        /// </summary>
        /// <returns>true if the xml is valid</returns>
        private bool IsValidXML()
        {
            if (!timer.IsEnabled)
            {
                try
                {
                    // get the xml data
                    string url = "http://www.google.com/ig/api?weather=" + ZipCode.Text.Replace(" ", "%20");
                    WebRequest webRequest = WebRequest.Create(url);
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (Stream responseStream = webResponse.GetResponseStream())
                        {
                            StreamReader s = new StreamReader(responseStream, Encoding.GetEncoding(1252));
                            XmlReader r = XmlReader.Create(s);

                            // check that there is xml data
                            for (int i = 0; i < 4; i++)
                            {
                                r.Read();
                            }

                            if (r.Name.Equals("problem_cause"))
                            {
                                ZipCity.Text = "City";
                                TempInfo.Text = "Current Temperature: N/A";
                                return false;
                            }

                            reader = r;
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            return false;
        }

        /// <summary>
        /// Bucket handler for when the zip code and temperature fields are changed
        /// </summary>
        private void TempZip_TextChanged(object sender, RoutedEventArgs e)
        {
            if (ConnectedToInternet())
            {
                VerifyFields();
            }
            else
            {
                ZipCity.Text = "City";
                TextChanged("Cannot connect to the Internet");
            }
        }

        /// <summary>
        /// Bucket function for whenever text is changed, this is passed a string.
        /// If the user can not save, the error string is displayed, otherwise they can save.
        /// </summary>
        /// <param name="text">The error text to display if fields are not valid</param>
        private void TextChanged(string text)
        {
            textInvalid.Text = text;
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            if (CanSave && !timer.IsEnabled)
            {
                CheckWeather();
            }
        }

        /// <summary>
        /// Pulls the city name and icon link from the xml
        /// If there is any xml encoding issues, they are caught here
        /// and the user is not allowed to save.
        /// </summary>
        private void CheckWeather()
        {
            try
            {
                reader.ReadToFollowing("city");
                city = reader.GetAttribute("data");
                reader.ReadToFollowing("temp_f");
                string currentTemp = reader.GetAttribute("data");
                reader.ReadToFollowing("icon");
                icon = reader.GetAttribute("data");

                reader.Close();

                ZipCity.Text = city;
                TempInfo.Text = String.Format("Current Temperature: {0}F", currentTemp);
                UpdateImage();
            }
            catch
            {
                CanSave = false;
                TextChanged("Invalid characters in returned data for " + ZipCode.Text);
            }
        }

        /// <summary>
        /// Grabs the weather icon using the url from the xml returned
        /// by the weather service
        /// </summary>
        private void UpdateImage()
        {
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("http://www.google.com/" + icon, UriKind.Absolute);
                bi.EndInit();

                WeatherIcon.Source = bi;
            }
            catch
            {
                CanSave = false;
                TextChanged("Cannot connect to the Internet");
            }
        }

        /// <summary>
        /// Checks if there is a current Internet connection, if there is not
        /// starts a timer that checks every 2 seconds, and when Internet is
        /// again connected, checks for valid text fields
        /// </summary>
        #region CheckInternet
        private bool ConnectedToInternet()
        {
            try
            {
                // check Internet connection (ping google w/ 650ms timeout
                CallWithTimeout(MakeRequest, 650);
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    VerifyFields();
                }

                return true;
            }
            catch
            {
            }

            if (!timer.IsEnabled)
            {
                timer.Start();
            }

            return false;
        }

        /// <summary>
        /// Make a request to google.com
        /// </summary>
        private static void MakeRequest()
        {
            IPHostEntry obj = Dns.GetHostEntry("www.google.com");
        }

        /// <summary>
        /// A method that will trigger a "timeout" error to be used when running a method with a 
        /// specific timout trigger
        /// </summary>
        /// <param name="action">The method being run</param>
        /// <param name="timeout">The timeout in milliseconds</param>
        public static void CallWithTimeout(Action action, int timeout)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeout))
            {
                wrappedAction.EndInvoke(result);
                //throw new TimeoutException();
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }
        #endregion
    }
}
