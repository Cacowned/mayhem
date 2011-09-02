﻿using System.Windows;
using System.Windows.Controls;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System;
using System.Windows.Media.Effects;

namespace MayhemWpf.UserControls
{
    /// <summary>
    /// Interaction logic for ModuleView.xaml
    /// </summary>
    public partial class ModuleView : UserControl
    {
        public Connection Connection
        {
            get { return (Connection)GetValue(ConnectionProperty); }
            set { SetValue(ConnectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Module.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionProperty =
            DependencyProperty.Register("Connection", typeof(Connection), typeof(ModuleView), new UIPropertyMetadata(null));

        DoubleAnimation animOut;
        DoubleAnimation animIn;
        DoubleAnimation animBlurOut;
        DoubleAnimation animBlurIn;
        DoubleAnimation animBlurOpacityOut;
        DoubleAnimation animBlurOpacityIn;
        DoubleAnimation animBlurDistanceOut;
        DoubleAnimation animBlurDistanceIn;

        public ModuleView()
        {
            InitializeComponent();

            animOut = new DoubleAnimation(0.5, new Duration(TimeSpan.FromSeconds(0.25)));
            animIn = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(0.25)));
            animBlurOut = new DoubleAnimation(4, new Duration(TimeSpan.FromSeconds(0.25)));
            animBlurIn = new DoubleAnimation(16, new Duration(TimeSpan.FromSeconds(0.25)));
            animBlurOpacityOut = new DoubleAnimation(0.5, new Duration(TimeSpan.FromSeconds(0.25)));
            animBlurOpacityIn = new DoubleAnimation(0.9, new Duration(TimeSpan.FromSeconds(0.25)));
            animBlurDistanceOut = new DoubleAnimation(2, new Duration(TimeSpan.FromSeconds(0.25)));
            animBlurDistanceIn = new DoubleAnimation(5, new Duration(TimeSpan.FromSeconds(0.25)));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Connection.Event.HasConfig)
            {
                ImageSettingsEvent.Visibility = System.Windows.Visibility.Hidden;
                buttonTrigger.Cursor = null;
            }
            if (!Connection.Reaction.HasConfig)
            {
                ImageSettingsReaction.Visibility = System.Windows.Visibility.Hidden;
                buttonReaction.Cursor = null;
            }

            connectionButtons.Opacity = Connection.Enabled ? 1 : 0.5;
            dropShadow.BlurRadius = Connection.Enabled ? 16 : 4;
            dropShadow.Opacity = Connection.Enabled ? 0.9 : 0.5;
            dropShadow.ShadowDepth = Connection.Enabled ? 5 : 2;
        }

        private void ConfigureTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (!Connection.Event.HasConfig)
                return;

            MainWindow.DimMainWindow(true);

            Connection.IsConfiguring = true;

            bool wasEnabled = Connection.Enabled;
            if (wasEnabled)
            {
                Connection.Disable();
            }
            ConfigWindow config = new ConfigWindow((IWpfConfigurable)Connection.Event);
            config.ShowDialog();

            if (wasEnabled)
            {
                Connection.Enable();
            }
            Connection.IsConfiguring = false;

            MainWindow.DimMainWindow(false);
        }

        private void ConfigureReaction_Click(object sender, RoutedEventArgs e)
        {
            if (!Connection.Reaction.HasConfig)
                return;

            MainWindow.DimMainWindow(true);

            Connection.IsConfiguring = true;

            bool wasEnabled = Connection.Enabled;
            if (wasEnabled)
            {
                Connection.Disable();
            }
            ConfigWindow config = new ConfigWindow((IWpfConfigurable)Connection.Reaction);
            config.ShowDialog();

            if (wasEnabled)
            {
                Connection.Enable();
            }
            Connection.IsConfiguring = false;

            MainWindow.DimMainWindow(false);
        }

        private void DeleteConnectionClick(object sender, RoutedEventArgs e)
        {
            Connection c = ((Button)sender).Tag as Connection;
            c.Disable();
            c.Delete();

            Mayhem.Instance.ConnectionList.Remove(c);
            ((MainWindow)Application.Current.MainWindow).Save();
        }

        private void OnOffClick(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;

            if (!Connection.Enabled)
            {
                Connection.Enable();

                if (!Connection.Enabled)
                {
                    //Debug.WriteLine("Connection didn't enable.");

                    // We wanted to enable it, and it didn't enable
                    // mark the event as handled so it doesn't
                    // flip the button
                    button.IsChecked = false;
                    e.Handled = true;
                }

            }
            else
            {
                Connection.Disable();

                if (Connection.Enabled)
                {
                    //Debug.WriteLine("Connection didn't disable.");
                    // We wanted to disable it, and it didn't disable
                    // mark the event as handled so it doesn't
                    // flip the button
                    button.IsChecked = true;
                    e.Handled = true;
                }
                else
                {
                    //Debug.WriteLine("Connection disabled");
                }
            }
            if (Connection.Enabled)
            {
                connectionButtons.BeginAnimation(StackPanel.OpacityProperty, animIn);
                dropShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, animBlurIn);
                dropShadow.BeginAnimation(DropShadowEffect.OpacityProperty, animBlurOpacityIn);
                dropShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, animBlurDistanceIn);
            }
            else
            {
                connectionButtons.BeginAnimation(StackPanel.OpacityProperty, animOut);
                dropShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, animBlurOut);
                dropShadow.BeginAnimation(DropShadowEffect.OpacityProperty, animBlurOpacityOut);
                dropShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, animBlurDistanceOut);
            }
            ((MainWindow)Application.Current.MainWindow).Save();
        }
    }
}
