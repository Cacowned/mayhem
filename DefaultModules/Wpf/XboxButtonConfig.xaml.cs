using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Threading;
using DefaultModules.Actions.Xbox;

namespace DefaultModules.Wpf
{
    /// <summary>
    /// Interaction logic for XboxButtonConfig.xaml
    /// </summary>
    public partial class XboxButtonConfig : Window
    {
        protected BackgroundWorker bw = new BackgroundWorker();

        // TODO: Make this configurable
        protected PlayerIndex player = PlayerIndex.One;

        public string ButtonsDown {
            get { return (string)GetValue(ButtonsDownProperty); }
            set { SetValue(ButtonsDownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonsDown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonsDownProperty =
            DependencyProperty.Register("ButtonsDown", typeof(string), typeof(XboxButtonConfig), new UIPropertyMetadata(string.Empty));

        public Buttons down_buttons;
        
        public XboxButtonConfig(GamePadButtons buttons) {
            InitializeComponent();

            down_buttons = new Buttons();

            XboxButton.MergeButtons(ref down_buttons, buttons);

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(WatchButtons);

            bw.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);

            ButtonsDown = ButtonsToString(down_buttons);

            bw.RunWorkerAsync();

        }


        protected void WatchButtons(object sender, DoWorkEventArgs e) {

            BackgroundWorker worker = sender as BackgroundWorker;

            while (true) {

                if (worker.CancellationPending == true)
                    return;

                var state2 = GamePad.GetState(player).Buttons;

                XboxButton.MergeButtons(ref down_buttons, state2);
                worker.ReportProgress(0);
                Thread.Sleep(50);
            }
        }

        protected void ProgressChanged(object sender, ProgressChangedEventArgs e) {
            // Hack: using gamepadbutton's tostring to get a good output
            ButtonsDown = ButtonsToString(down_buttons);
        }

        protected string ButtonsToString(Buttons buttons) {
            return new GamePadButtons(buttons).ToString();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e) {
            if (ButtonsToString(down_buttons) == "{Buttons:None}") {
                MessageBox.Show("You must push at least one button");
                return;
            }

            DialogResult = true;
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e) {
            down_buttons = new Buttons();
            ButtonsDown = String.Empty;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
            bw.CancelAsync();
            DialogResult = false;
        }


    }
}
