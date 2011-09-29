﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using System.Windows.Media;

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
        WpfConfiguration iWpfConfig;

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ModuleList), new UIPropertyMetadata(string.Empty));

        RectAnimation animSize;

        bool isCheckingSizeChanged = false;
        bool isFirstLoad = true;

        const double AnimationTime = 0.2;

        int heightBasedOnModules;

        public ModuleList(IEnumerable list, string headerText)
        {
            Text = headerText;
            InitializeComponent();

            animSize = new RectAnimation(new Rect(), new Duration(TimeSpan.FromSeconds(AnimationTime)));

            ModulesList.ItemsSource = list;

            heightBasedOnModules = (int)Math.Min(155 + 43 * ModulesList.Items.Count, Height);
            Height = heightBasedOnModules;

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
            //double targetWidth = iWpfConfig.Width + 40;
            //double targetHeight = windowHeaderConfig.ActualHeight + iWpfConfig.ActualHeight + 100;

            //Rect target = new Rect(Left - (targetWidth - ActualWidth) / 2, Top - (targetHeight - ActualHeight) / 2,
            //                       targetWidth, targetHeight);

            //StartStoryBoard(WindowRect, target, AnimationTime, false);
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
                        iWpfConfig.Loaded += new RoutedEventHandler(iWpfConfig_Loaded);
                        iWpfConfig.CanSavedChanged += new WpfConfiguration.ConfigCanSaveHandler(iWpfConfig_CanSavedChanged);
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
            Dispatcher.Invoke(new Action(delegate()
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

            DoubleAnimation animSlideOut = new DoubleAnimation();
            animSlideOut.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            animSlideOut.To = -300;
            animSlideOut.Completed += delegate(object s, EventArgs args)
            {
                stackPanelList.Visibility = System.Windows.Visibility.Hidden;
            };
            ((TranslateTransform)gridControls.RenderTransform).BeginAnimation(TranslateTransform.XProperty, animSlideOut);

            ((TranslateTransform)stackPanelConfig.RenderTransform).X = 280;
            stackPanelConfig.Visibility = System.Windows.Visibility.Visible;
            animSlideOut = new DoubleAnimation();
            animSlideOut.To = 300;
            animSlideOut.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            ((TranslateTransform)stackPanelConfig.RenderTransform).BeginAnimation(TranslateTransform.XProperty, animSlideOut);

            Rect target = new Rect(Left - (targetWidth - ActualWidth) / 2, Top - (targetHeight - ActualHeight) / 2,
                                   targetWidth, targetHeight);

            StartStoryBoard(WindowRect, target, AnimationTime, true);

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
            
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                try
                {
                    iWpfConfig.OnClosing();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error closing " + SelectedModule.Name + "'s configuration");
                }
            }));
            DialogResult = true;
        }

        private void buttonConfigCancel_Click(object sender, RoutedEventArgs e)
        {
            if (isCheckingSizeChanged)
            {
                isCheckingSizeChanged = false;
                ConfigContent.SizeChanged -= new SizeChangedEventHandler(ConfigContent_SizeChanged);
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
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
                }));

            DoubleAnimation animSlideOut = new DoubleAnimation();
            animSlideOut.To = 0;
            animSlideOut.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            animSlideOut.Completed += delegate(object s, EventArgs args)
            {
                stackPanelConfig.Visibility = System.Windows.Visibility.Hidden;
            };
            ((TranslateTransform)gridControls.RenderTransform).BeginAnimation(TranslateTransform.XProperty, animSlideOut);

            ((TranslateTransform)stackPanelList.RenderTransform).X = 20;
            stackPanelList.Visibility = System.Windows.Visibility.Visible;
            animSlideOut = new DoubleAnimation();
            animSlideOut.To = 0;
            animSlideOut.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            ((TranslateTransform)stackPanelList.RenderTransform).BeginAnimation(TranslateTransform.XProperty, animSlideOut);

            //animSlideOut.To = new Thickness(0);
            //stackPanelList.BeginAnimation(StackPanel.MarginProperty, animSlideOut);

            //animSlideIn.From = new Thickness(0);
            //animSlideIn.To = new Thickness(290, 0, -300, 0);
            //animSlideIn.Duration = new Duration(TimeSpan.FromSeconds(AnimationTime));
            //animSlideIn.Completed += new EventHandler(animSlideIn_Completed);
            //stackPanelConfig.BeginAnimation(StackPanel.MarginProperty, animSlideIn);

            Rect target = new Rect(Left - (300 - ActualWidth) / 2, Top - (heightBasedOnModules - ActualHeight) / 2,
                                   300, heightBasedOnModules);

            StartStoryBoard(WindowRect, target, AnimationTime, false);

            buttonChoose.IsEnabled = true;
            buttonCancel.IsEnabled = true;
            buttonSave.IsEnabled = false;
            buttonConfigCancel.IsEnabled = false;
        }

        void animSlideIn_Completed(object sender, EventArgs e)
        {
            stackPanelConfig.Margin = new Thickness(300, 0, -300, 0);
        }

        void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ChooseButtonClick(sender, e);
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
        private void StartStoryBoard(Rect currentRect, Rect targetRect, double time, bool checkOnComplete)
        {
            // Set up animation duration and behavior
            RectAnimation rectAnimation = new RectAnimation();
            rectAnimation.Duration = TimeSpan.FromSeconds(time);
            rectAnimation.FillBehavior = FillBehavior.HoldEnd;

            // Set the From and To properties of the animation.
//            rectAnimation.From = currentRect;
            rectAnimation.To = targetRect;

            // Set the Target of the animation to the Window
            // Remember to define a name in XAML
            Storyboard.SetTarget(rectAnimation, this);
            Storyboard.SetTargetProperty(rectAnimation, new PropertyPath(WindowRectProperty));

            // Create a storyboard to apply the animation.
            Storyboard storyBoard = new Storyboard();
            storyBoard.Children.Add(rectAnimation);

            if (checkOnComplete)
            {
                storyBoard.Completed += new EventHandler(storyBoard_Completed);
            }
            storyBoard.Begin(this);
        }

        void storyBoard_Completed(object sender, EventArgs e)
        {
            if (!isCheckingSizeChanged)
            {
                isCheckingSizeChanged = true;
                ConfigContent.SizeChanged += new SizeChangedEventHandler(ConfigContent_SizeChanged);
            }
        }
        #endregion

        #endregion
    }
}
