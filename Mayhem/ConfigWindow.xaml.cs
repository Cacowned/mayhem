using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace Mayhem
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private const double AnimationTime = 0.2;

        private IWpfConfigurable iWpf;
        private WpfConfiguration iWpfConfig;
        
        private Size previousSize;

        public ConfigWindow(IWpfConfigurable iWpf)
        {
            this.Owner = Application.Current.MainWindow;
            this.iWpf = iWpf;
            this.iWpfConfig = iWpf.ConfigurationControl;
            InitializeComponent();
            ConfigContent.Content = iWpfConfig;

            buttonSave.IsEnabled = iWpfConfig.CanSave;

            windowHeader.Text = iWpfConfig.Title;
            iWpfConfig.CanSavedChanged += iWpfConfig_CanSavedChanged;

            iWpfConfig.Loaded += new RoutedEventHandler(iWpfConfig_Loaded);

            // In constructor subscribe to the Change event of the WindowRect DependencyProperty
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(WindowRectProperty, typeof(ConfigWindow));
            if (dpd != null)
            {
                dpd.AddValueChanged(this, delegate
                {
                    ResizeWindow(WindowRect);
                });
            }
        }

        private void iWpfConfig_Loaded(object sender, RoutedEventArgs e)
        {
            this.previousSize = new Size(ActualWidth, ActualHeight);
            iWpfConfig.SizeChanged += new SizeChangedEventHandler(ConfigContent_SizeChanged);
        }

        private void ConfigContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WindowRect = new Rect(Left, Top, ActualWidth, ActualHeight);
            double targetWidth = iWpfConfig.Width + 40;

            Rect target = new Rect(
                Left - ((ActualWidth - previousSize.Width) / 2),
                Top - ((ActualHeight - previousSize.Height) / 2),
                ActualWidth,
                ActualHeight);

            previousSize = new Size(ActualWidth, ActualHeight);
            StartStoryBoard(WindowRect, target, AnimationTime);
        }

        private void iWpfConfig_CanSavedChanged(bool canSave)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                buttonSave.IsEnabled = canSave;
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                iWpfConfig.OnLoad();
            }
            catch
            {
                MessageBox.Show("Error loading " + iWpfConfig.Name, "Mayhem: Error", MessageBoxButton.OK);
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            WpfConfiguration config = ConfigContent.Content as WpfConfiguration;
            try
            {
                iWpfConfig.OnSave();
                iWpf.OnSaved(config);
                ((ModuleBase)iWpf).SetConfigString();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error saving " + iWpfConfig.Name);
            }

            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    iWpfConfig.OnClosing();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error closing " + iWpfConfig.Name + "'s configuration");
                }
            });
            ((MainWindow)Application.Current.MainWindow).Save();
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    iWpfConfig.OnCancel();
                    iWpfConfig.OnClosing();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error cancelling " + iWpfConfig.Name + "'s configuration");
                }
            });
            DialogResult = false;
        }

        #region Window Resizing
        public Rect WindowRect
        {
            get
            {
                return (Rect)GetValue(WindowRectProperty);
            }

            set
            {
                SetValue(WindowRectProperty, value);
            }
        }

        public static readonly DependencyProperty WindowRectProperty =
            DependencyProperty.Register("WindowRect", typeof(Rect), typeof(ConfigWindow), new UIPropertyMetadata(new Rect(0, 0, 0, 0)));

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// Resizes the window to the desired Rect
        /// Called when WindowRect DependencyProperty changes
        /// </summary>
        /// <param name="value">The target Rect containing size and Position</param>
        private void ResizeWindow(Rect value)
        {
            IntPtr windowPtr = new WindowInteropHelper(this).Handle;
            MoveWindow(windowPtr, (int)value.Left, (int)value.Top, (int)value.Width, (int)value.Height, true);
        }

        private void StartStoryBoard(Rect currentRect, Rect targetRect, double time)
        {
            RectAnimation rectAnimation = new RectAnimation();
            rectAnimation.Duration = TimeSpan.FromSeconds(time);
            rectAnimation.FillBehavior = FillBehavior.HoldEnd;

            rectAnimation.To = targetRect;

            Storyboard.SetTarget(rectAnimation, this);
            Storyboard.SetTargetProperty(rectAnimation, new PropertyPath(WindowRectProperty));

            Storyboard storyBoard = new Storyboard();
            storyBoard.Children.Add(rectAnimation);

            storyBoard.Begin(this);
        }

        #endregion
    }
}
