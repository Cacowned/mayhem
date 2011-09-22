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
using System.Threading.Tasks;
using System.Deployment.Application;
using System.Threading;

namespace Mayhem
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

        AutoResetEvent waitForSave = new AutoResetEvent(false);

        public ObservableCollection<MayhemError> Errors
        {
            get { return (ObservableCollection<MayhemError>)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Errors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(ObservableCollection<MayhemError>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<MayhemError>()));

        private string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
        MayhemEntry mayhem;

        public MainWindow()
        {
            Application.Current.Exit += new ExitEventHandler(Application_Exit);
            if (!UriParser.IsKnownScheme("pack"))
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);

            ResourceDictionary dict = new ResourceDictionary();
            Uri uri = new Uri("/MayhemWpf;component/Styles.xaml", UriKind.Relative);
            dict.Source = uri;
            Application.Current.Resources.MergedDictionaries.Add(dict);

            mayhem = MayhemEntry.Instance;
            mayhem.SetConfigurationType(typeof(IWpfConfigurable));

            string directory = "";
            // if we are running as a clickonce application
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                directory = ApplicationDeployment.CurrentDeployment.DataDirectory;
            }
            else
            {
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");
            }

            // Scan for modules in the module directory
            bool foundModules = mayhem.EventList.ScanModules(directory);
            foundModules &= mayhem.ReactionList.ScanModules(directory);
            if (!foundModules)
            {
                ///TODO: Do something user-friendly here
                throw new Exception("No modules!");
            }

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
                        allTypes.AddRange(mayhem.EventList.GetAllTypesInModules());
                        allTypes.AddRange(mayhem.ReactionList.GetAllTypesInModules());
                        mayhem.LoadConnections(ConnectionList.Deserialize(stream, allTypes));

                        Logger.WriteLine("Starting up with " + mayhem.ConnectionList.Count + " connections");

                    }
                    catch (SerializationException e)
                    {
                        Logger.WriteLine("(De-)SerializationException " + e);
                    }
                }
            }

            RunList.ItemsSource = mayhem.ConnectionList;

            Errors = ErrorLog.Errors;
        }

        private void Save_()
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                MayhemEntry.Instance.ConnectionList.Serialize(stream);
            }
            waitForSave.Set();
        }

        public void Save()
        {
            Parallel.Invoke(Save_);
        }

        void Application_Exit(object sender, ExitEventArgs e)
        {
            Save();
            waitForSave.WaitOne();
            mayhem.Shutdown();
            foreach (Connection connection in mayhem.ConnectionList)
            {
                connection.Disable(null);
            }
        }

        private void EventListClick(object sender, RoutedEventArgs e)
        {
            DimMainWindow(true);

            ModuleList dlg = new ModuleList(MayhemEntry.Instance.EventList, "Event List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.SelectedModule != null)
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

            ModuleList dlg = new ModuleList(MayhemEntry.Instance.ReactionList, "Reaction List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.SelectedModule != null)
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
                MayhemEntry.Instance.ConnectionList.Add(new Connection(ev, reaction));

                buttonEmptyReaction.Style = (Style)FindResource("EmptyReactionButton");
                buttonEmptyEvent.Style = (Style)FindResource("EmptyEventButton");
                buttonEmptyReaction.Content = "Create Reaction";
                buttonEmptyEvent.Content = "Create Event";


                _event = null;
                _reaction = null;

                Save();
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
