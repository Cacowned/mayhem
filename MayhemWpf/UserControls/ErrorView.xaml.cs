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
    class SingleAnalysisStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is ErrorType))
                throw new NotImplementedException("SingleAnalysisStatusConverter can only convert from SingleAnalysisStatus");

            String path = null;
            switch ((ErrorType)value)
            {
                case ErrorType.Failure:
                    path = "Images/error.png";
                    break;
                case ErrorType.Message:
                    path = "Images/message.png";
                    break;
                case ErrorType.Warning:
                    path = "Images/warning.png";
                    break;
                default:
                    throw new NotSupportedException();
            }
            return new BitmapImage(new Uri("/MayhemWPF;component/" + path, UriKind.Relative));
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// Interaction logic for ErrorView.xaml
    /// </summary>
    public partial class ErrorView : UserControl
    {
        public ObservableCollection<MayhemError> Errors
        {
            get { return (ObservableCollection<MayhemError>)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Errors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(ObservableCollection<MayhemError>), typeof(ErrorView), new UIPropertyMetadata(new ObservableCollection<MayhemError>()));

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

        public static readonly RoutedEvent NotifyEvent = EventManager.RegisterRoutedEvent(
            "Notify", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ErrorView));

        public event RoutedEventHandler Notify
        {
            add { AddHandler(NotifyEvent, value); }
            remove { RemoveHandler(NotifyEvent, value); }
        }

        bool isShowing = false;

        public ErrorView()
        {
            InitializeComponent();

            Errors = ErrorLog.Errors;
            Errors.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Errors_CollectionChanged);
        }

        void Errors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!isShowing)
            {
                RaiseEvent(new RoutedEventArgs(NotifyEvent));
                StartCloseTimer();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Logger.WriteLine("Border: " + e.GetPosition(this));
            isShowing = true;
            RaiseEvent(new RoutedEventArgs(ShowEvent));
            //Mouse.Capture(this);
            //e.Handled = true;
        }

        private void MayhemErrorView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ShowEvent));
            //if (e.GetPosition(this).Y < borderNumber.Height)
            //{
            //    Logger.WriteLine("Self: " + e.GetPosition(this));
            //    RaiseEvent(new RoutedEventArgs(HideEvent));
            //    Mouse.Capture(null);
            //}
        }

        void StartCloseTimer()
        {
            if (leaveTimer != null)
                leaveTimer.Stop();
            leaveTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 2) };
            leaveTimer.Tick += new EventHandler(leaveTimer_Tick);
            leaveTimer.Start();
        }

        private void MayhemErrorView_MouseLeave(object sender, MouseEventArgs e)
        {
            StartCloseTimer();
        }

        void leaveTimer_Tick(object sender, EventArgs e)
        {
            isShowing = false;
            leaveTimer.Stop();
            leaveTimer = null;
            RaiseEvent(new RoutedEventArgs(HideEvent));
            //Mouse.Capture(null);
        }

        private void MayhemErrorView_MouseMove(object sender, MouseEventArgs e)
        {
            if (leaveTimer != null)
                leaveTimer.Stop();
        }
    }
}
