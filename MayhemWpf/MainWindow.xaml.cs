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

        public Mayhem<IWpf> Mayhem
        {
            get { return (Mayhem<IWpf>)GetValue(MayhemProperty); }
            set { SetValue(MayhemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mayhem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MayhemProperty =
            DependencyProperty.Register("Mayhem", typeof(Mayhem<IWpf>), typeof(MainWindow), new UIPropertyMetadata(null));



        public ActionBase Action
        {
            get { return (ActionBase)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Action.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(ActionBase), typeof(MainWindow), new UIPropertyMetadata(null));



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
            Mayhem = new Mayhem<IWpf>();

            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");

            // Scan for modules in the module directory
            Mayhem.ActionList.ScanModules(directory);
            Mayhem.ReactionList.ScanModules(directory);

            InitializeComponent();
        }

        public void Load()
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                if (File.Exists(filename))
                {
                    try
                    {
                        // Empty the connection list (should be empty already)
                        Mayhem.ConnectionList.Clear();
                        // Load all the serialized connections
                        List<Type> allTypes = new List<Type>();
                        allTypes.AddRange(Mayhem.ActionList.types);
                        allTypes.AddRange(Mayhem.ReactionList.types);
                        Mayhem.LoadConnections(ConnectionList.Deserialize(stream, allTypes));

                        Debug.WriteLine("Starting up with " + Mayhem.ConnectionList.Count + " connections");

                    }
                    catch (SerializationException e)
                    {
                        Debug.WriteLine("(De-)SerializationException " + e);
                    }
                }
            }

            RunList.ItemsSource = Mayhem.ConnectionList;

            Errors = ErrorLog.Errors;
        }

        private void AppClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                Mayhem.ConnectionList.Serialize(stream);
            }
        }

        private void ActionListClick(object sender, RoutedEventArgs e)
        {
            DimMainWindow(true);

            ModuleList dlg = new ModuleList(Mayhem.ActionList, "Action List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.ModulesList.SelectedItem != null)
                {
                    Action = (ActionBase)dlg.ModulesList.SelectedItem;

                    buttonEmptyTrigger.Style = (Style)FindResource("TriggerButton");
                    buttonEmptyTrigger.Content = Action.Name;

                    // Take this item, remove it and add it to the front (MoveToFrontList)
//                    Mayhem.ActionList.Remove(Action);
//                    Mayhem.ActionList.Insert(0, Action);

                    CheckEnableBuild();
                }
            }
        }

        private void ReactionListClick(object sender, RoutedEventArgs e)
        {
            DimMainWindow(true);

            ModuleList dlg = new ModuleList(Mayhem.ReactionList, "Reaction List");
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

                    buttonEmptyAction.Style = (Style)FindResource("ReactionButton");
                    buttonEmptyAction.Content = Reaction.Name;

                    // Take this item, remove it and add it to the front (MoveToFrontList)
//                    Mayhem.ReactionList.Remove(Reaction);
//                    Mayhem.ReactionList.Insert(0, Reaction);

                    CheckEnableBuild();
                }
            }
        }

        private void CheckEnableBuild()
        {
            if (Action != null && Reaction != null)
            {

                // We have to clone the action and reaction
                Type t = Action.GetType();
                ActionBase action = (ActionBase)Activator.CreateInstance(t);

                t = Reaction.GetType();
                ReactionBase reaction = (ReactionBase)Activator.CreateInstance(t);

                Mayhem.ConnectionList.Add(new Connection(action, reaction));

                buttonEmptyTrigger.Style = (Style)FindResource("EmptyTriggerButton");
                buttonEmptyAction.Style = (Style)FindResource("EmptyActionButton");
                buttonEmptyTrigger.Content = "Create Trigger";
                buttonEmptyAction.Content = "Create Action";


                Action = null;
                Reaction = null;
            }
        }

        private void DeleteConnectionClick(object sender, RoutedEventArgs e)
        {
            Connection c = ((Button)sender).Tag as Connection;
            c.Disable();
            Mayhem.ConnectionList.Remove(c);
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
