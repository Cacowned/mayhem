using System.Diagnostics;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Data;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Mayhem<ICli> mayhem;

        ActionBase action;
        ReactionBase reaction;


        public MainWindow() {
            InitializeComponent();

            mayhem = new Mayhem<ICli>();

            if (File.Exists(Base64Serialize<ConnectionList>.filename)) {

                try {
                    mayhem.ConnectionList = Base64Serialize<ConnectionList>.Deserialize();

                    Debug.WriteLine("Starting up with " + mayhem.ConnectionList.Count + " connections");

                } catch (SerializationException e) {
                    Debug.WriteLine("(De-)SerializationException " + e);
                }
            } else {
                Connection c = new Connection(mayhem.ActionList[0], mayhem.ReactionList[0]);
                mayhem.ConnectionList.Add(c);

                c = new Connection(mayhem.ActionList[1], mayhem.ReactionList[1]);
                mayhem.ConnectionList.Add(c);

            }


            RunList.ItemsSource = mayhem.ConnectionList;
        }

        private void ActionListClick(object sender, RoutedEventArgs e)
        {
            DimMainWindow(true);

            ModuleList dlg = new ModuleList(mayhem.ActionList, "Action List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.ModulesList.SelectedItem != null) {
                    action = (ActionBase)dlg.ModulesList.SelectedItem;
                    CheckEnableBuild();
                }
            }
        }

        private void ReactionListClick(object sender, RoutedEventArgs e)
        {
            DimMainWindow(true);

            ModuleList dlg = new ModuleList(mayhem.ReactionList, "Reaction List");
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ModulesList.SelectedIndex = 0;

            dlg.ShowDialog();
            DimMainWindow(false);

            if (dlg.DialogResult == true)
            {
                if (dlg.ModulesList.SelectedItem != null) {
                    reaction = (ReactionBase)dlg.ModulesList.SelectedItem;
                    CheckEnableBuild();
                }
            }
        }

        private void CheckEnableBuild() {
            if (action != null && reaction != null) {
                mayhem.ConnectionList.Add(new Connection(action, reaction));

                action = null;
                reaction = null;
            }
        }

        private void DeleteConnectionClick(object sender, RoutedEventArgs e) {
            Connection c = ((Button)sender).Tag as Connection;
            mayhem.ConnectionList.Remove(c);
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
            Base64Serialize<ConnectionList>.SerializeObject(mayhem.ConnectionList);
        }

        protected void DimMainWindow(bool dim) {
            WindowCollection wc = Application.Current.Windows;
            Debug.WriteLine("Number of current Windows: " + wc.Count);

            MainWindow mainW = null;

            foreach (Window w in wc) {
                Debug.WriteLine("Name? " + w.Name);

                if (w.Name == "MayhemMainWindow") {
                    mainW = w as MainWindow;
                }
            }

            if (mainW != null) {
                if (dim) {
                    Panel.SetZIndex(mainW.DimRectangle, 99);
                    mainW.DimRectangle.Visibility = Visibility.Visible;
                } else {
                    Panel.SetZIndex(mainW.DimRectangle, 0);
                    mainW.DimRectangle.Visibility = Visibility.Collapsed;
                }
            }

        }
    }
}
