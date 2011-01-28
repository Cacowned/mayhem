using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.ComponentModel;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Diagnostics;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Actions.Xbox
{
    public class XboxButton : ActionBase, ICli
    {
        protected BackgroundWorker bw = new BackgroundWorker();

        protected const string TAG = "[Xbox]";

        protected GamePadState configState;

        public XboxButton()
            : base("Xbox Controller: Button", "Triggers when buttons on an Xbox 360 controller are pushed")
        {
            hasConfig = true;

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(WatchButtons);
            bw.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
        }

        public override void Enable() {
            base.Enable();
            bw.RunWorkerAsync();
        }

        public override void Disable() {
            base.Disable();
            bw.CancelAsync();
        }

        protected void WatchButtons(object sender, DoWorkEventArgs e) {
            //var state = GamePad.GetState(PlayerIndex.One);
            
            bool wasEquals = false;

            BackgroundWorker worker = sender as BackgroundWorker;

            while (true) {

                if (worker.CancellationPending == true)
                    return;

                var state2 = GamePad.GetState(PlayerIndex.One);

                bool isEquals = StateEquals(state2);
                if (wasEquals == false && isEquals == true) {
                    // It doesn't matter what we return
                    worker.ReportProgress(0);
                }

                wasEquals = isEquals;

                Thread.Sleep(50);
            }
        }

        protected void ProgressChanged(object sender, ProgressChangedEventArgs e) {
            base.OnActionActivated();
        }


        public void CliConfig() {
            PlayerIndex player;

            int playerNum;

            string input = "";
            do {
                Console.WriteLine("{0} What player's controller? 1-4.");
                input = Console.ReadLine();
            }
            while (!Int32.TryParse(input, out playerNum) || !(playerNum >= 1 && playerNum <= 4));

            // TODO: Need to make sure that the player is plugged in.
            switch (playerNum) {
                case 2: player = PlayerIndex.Two;
                    break;
                case 3: player = PlayerIndex.Three;
                    break;
                case 4: player = PlayerIndex.Four;
                    break;
                default: player = PlayerIndex.One;
                    break;
            }
           

            Console.WriteLine("{0} Push buttons on the controller and press enter to set.", TAG);
            Console.ReadLine();
            configState = GamePad.GetState(player);
        }

        /// <summary>
        /// Checks the gamepad state against the one saved from configuration
        /// </summary>
        /// <param name="checkState"></param>
        public bool StateEquals(GamePadState checkState) {
            return configState.Buttons.Equals(checkState.Buttons) && configState.DPad.Equals(checkState.DPad);
        }
    }
}
