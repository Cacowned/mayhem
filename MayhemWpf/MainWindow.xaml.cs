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
    }
}
