﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MayhemCore;
using MayhemWpf.ModuleTypes;

namespace Mayhem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ModuleType eventType;
        private EventBase eventInstance = null;
        private ModuleType reactionType;
        private ReactionBase reactionInstance = null;

        private AutoResetEvent waitForSave = new AutoResetEvent(false);

        public ObservableCollection<MayhemError> Errors
        {
            get { return (ObservableCollection<MayhemError>)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Errors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(ObservableCollection<MayhemError>), typeof(MainWindow), new UIPropertyMetadata(new ObservableCollection<MayhemError>()));

        private string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
        
        private MayhemEntry mayhem;

        public MainWindow()
        {
            Application.Current.Exit += new ExitEventHandler(Application_Exit);
            if (!UriParser.IsKnownScheme("pack"))
            {
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);
            }

            ResourceDictionary dict = new ResourceDictionary();
            Uri uri = new Uri("/MayhemWpf;component/Styles.xaml", UriKind.Relative);
            dict.Source = uri;
            Application.Current.Resources.MergedDictionaries.Add(dict);

            mayhem = MayhemEntry.Instance;
            mayhem.SetConfigurationType(typeof(IWpfConfigurable));

            string directory = string.Empty;

            // if we are running as a clickonce application
            if (ApplicationDeployment.IsNetworkDeployed)
                directory = ApplicationDeployment.CurrentDeployment.DataDirectory;
            else
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");

            // Scan for modules in the module directory
            bool foundModules = mayhem.EventList.ScanModules(directory);
            foundModules &= mayhem.ReactionList.ScanModules(directory);
            if (!foundModules)
            {
                // TODO: Do something user-friendly here
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
                        ErrorLog.AddError(ErrorType.Failure, "Error loading saved data");
                        Logger.WriteLine("(De-)SerializationException " + e);
                    }
                }
            }

            RunList.ItemsSource = mayhem.ConnectionList;

            Errors = ErrorLog.Errors;
        }

        private void Save_()
        {
            try
            {
                using (FileStream stream = new FileStream(this.filename, FileMode.Create))
                {
                    MayhemEntry.Instance.ConnectionList.Serialize(stream);
                }

                this.waitForSave.Set();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error saving data");
            }
        }

        public void Save()
        {
            Parallel.Invoke(Save_);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Save();
            waitForSave.WaitOne();
            try
            {
                mayhem.Shutdown();
                foreach (Connection connection in mayhem.ConnectionList)
                {
                    connection.Disable(new DisabledEventArgs(false), null);
                }
            }
            catch
            {
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
                    this.eventType = dlg.SelectedModule;
                    this.eventInstance = dlg.SelectedModuleInstance as EventBase;

                    buttonEmptyEvent.Style = (Style)FindResource("EventButton");
                    buttonEmptyEvent.Content = this.eventType.Name;

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
                    this.reactionType = dlg.SelectedModule;
                    this.reactionInstance = dlg.SelectedModuleInstance as ReactionBase;

                    buttonEmptyReaction.Style = (Style)FindResource("ReactionButton");
                    buttonEmptyReaction.Content = this.reactionType.Name;

                    CheckEnableBuild();
                }
            }
        }

        private void CheckEnableBuild()
        {
            if (eventType != null && reactionType != null)
            {
                try
                {
                    EventBase ev;
                    if (eventInstance != null)
                    {
                        ev = eventInstance;
                    }
                    else
                    {
                        Type t = eventType.Type;
                        ev = (EventBase)Activator.CreateInstance(t);
                    }

                    ReactionBase re;
                    if (reactionInstance != null)
                    {
                        re = reactionInstance;
                    }
                    else
                    {
                        Type t = this.reactionType.Type;
                        re = (ReactionBase)Activator.CreateInstance(t);
                    }

                    MayhemEntry.Instance.ConnectionList.Add(new Connection(ev, re));
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error creating connection between " + eventType.Type.Name + " and " + this.reactionType.Type.Name);
                }

                buttonEmptyReaction.Style = (Style)FindResource("EmptyReactionButton");
                buttonEmptyEvent.Style = (Style)FindResource("EmptyEventButton");
                buttonEmptyReaction.Content = "Choose Reaction";
                buttonEmptyEvent.Content = "Choose Event";

                eventType = null;
                this.reactionType = null;

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

                    storyB.Completed += delegate
                    {
                        Panel.SetZIndex(mainW.DimRectangle, 0);
                    };

                    storyB.Begin();
                }
            }
        }
    }
}
