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
using System.Runtime.Serialization;

namespace DefaultModules.Actions.Xbox
{
    [Serializable]
    public class XboxButton : ActionBase, ICli, ISerializable
    {
        protected BackgroundWorker bw = new BackgroundWorker();

        protected const string TAG = "[Xbox]";

        protected GamePadButtons buttons;
        //protected GamePadState configState;
        protected PlayerIndex player = PlayerIndex.One;

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

                var state2 = GamePad.GetState(player).Buttons;

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
            Debug.WriteLine(String.Format("{0} Button Pressed", TAG));
            base.OnActionActivated();
        }


        public void CliConfig() {
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
            //configState = GamePad.GetState(player);
            buttons = GamePad.GetState(player).Buttons;
        }

        /// <summary>
        /// Checks the gamepad state against the one saved from configuration
        /// </summary>
        /// <param name="checkState"></param>
        public bool StateEquals(GamePadButtons checkState) {
            return buttons.Equals(checkState);
            //return configState.Buttons.Equals(checkState.Buttons) && configState.DPad.Equals(checkState.DPad);
        }

        #region Serialization
        // The xna stuff isn't serializable. Find a fix?
        public XboxButton(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {

            //configState = (GamePadState)info.GetValue("GamePadState", typeof(GamePadState));
            //buttons = (GamePadButtons)info.GetValue("GamePadButtons", typeof(GamePadButtons));
            //player = (PlayerIndex)info.GetValue("Player", typeof(PlayerIndex));
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            //info.AddValue("GamePadState", configState);
            //info.AddValue("GamePadButtons", buttons);
            //info.AddValue("Player", player);
        }
        #endregion

    }
}
