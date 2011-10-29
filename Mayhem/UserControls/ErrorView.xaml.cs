using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MayhemCore;

namespace Mayhem.UserControls
{
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

        private DispatcherTimer leaveTimer = null;

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

        internal bool IsShowing 
        {
            get;
            private set; 
        }

        public ErrorView()
        {
            InitializeComponent();

            Errors = ErrorLog.Errors;
            Errors.CollectionChanged += Errors_CollectionChanged;
        }

        // When clicking on the number of errors
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowOrClose();
        }

        // When clicking on the listbox
        private void Errors_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // if we aren't showing (only possible while notifying)
            if (!IsShowing)
            {
                // then open the whole thing
                StartShowing();
            }
        }

        // When a new error is added
        private void Errors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // If we aren't already showing
            if (!IsShowing)
            {
                // start showing
                RaiseEvent(new RoutedEventArgs(NotifyEvent));
                StartCloseTimer();
            }
        }

        // When the mouse leaves, start the timer to close
        private void MayhemErrorView_MouseLeave(object sender, MouseEventArgs e)
        {
            StartCloseTimer();
        }

        private void StartCloseTimer()
        {
            if (leaveTimer != null)
                leaveTimer.Stop();

            // close in two seconds
            leaveTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 2) };
            leaveTimer.Tick += leaveTimer_Tick;
            leaveTimer.Start();
        }

        private void leaveTimer_Tick(object sender, EventArgs e)
        {
            leaveTimer.Stop();
            leaveTimer = null;
            StopShowing();
        }

        // If we move the mouse inside of the control, don't close
        // the view
        private void MayhemErrorView_MouseMove(object sender, MouseEventArgs e)
        {
            if (leaveTimer != null)
                leaveTimer.Stop();
        }

        private void ShowOrClose()
        {
            if (IsShowing)
            {
                StopShowing();
            }
            else
            {
                StartShowing();
            }
        }

        private void StartShowing()
        {
            IsShowing = true;
            RaiseEvent(new RoutedEventArgs(ShowEvent));
        }

        internal void StopShowing()
        {
            IsShowing = false;
            RaiseEvent(new RoutedEventArgs(HideEvent));
        }
    }
}
