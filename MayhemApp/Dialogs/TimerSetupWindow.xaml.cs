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
using System.Windows.Shapes;
using System.Diagnostics;

namespace MayhemApp
{
    /// <summary>
    /// Interaction logic for TimerSetupWindow.xaml
    /// </summary>
    public partial class TimerSetupWindow : Window
    {
        // the MotionDetectionTrigger listens to this event in order to set the motion detection window
        public delegate void SetButtonClickedHandler(object sender, RoutedEventArgs e);
        public event SetButtonClickedHandler OnSetButtonClicked;

        private List<int> hours = new List<int>();
        private List<int> minutes = new List<int>();
        private List<int> seconds = new List<int>();

        private int selected_hours_;
        private int selected_mins_;
        private int selected_seconds_;

        public  int selected_hours
        {
            get
            {
                return selected_hours_;
            }
            set
            {
                selected_hours_ = value;
                box_hrs.SelectedItem = hours.IndexOf(value);
            }

        }

        public int selected_minutes
        {
            get
            {
                return selected_mins_;
            }
            set
            {
                selected_mins_ = value;
                box_mins.SelectedItem = minutes.IndexOf(value) ;
            }

        }

        public int selected_seconds
        {
            get
            {
                return selected_seconds_;
            }
            set
            {
                selected_seconds_ = value;
                // seconds don't start at 0 so they get an extra increment
                box_secs.SelectedItem = seconds.IndexOf(value)+1;
            }

        }





        public TimerSetupWindow()
        {

            InitializeComponent();

            for (int i = 0; i < 25; i++)
            {
                hours.Add(i);
            }
            for (int i = 0; i < 60; i++)
            {
                minutes.Add(i);
                if (i>0)
                    seconds.Add(i);
            }

            box_hrs.ItemsSource = hours;
            box_mins.ItemsSource = minutes;
            box_secs.ItemsSource = seconds;

            box_hrs.SelectedIndex = 0;
            box_mins.SelectedIndex = 0;
            box_secs.SelectedIndex = 0;

            selected_hours_ = 0;
            selected_mins_ = 0;
            selected_seconds_ = 1;
            
        }


        public void SetupWindowCloseButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }


        #region reset and set buttons

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // set button is clicked

            if (this.OnSetButtonClicked != null)
            {
                OnSetButtonClicked(this, e);
            }

        }
        #endregion

        private void box_hrs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_hours_ = (int) box_hrs.SelectedItem;
            PrintSelections();
        }

        private void box_mins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_mins_ = (int)box_mins.SelectedItem;
            PrintSelections();
        }

        private void box_secs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_seconds_ = (int)box_secs.SelectedItem;
            PrintSelections();
        }

        private void PrintSelections()
        {
            Debug.WriteLine("[TimerSetupWindow] : h " + selected_hours_ + " m " + selected_mins_ + " s " + selected_seconds_);
        }

    }
}
