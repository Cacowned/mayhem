using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.ComponentModel;

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
                ImageSettingsEvent.Visibility = System.Windows.Visibility.Hidden;
                textBlockEventName.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                textBlockEventName.Margin = new Thickness(5, -2, 14, 2);
                buttonTrigger.Cursor = null;
            }
            if (!Connection.Reaction.HasConfig)
            {
                ImageSettingsReaction.Visibility = System.Windows.Visibility.Hidden;
                textBlockReactionName.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                textBlockReactionName.Margin = new Thickness(7, -2, 14, 2);
                buttonReaction.Cursor = null;
            }

            connectionButtons.Opacity = Connection.IsEnabled ? 1 : 0.5;
            dropShadow.BlurRadius = Connection.IsEnabled ? 16 : 4;
            dropShadow.Opacity = Connection.IsEnabled ? 0.9 : 0.5;
            dropShadow.ShadowDepth = Connection.IsEnabled ? 5 : 2;
        }

        void ShowConfig(ModuleBase configurable)
        {
            if (!configurable.HasConfig)
                return;

            MainWindow.DimMainWindow(true);

            Connection.IsConfiguring = true;

            bool wasEnabled = Connection.IsEnabled;
            if (wasEnabled)
            {
                Connection.Disable(null);
            }
            ConfigWindow config = new ConfigWindow((IWpfConfigurable)configurable);
            config.ShowDialog();

            if (wasEnabled)
            {
                Connection.Enable(new Action(() => Dispatcher.Invoke((Action)delegate { Connection.IsConfiguring = false; })));
            }
            //Connection.IsConfiguring = false;

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
            c.Disable(new Action(() =>
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
                    });
                });

            if (!Connection.IsEnabled)
            {
                Connection.Enable(action);
            }
            else
            {
                Connection.Disable(action);
            }
        }
    }
}
