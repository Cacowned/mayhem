using System.Windows;
using System.Windows.Controls;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System;

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

        public ModuleView()
        {
            InitializeComponent();

            animOut = new DoubleAnimation();
            animOut.To = 0.5;
            animOut.Duration = new Duration(TimeSpan.FromSeconds(0.25));

            animIn = new DoubleAnimation();
            animIn.To = 1.0;
            animIn.Duration = new Duration(TimeSpan.FromSeconds(0.25));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Connection.Action.HasConfig)
                buttonTrigger.Cursor = null;
            if (!Connection.Reaction.HasConfig)
                buttonReaction.Cursor = null;

            connectionButtons.Opacity = Connection.Enabled ? 1 : 0.5;
        }

        private void ConfigureTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (!Connection.Action.HasConfig)
                return;

            MainWindow.DimMainWindow(true);

            bool wasEnabled = Connection.Enabled;

            Connection.Disable();
            ((IWpf)Connection.Action).WpfConfig();

            if (wasEnabled)
            {
                Connection.Enable();
            }

            MainWindow.DimMainWindow(false);
        }

        private void ConfigureReaction_Click(object sender, RoutedEventArgs e)
        {
            if (!Connection.Reaction.HasConfig)
                return;

            MainWindow.DimMainWindow(true);

            bool wasEnabled = Connection.Enabled;

            Connection.Disable();
            ((IWpf)Connection.Reaction).WpfConfig();

            if (wasEnabled)
            {
                Connection.Enable();
            }

            MainWindow.DimMainWindow(false);
        }

        private void DeleteConnectionClick(object sender, RoutedEventArgs e)
        {
            Connection c = ((Button)sender).Tag as Connection;
            c.Disable();
            MainWindow.Mayhem.ConnectionList.Remove(c);
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
                connectionButtons.BeginAnimation(StackPanel.OpacityProperty, animIn);
            else
                connectionButtons.BeginAnimation(StackPanel.OpacityProperty, animOut);
            //connectionButtons.Opacity = Connection.Enabled ? 1 : 0.5;
        }
    }
}
