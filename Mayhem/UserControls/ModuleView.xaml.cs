using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.ComponentModel;
using System.Windows.Shapes;

namespace Mayhem.UserControls
{
    /// <summary>
    /// Interaction logic for ModuleView.xaml
    /// </summary>
    public partial class ModuleView : UserControl
    {
        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }
        public string ReactionName
        {
            get { return (string)GetValue(ReactionNameProperty); }
            set { SetValue(ReactionNameProperty, value); }
        }
        public string EventConfigString
        {
            get { return (string)GetValue(EventConfigStringProperty); }
            set { SetValue(EventConfigStringProperty, value); }
        }
        public string ReactionConfigString
        {
            get { return (string)GetValue(ReactionConfigStringProperty); }
            set { SetValue(ReactionConfigStringProperty, value); }
        }
        public Connection Connection
        {
            get
            {
                return (Connection)GetValue(ConnectionProperty);
            }
            set
            {
                SetValue(ConnectionProperty, value);
            }
        }

        public static readonly DependencyProperty ConnectionProperty =
            DependencyProperty.Register("Connection", typeof(Connection), typeof(ModuleView), new UIPropertyMetadata(null));
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(ModuleView), new UIPropertyMetadata(""));
        public static readonly DependencyProperty ReactionNameProperty =
            DependencyProperty.Register("ReactionName", typeof(string), typeof(ModuleView), new UIPropertyMetadata(""));
        public static readonly DependencyProperty EventConfigStringProperty =
            DependencyProperty.Register("EventConfigString", typeof(string), typeof(ModuleView), new UIPropertyMetadata(""));
        public static readonly DependencyProperty ReactionConfigStringProperty =
            DependencyProperty.Register("ReactionConfigString", typeof(string), typeof(ModuleView), new UIPropertyMetadata(""));

        DoubleAnimation animOut;
        DoubleAnimation animIn;

        public ModuleView()
        {
            InitializeComponent();

            animOut = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.25)));
            animIn = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(0.25)));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Connection connection = Connection;
            EventName = connection.Event.Name;
            ReactionName = connection.Reaction.Name;
            EventConfigString = connection.Event.ConfigString;
            ReactionConfigString = connection.Reaction.ConfigString;
            connection.Event.PropertyChanged += delegate(object s, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "ConfigString")
                    EventConfigString = Connection.Event.ConfigString;
            };
            connection.Reaction.PropertyChanged += delegate(object s, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "ConfigString")
                    ReactionConfigString = Connection.Reaction.ConfigString;
            };
            if (!Connection.Event.HasConfig)
            {
                ImageSettingsEventOff.Visibility = Visibility.Hidden;
                ImageSettingsEventRed.Visibility = Visibility.Hidden;
                textBlockEventName.VerticalAlignment = VerticalAlignment.Center;
                textBlockEventNameDisabled.VerticalAlignment = VerticalAlignment.Center;
                textBlockEventName.Margin = new Thickness(5, -4, 14, 4);
                textBlockEventNameDisabled.Margin = new Thickness(5, -4, 14, 4);
                buttonTrigger.Cursor = null;
            }
            if (!Connection.Reaction.HasConfig)
            {
                ImageSettingsReactionOff.Visibility = Visibility.Hidden;
                ImageSettingsReactionBlue.Visibility = Visibility.Hidden;
                textBlockReactionName.VerticalAlignment = VerticalAlignment.Center;
                textBlockReactionNameDisabled.VerticalAlignment = VerticalAlignment.Center;
                textBlockReactionName.Margin = new Thickness(7, -4, 14, 4);
                textBlockReactionNameDisabled.Margin = new Thickness(7, -4, 14, 4);
                buttonReaction.Cursor = null;
            }

            redButtonImage.Opacity = Connection.IsEnabled ? 1 : 0;
            blueButtonImage.Opacity = Connection.IsEnabled ? 1 : 0;
            ImageSettingsEventRed.Opacity = Connection.IsEnabled ? 1 : 0;
            ImageSettingsReactionBlue.Opacity = Connection.IsEnabled ? 1 : 0;
            textBlockEventName.Opacity = Connection.IsEnabled ? 1 : 0;
            textBlockReactionName.Opacity = Connection.IsEnabled ? 1 : 0;
        }

        private void ShowConfig(ModuleBase configurable)
        {
            if (!configurable.HasConfig)
                return;

            MainWindow.DimMainWindow(true);
            Connection connection = Connection;

            DisabledEventArgs args = new DisabledEventArgs(true);

            bool wasEnabled = connection.IsEnabled;
            if (wasEnabled)
            {
                connection.Disable(args, null);
            }
            ConfigWindow config = new ConfigWindow((IWpfConfigurable)configurable);
            config.ShowDialog();

            if (wasEnabled)
            {
                Connection.Enable(new EnablingEventArgs(true), null);
            }

            MainWindow.DimMainWindow(false);
        }

        private void ConfigureTrigger_Click(object sender, RoutedEventArgs e)
        {
            ShowConfig(Connection.Event);
        }

        private void ConfigureReaction_Click(object sender, RoutedEventArgs e)
        {
            ShowConfig(Connection.Reaction);
        }

        private void DeleteConnectionClick(object sender, RoutedEventArgs e)
        {
            Connection c = ((Button)sender).Tag as Connection;
            c.Disable(new DisabledEventArgs(false), new Action(() =>
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        c.Delete();

                        MayhemEntry.Instance.ConnectionList.Remove(c);
                        ((MainWindow)Application.Current.MainWindow).Save();
                    });
                }));
        }

        private void OnOffClick(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;

            Action action = new Action(() =>
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        if (!Connection.IsEnabled)
                        {
                            //Logger.WriteLine("Connection didn't enable.");

                            // We wanted to enable it, and it didn't enable
                            // mark the event as handled so it doesn't
                            // flip the button
                            button.IsChecked = false;
                        }
                        button.IsChecked = Connection.IsEnabled;
                        if (Connection.IsEnabled)
                        {
                            redButtonImage.BeginAnimation(Rectangle.OpacityProperty, animIn);
                            blueButtonImage.BeginAnimation(Rectangle.OpacityProperty, animIn);
                            ImageSettingsEventRed.BeginAnimation(Rectangle.OpacityProperty, animIn);
                            ImageSettingsReactionBlue.BeginAnimation(Rectangle.OpacityProperty, animIn);
                            textBlockEventName.BeginAnimation(Rectangle.OpacityProperty, animIn);
                            textBlockReactionName.BeginAnimation(Rectangle.OpacityProperty, animIn);
                        }
                        else
                        {
                            redButtonImage.BeginAnimation(Rectangle.OpacityProperty, animOut);
                            blueButtonImage.BeginAnimation(Rectangle.OpacityProperty, animOut);
                            ImageSettingsEventRed.BeginAnimation(Rectangle.OpacityProperty, animOut);
                            ImageSettingsReactionBlue.BeginAnimation(Rectangle.OpacityProperty, animOut);
                            textBlockEventName.BeginAnimation(Rectangle.OpacityProperty, animOut);
                            textBlockReactionName.BeginAnimation(Rectangle.OpacityProperty, animOut);
                        }
                        ((MainWindow)Application.Current.MainWindow).Save();
                    });
                });

            if (!Connection.IsEnabled)
            {
                Connection.Enable(new EnablingEventArgs(false), action);
            }
            else
            {
                Connection.Disable(new DisabledEventArgs(false), action);
            }
        }
    }
}
