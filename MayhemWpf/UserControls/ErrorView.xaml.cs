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
using System.Collections.ObjectModel;
using MayhemCore;
using System.Diagnostics;
using System.Windows.Threading;

namespace MayhemWpf.UserControls
{
    /// <summary>
    /// Interaction logic for ErrorView.xaml
    /// </summary>
    public partial class ErrorView : UserControl
    {
        public ObservableCollection<Error> Errors
        {
            get { return (ObservableCollection<Error>)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Errors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(ObservableCollection<Error>), typeof(ErrorView), new UIPropertyMetadata(new ObservableCollection<Error>()));

        public static readonly RoutedEvent ShowEvent = EventManager.RegisterRoutedEvent(
            "Show", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ErrorView));

        public event RoutedEventHandler Show
        {
            add { AddHandler(ShowEvent, value); }
            remove { RemoveHandler(ShowEvent, value); }
        }

        public static readonly RoutedEvent HideEvent = EventManager.RegisterRoutedEvent(
            "Hide", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ErrorView));

        DispatcherTimer leaveTimer = null;

        public event RoutedEventHandler Hide
        {
            add { AddHandler(HideEvent, value); }
            remove { RemoveHandler(HideEvent, value); }
        }
        
        public ErrorView()
        {
            InitializeComponent();

            Errors = ErrorLog.Errors;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Border: " + e.GetPosition(this));
            RaiseEvent(new RoutedEventArgs(ShowEvent));
            //Mouse.Capture(this);
            //e.Handled = true;
        }

        private void MayhemErrorView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.GetPosition(this).Y < borderNumber.Height)
            //{
            //    Debug.WriteLine("Self: " + e.GetPosition(this));
            //    RaiseEvent(new RoutedEventArgs(HideEvent));
            //    Mouse.Capture(null);
            //}
        }

        private void MayhemErrorView_MouseLeave(object sender, MouseEventArgs e)
        {
            if (leaveTimer != null)
                leaveTimer.Stop();
            leaveTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 2) };
            leaveTimer.Tick += new EventHandler(leaveTimer_Tick);
            leaveTimer.Start();
        }

        void leaveTimer_Tick(object sender, EventArgs e)
        {
            leaveTimer.Stop();
            leaveTimer = null;
            RaiseEvent(new RoutedEventArgs(HideEvent));
            //Mouse.Capture(null);
        }
    }
}
