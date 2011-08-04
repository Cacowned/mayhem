using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public EventBase Event
        {
            get { return (EventBase)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Event.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof(EventBase), typeof(MainWindow), new UIPropertyMetadata(null));

        public ReactionBase Reaction
        {
            get { return (ReactionBase)GetValue(ReactionProperty); }
            set { SetValue(ReactionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Reaction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReactionProperty =
            DependencyProperty.Register("Reaction", typeof(ReactionBase), typeof(MainWindow), new UIPropertyMetadata(null));


        public ObservableCollection<Error> Errors
        {
            get { return (ObservableCollection<Error>)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Errors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(ObservableCollection<Error>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<Error>()));

        private string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");

        public MainWindow()
        {
            Mayhem<IWpf> mayhem = MayhemInstance.Instance;

            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");

            // Scan for modules in the module directory
            mayhem.EventList.ScanModules(directory);
            mayhem.ReactionList.ScanModules(directory);

            InitializeComponent();
        }

        public void Load()
        {
            Mayhem<IWpf> mayhem = MayhemInstance.Instance;
            if (File.Exists(filename))
            {
                using (FileStream stream = new FileStream(filename, FileMode.Open))
                {

                    try
                    {
                        // Empty the connection list (should be empty already)
                        mayhem.ConnectionList.Clear();
                        // Load all the serialized connections
                        List<Type> allTypes = new List<Type>();
                        allTypes.AddRange(mayhem.EventList.types);
                        allTypes.AddRange(mayhem.ReactionList.types);
                        mayhem.LoadConnections(ConnectionList.Deserialize(stream, allTypes));

                        Debug.WriteLine("Starting up with " + mayhem.ConnectionList.Count + " connections");

                    }
                    catch (SerializationException e)
                    {
                        Debug.WriteLine("(De-)SerializationException " + e);
                    }
                }
            }

            RunList.ItemsSource = mayhem.ConnectionList;

            Errors = ErrorLog.Errors;
        }

        private void AppClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                MayhemInstance.Instance.ConnectionList.Serialize(stream);
            }
        }

        private void EventListClick(object sender, RoutedEventArgs e)
        {
            DimMainWindow(true);

            ModuleList dlg = new ModuleList(MayhemInstance.Instance.EventList, "Event List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.ModulesList.SelectedItem != null)
                {
                    Event = (EventBase)dlg.ModulesList.SelectedItem;

                    buttonEmptyEvent.Style = (Style)FindResource("EventButton");
                    buttonEmptyEvent.Content = Event.Name;

                    // Take this item, remove it and add it to the front (MoveToFrontList)
//                    Mayhem.EventList.Remove(Event);
//                    Mayhem.EventList.Insert(0, Event);

                    CheckEnableBuild();
                }
            }
        }

        private void ReactionListClick(object sender, RoutedEventArgs e)
        {
            DimMainWindow(true);

            ModuleList dlg = new ModuleList(MayhemInstance.Instance.ReactionList, "Reaction List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.ModulesList.SelectedItem != null)
                {
                    Reaction = (ReactionBase)dlg.ModulesList.SelectedItem;

                    buttonEmptyReaction.Style = (Style)FindResource("ReactionButton");
                    buttonEmptyReaction.Content = Reaction.Name;

                    // Take this item, remove it and add it to the front (MoveToFrontList)
//                    Mayhem.ReactionList.Remove(Reaction);
//                    Mayhem.ReactionList.Insert(0, Reaction);

                    CheckEnableBuild();
                }
            }
        }

        private void CheckEnableBuild()
        {
            if (Event != null && Reaction != null)
            {

                // We have to clone the action and reaction
                Type t = Event.GetType();
                EventBase action = (EventBase)Activator.CreateInstance(t);

                t = Reaction.GetType();
                ReactionBase reaction = (ReactionBase)Activator.CreateInstance(t);

                MayhemInstance.Instance.ConnectionList.Add(new Connection(action, reaction));

                buttonEmptyReaction.Style = (Style)FindResource("EmptyReactionButton");
                buttonEmptyEvent.Style = (Style)FindResource("EmptyEventButton");
                buttonEmptyReaction.Content = "Create Reaction";
                buttonEmptyEvent.Content = "Create Event";


                Event = null;
                Reaction = null;
            }
        }

        public static void DimMainWindow(bool dim)
        {
            WindowCollection wc = Application.Current.Windows;

            MainWindow mainW = null;

            foreach (Window w in wc)
            {
                if (w.Name == "MayhemMainWindow")
                {
                    mainW = w as MainWindow;
                }
            }

            if (mainW != null)
            {
                if (dim)
                {
                    Panel.SetZIndex(mainW.DimRectangle, 99);
                    var storyB = (Storyboard)mainW.DimRectangle.FindResource("FadeIn");
                    storyB.Begin();
                }
                else
                {
                    var storyB = (Storyboard)mainW.DimRectangle.FindResource("FadeOut");

                    storyB.Completed += delegate(object sender, EventArgs e)
                    {
                        Panel.SetZIndex(mainW.DimRectangle, 0);
                    };

                    storyB.Begin();

                }
            }

        }
    }
}
