using System.Diagnostics;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Mayhem<ICli> mayhem;

        public MainWindow() {
            InitializeComponent();

            mayhem = new Mayhem<ICli>();

            Connection c = new Connection(mayhem.ActionList[0], mayhem.ReactionList[0]);
            mayhem.ConnectionList.Add(c);

            c = new Connection(mayhem.ActionList[1], mayhem.ReactionList[1]);
            mayhem.ConnectionList.Add(c);

            RunList.ItemsSource = mayhem.ConnectionList;
        }

        private void ActionListClick(object sender, RoutedEventArgs e)
        {
            ActionList dlg = new ActionList(mayhem);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ActionsList.SelectedIndex = 0;

            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                if (dlg.ActionsList.SelectedItem != null)
                    MessageBox.Show(dlg.ActionsList.SelectedItem.GetType().ToString());
                else
                    MessageBox.Show("Nothing Selected");
            }
            else
                MessageBox.Show("Nay!");
        }

        private void ReactionListClick(object sender, RoutedEventArgs e)
        {
            ReactionList dlg = new ReactionList(mayhem);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                MessageBox.Show("Yay!");
            }
            MessageBox.Show("Nay!");
        }
    }
}
