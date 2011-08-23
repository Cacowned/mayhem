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
using System.Reflection;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ModuleType _event;
        private EventBase _eventInstance = null;
        private ModuleType _reaction;
        private ReactionBase _reactionInstance = null;

        public ObservableCollection<Error> Errors
        {
            get { return (ObservableCollection<Error>)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Errors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(ObservableCollection<Error>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<Error>()));

        private string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
        Mayhem mayhem;

        public MainWindow()
        {
            mayhem = Mayhem.Instance;
            mayhem.SetConfigurationType(typeof(IWpfConfigurable));

            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");

            // Scan for modules in the module directory
            mayhem.EventList.ScanModules(directory);
            mayhem.ReactionList.ScanModules(directory);

            InitializeComponent();
        }

        public void Load()
        {
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
                        allTypes.AddRange(mayhem.EventList.ToTypeArray());
                        allTypes.AddRange(mayhem.ReactionList.ToTypeArray());
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
                Mayhem.Instance.ConnectionList.Serialize(stream);
            }
        }

        private void EventListClick(object sender, RoutedEventArgs e)
        {
            DimMainWindow(true);

            ModuleList dlg = new ModuleList(Mayhem.Instance.EventList, "Event List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.ModulesList.SelectedItem != null)
                {
                    _event = dlg.SelectedModule;
                    _eventInstance = dlg.SelectedModuleInstance as EventBase;

                    buttonEmptyEvent.Style = (Style)FindResource("EventButton");
                    buttonEmptyEvent.Content = _event.Name;

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

            ModuleList dlg = new ModuleList(Mayhem.Instance.ReactionList, "Reaction List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.ModulesList.SelectedItem != null)
                {
                    _reaction = dlg.SelectedModule;
                    _reactionInstance = dlg.SelectedModuleInstance as ReactionBase;

                    buttonEmptyReaction.Style = (Style)FindResource("ReactionButton");
                    buttonEmptyReaction.Content = _reaction.Name;

                    // Take this item, remove it and add it to the front (MoveToFrontList)
//                    Mayhem.ReactionList.Remove(Reaction);
//                    Mayhem.ReactionList.Insert(0, Reaction);

                    CheckEnableBuild();
                }
            }
        }

        private void CheckEnableBuild()
        {
            if (_event != null && _reaction != null)
            {
                EventBase ev;
                ReactionBase reaction;
                if (_eventInstance != null)
                    ev = _eventInstance;
                else
                {
                    Type t = _event.Type;
                    ev = (EventBase)Activator.CreateInstance(t);
                }

                if (_reactionInstance != null)
                    reaction = _reactionInstance;
                else
                {
                    Type t = _reaction.Type;
                    reaction = (ReactionBase)Activator.CreateInstance(t);
                }
                Mayhem.Instance.ConnectionList.Add(new Connection(ev, reaction));

                buttonEmptyReaction.Style = (Style)FindResource("EmptyReactionButton");
                buttonEmptyEvent.Style = (Style)FindResource("EmptyEventButton");
                buttonEmptyReaction.Content = "Create Reaction";
                buttonEmptyEvent.Content = "Create Event";


                _event = null;
                _reaction = null;
            }
        }

        public static void DimMainWindow(bool dim)
        {
            MainWindow mainW = Application.Current.MainWindow as MainWindow;

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
