using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace Mayhem
{
    /// <summary>
    /// Interaction logic for EventList.xaml
    /// </summary>
    public partial class ModuleList : Window
    {
        internal ModuleType SelectedModule
        {
            get;
            private set;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public IWpfConfigurable SelectedModuleInstance
        {
            get;
            private set;
        }

        private WpfConfiguration iWpfConfig;

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ModuleList), new UIPropertyMetadata(string.Empty));

        private const double AnimationTime = 0.2;

        private int heightBasedOnModules;

        public ModuleList(IEnumerable list, string headerText)
        {
            Text = headerText;
            InitializeComponent();

            ModulesList.ItemsSource = list;

            this.heightBasedOnModules = (int)Math.Min(185 + (43 * ModulesList.Items.Count), Height);
            Height = this.heightBasedOnModules;

            // In constructor subscribe to the Change event of the WindowRect DependencyProperty
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(WindowRectProperty, typeof(ModuleList));
            if (dpd != null)
            {
                dpd.AddValueChanged(this, delegate
                {
                    ResizeWindow(WindowRect);
                });
            }
        }

        private void ConfigContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WindowRect = new Rect(Left, Top, ActualWidth, ActualHeight);
            double targetWidth = iWpfConfig.Width + 40;
            double targetHeight = windowHeaderConfig.ActualHeight + iWpfConfig.ActualHeight + 100;

            Rect target = new Rect(
                Left - ((targetWidth - ActualWidth) / 2),
                Top - ((targetHeight - ActualHeight) / 2),
                targetWidth,
                targetHeight);

            StartStoryBoard(WindowRect, target, AnimationTime);
        }

        private void ChooseButtonClick(object sender, RoutedEventArgs e)
        {
            bool hasConfig = false;
            SelectedModule = (ModuleType)ModulesList.SelectedItem;
            SelectedModuleInstance = null;
            Type[] interfaceTypes = SelectedModule.Type.GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                if (interfaceType.Equals(typeof(IWpfConfigurable)))
                {
                    try
                    {
                        SelectedModuleInstance = (IWpfConfigurable)Activator.CreateInstance(SelectedModule.Type);
                        iWpfConfig = (WpfConfiguration)SelectedModuleInstance.ConfigurationControl;
                        ConfigContent.Content = iWpfConfig;
                        buttonSave.IsEnabled = iWpfConfig.CanSave;
                        windowHeaderConfig.Text = iWpfConfig.Title;
                        iWpfConfig.Loaded += iWpfConfig_Loaded;
                        iWpfConfig.CanSavedChanged += iWpfConfig_CanSavedChanged;
                        iWpfConfig.SizeChanged += new SizeChangedEventHandler(ConfigContent_SizeChanged);
                        iWpfConfig.OnLoad();
                    }
                    catch
                    {
                        MessageBox.Show("Error creating " + SelectedModule.Name, "Mayhem: Error", MessageBoxButton.OK);
                    }

                    hasConfig = true;
                    break;
                }
            }

            if (!hasConfig)
            {
                DialogResult = true;
            }
        }

        private void iWpfConfig_CanSavedChanged(bool canSave)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                buttonSave.IsEnabled = canSave;
            }));
        }

        private void iWpfConfig_Loaded(object sender, RoutedEventArgs e)
        {
            WindowRect = new Rect(Left, Top, ActualWidth, ActualHeight);
            double targetWidth = iWpfConfig.Width + 40;
            double targetHeight = windowHeaderConfig.ActualHeight + iWpfConfig.ActualHeight + 100;
            stackPanelConfig.Width = targetWidth;

            // Animate the render transform of the grid
            DoubleAnimation animSlideOut = new DoubleAnimation();
            animSlideOut.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            animSlideOut.To = -300;
            animSlideOut.Completed += delegate
            {
                stackPanelList.Visibility = System.Windows.Visibility.Hidden;
            };
            ((TranslateTransform)gridControls.RenderTransform).BeginAnimation(TranslateTransform.XProperty, animSlideOut);

            // Animate the render transform of the config (this covers up the white space between them)
            ((TranslateTransform)stackPanelConfig.RenderTransform).X = 280;
            stackPanelConfig.Visibility = System.Windows.Visibility.Visible;
            animSlideOut = new DoubleAnimation();
            animSlideOut.To = 300;
            animSlideOut.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            ((TranslateTransform)stackPanelConfig.RenderTransform).BeginAnimation(TranslateTransform.XProperty, animSlideOut);

            // Animate the window size to match the config control
            Rect target = new Rect(
                Left - ((targetWidth - ActualWidth) / 2),
                Top - ((targetHeight - ActualHeight) / 2),
                targetWidth, 
                targetHeight);

            StartStoryBoard(WindowRect, target, AnimationTime);

            buttonChoose.IsEnabled = false;
            buttonCancel.IsEnabled = false;
            buttonSave.IsEnabled = iWpfConfig.CanSave;
            buttonConfigCancel.IsEnabled = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            WpfConfiguration config = ConfigContent.Content as WpfConfiguration;
            try
            {
                iWpfConfig.OnSave();
                SelectedModuleInstance.OnSaved(config);
                ((ModuleBase)SelectedModuleInstance).SetConfigString();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error saving " + SelectedModule.Name);
            }

            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    iWpfConfig.OnClosing();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error closing " + SelectedModule.Name + "'s configuration");
                }
            });
            DialogResult = true;
        }

        private void buttonConfigCancel_Click(object sender, RoutedEventArgs e)
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
                        ErrorLog.AddError(ErrorType.Failure, "Error cancelling " + SelectedModule.Name + "'s configuration");
                    }
                });

            // Animate the render transform of the grid
            DoubleAnimation animSlideOut = new DoubleAnimation();
            animSlideOut.To = 0;
            animSlideOut.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            animSlideOut.Completed += delegate(object s, EventArgs args)
            {
                stackPanelConfig.Visibility = System.Windows.Visibility.Hidden;
            };
            ((TranslateTransform)gridControls.RenderTransform).BeginAnimation(TranslateTransform.XProperty, animSlideOut);

            // Animate the render transform of the list (this covers up the white space between them)
            ((TranslateTransform)stackPanelList.RenderTransform).X = 20;
            stackPanelList.Visibility = System.Windows.Visibility.Visible;
            animSlideOut = new DoubleAnimation();
            animSlideOut.To = 0;
            animSlideOut.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            ((TranslateTransform)stackPanelList.RenderTransform).BeginAnimation(TranslateTransform.XProperty, animSlideOut);

            // Animate the window size to match the list control
            Rect target = new Rect(
                Left - ((300 - ActualWidth) / 2),
                Top - ((heightBasedOnModules - ActualHeight) / 2),
                300, 
                heightBasedOnModules);

            StartStoryBoard(WindowRect, target, AnimationTime);

            buttonChoose.IsEnabled = true;
            buttonCancel.IsEnabled = true;
            buttonSave.IsEnabled = false;
            buttonConfigCancel.IsEnabled = false;
        }

        private void animSlideIn_Completed(object sender, EventArgs e)
        {
            stackPanelConfig.Margin = new Thickness(300, 0, -300, 0);
        }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ChooseButtonClick(sender, e);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
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

        // Using a DependencyProperty as the backing store for WindowRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowRectProperty =
            DependencyProperty.Register("WindowRect", typeof(Rect), typeof(ModuleList), new UIPropertyMetadata(new Rect(0, 0, 0, 0)));

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
