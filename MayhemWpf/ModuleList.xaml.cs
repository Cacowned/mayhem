using System.Collections;
using System.Windows;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using MayhemCore;
using System.Windows.Media.Animation;
using System;
using System.Windows.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.ComponentModel;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for EventList.xaml
    /// </summary>
    public partial class ModuleList : Window
    {
        public ModuleType SelectedModule
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
        IWpfConfiguration iWpfConfig;

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ModuleList), new UIPropertyMetadata(string.Empty));

        ThicknessAnimation animSlideOut;
        ThicknessAnimation animSlideIn;
        RectAnimation animSize;

        const double AnimationTime = 0.2;

        public ModuleList(IEnumerable list, string headerText)
        {
            Text = headerText;
            InitializeComponent();

            animSlideOut = new ThicknessAnimation(new Thickness(0), new Duration(TimeSpan.FromSeconds(AnimationTime)));
            animSlideIn = new ThicknessAnimation(new Thickness(0), new Duration(TimeSpan.FromSeconds(AnimationTime)));
            animSize = new RectAnimation(new Rect(), new Duration(TimeSpan.FromSeconds(AnimationTime)));

            ModulesList.ItemsSource = list;

            // In constructor subscribe to the Change even of the WindowRect DependencyProperty
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(WindowRectProperty, typeof(ModuleList));
            if (dpd != null)
            {
                dpd.AddValueChanged(this, delegate
                {
                    ResizeWindow(WindowRect);
                });
            }
        }

        private void ChooseButtonClick(object sender, RoutedEventArgs e)
        {
            bool hasConfig = false;
            SelectedModule = (ModuleType)ModulesList.SelectedItem;
            Type[] interfaceTypes = SelectedModule.Type.GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                if (interfaceType.Equals(typeof(IWpfConfigurable)))
                {
                    SelectedModuleInstance = (IWpfConfigurable)Activator.CreateInstance(SelectedModule.Type);
                    iWpfConfig = (IWpfConfiguration)SelectedModuleInstance.ConfigurationControl;
                    ConfigContent.Content = iWpfConfig;
                    buttonCanSave.IsEnabled = iWpfConfig.CanSave;
                    windowHeaderConfig.Text = "Config: " + iWpfConfig.Title;
                    iWpfConfig.Loaded += new RoutedEventHandler(iWpfConfig_Loaded);

                    iWpfConfig.OnLoad();

                    hasConfig = true;
                    break;
                }
            }
            if (!hasConfig)
            {
                DialogResult = true;
            }
        }

        void iWpfConfig_Loaded(object sender, RoutedEventArgs e)
        {
            WindowRect = new Rect(Left, Top, ActualWidth, ActualHeight);
            double targetWidth = iWpfConfig.Width + 40;
            double targetHeight = windowHeaderConfig.ActualHeight + iWpfConfig.ActualHeight + 100;

            animSlideOut.To = new Thickness(-targetWidth, 0, targetWidth, 0);
            stackPanelList.BeginAnimation(StackPanel.MarginProperty, animSlideOut);

            animSlideIn.To = new Thickness(0);
            animSlideIn.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime * 0.95));
            stackPanelConfig.BeginAnimation(StackPanel.MarginProperty, animSlideIn);

            Rect target = new Rect(Left - (targetWidth - ActualWidth) / 2, Top - (targetHeight - ActualHeight) / 2,
                                   targetWidth, targetHeight);

            StartStoryBoard(WindowRect, target);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (iWpfConfig.OnSave())
            {
                SelectedModuleInstance.OnSaved(ConfigContent.Content as IWpfConfiguration);
            }
            iWpfConfig.OnClosing();
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            iWpfConfig.OnCancel();
            iWpfConfig.OnClosing();
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

        #region Resize Animation
        private void StartStoryBoard(Rect currentRect, Rect targetRect)
        {
            // Set up animation duration and behavior
            RectAnimation rectAnimation = new RectAnimation();
            rectAnimation.Duration = TimeSpan.FromSeconds(AnimationTime);
            rectAnimation.FillBehavior = FillBehavior.HoldEnd;

            // Set the From and To properties of the animation.
            rectAnimation.From = currentRect;
            rectAnimation.To = targetRect;

            // Set the Target of the animation to the Window
            // Remember to define a name in XAML
            Storyboard.SetTarget(rectAnimation, this);
            Storyboard.SetTargetProperty(rectAnimation, new PropertyPath(WindowRectProperty));

            // Create a storyboard to apply the animation.
            Storyboard storyBoard = new Storyboard();
            storyBoard.Children.Add(rectAnimation);

            storyBoard.Begin(this);
        }
        #endregion
        #endregion
    }
}
