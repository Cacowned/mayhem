using System.Diagnostics;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Mayhem<ICli> Mayhem {
            get { return (Mayhem<ICli>)GetValue(MayhemProperty); }
            set { SetValue(MayhemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mayhem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MayhemProperty =
            DependencyProperty.Register("Mayhem", typeof(Mayhem<ICli>), typeof(MainWindow), new UIPropertyMetadata(null));



        public ActionBase Action {
            get { return (ActionBase)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Action.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(ActionBase), typeof(MainWindow), new UIPropertyMetadata(null));



        public ReactionBase Reaction {
            get { return (ReactionBase)GetValue(ReactionProperty); }
            set { SetValue(ReactionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Reaction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReactionProperty =
            DependencyProperty.Register("Reaction", typeof(ReactionBase), typeof(MainWindow), new UIPropertyMetadata(null));

        

        public MainWindow() {
            

            Mayhem = new Mayhem<ICli>();

            if (File.Exists(Base64Serialize<ConnectionList>.filename)) {

                try {
                    Mayhem.ConnectionList = Base64Serialize<ConnectionList>.Deserialize();

                    Debug.WriteLine("Starting up with " + Mayhem.ConnectionList.Count + " connections");

                } catch (SerializationException e) {
                    Debug.WriteLine("(De-)SerializationException " + e);
                }
            }


            InitializeComponent();

            RunList.ItemsSource = Mayhem.ConnectionList;
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
                if (dlg.ModulesList.SelectedItem != null) {
                    Action = (ActionBase)dlg.ModulesList.SelectedItem;
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
                if (dlg.ModulesList.SelectedItem != null) {
                    Reaction = (ReactionBase)dlg.ModulesList.SelectedItem;
                    CheckEnableBuild();
                }
            }
        }

        private void CheckEnableBuild() {
            if (Action != null && Reaction != null) {
                Mayhem.ConnectionList.Add(new Connection(Action, Reaction));

                Action = null;
                Reaction = null;
            }
        }

        private void DeleteConnectionClick(object sender, RoutedEventArgs e) {
            Connection c = ((Button)sender).Tag as Connection;
            Mayhem.ConnectionList.Remove(c);
        }

        private void OnOffClick(object sender, RoutedEventArgs e) {
            ToggleButton button = (ToggleButton)sender;
            Connection c = button.Tag as Connection;

            Debug.WriteLine("On/Off clicked on " + c.Action.Name);
            
            if (!c.Enabled) {
                c.Enable();
            } else {
                c.Disable();
            }
            
            Debug.WriteLine("Connection is enabled: " + c.Enabled);
        }

        private void AppClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            Base64Serialize<ConnectionList>.SerializeObject(Mayhem.ConnectionList);
        }

        public static void DimMainWindow(bool dim) {
            WindowCollection wc = Application.Current.Windows;

            MainWindow mainW = null;

            foreach (Window w in wc) {

                if (w.Name == "MayhemMainWindow") {
                    mainW = w as MainWindow;
                }
            }

            if (mainW != null) {
                if (dim) {
                    Panel.SetZIndex(mainW.DimRectangle, 99);
                    var storyB = (Storyboard)mainW.DimRectangle.FindResource("FadeIn");
                    storyB.Begin();
                } else {
                    

                    var storyB = (Storyboard)mainW.DimRectangle.FindResource("FadeOut");

                    storyB.Completed += delegate(object sender, EventArgs e) {
                        Panel.SetZIndex(mainW.DimRectangle, 0);
                    };

                    storyB.Begin();

                }
            }

        }
    }
}
