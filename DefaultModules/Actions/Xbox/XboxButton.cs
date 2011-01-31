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

            Setup();
        }

        protected void Setup() {
           

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
            
            bool wasEqual = false;

            BackgroundWorker worker = sender as BackgroundWorker;

            while (true) {

                if (worker.CancellationPending == true)
                    return;

                var state2 = GamePad.GetState(player).Buttons;
                bool isEqual = StateEquals(state2);
                if (wasEqual == false && isEqual == true) {
                    // It doesn't matter what we return
                    worker.ReportProgress(0);
                }

                wasEqual = isEqual;

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
                Console.WriteLine(String.Format("{0} What player's controller? 1-4.", TAG));
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
           

            Console.WriteLine(String.Format("{0} Push buttons on the controller and press enter to set.", TAG));
            Console.ReadLine();
            buttons = GamePad.GetState(player).Buttons;
            Console.WriteLine(String.Format("{0} Saved state {1}", TAG, buttons.ToString()));
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
            buttons = new GamePadButtons((Buttons)info.GetValue("Buttons", typeof(Buttons)));
            player = (PlayerIndex)info.GetInt32("Player");

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //info.AddValue("GamePadState", configState);
            //info.AddValue("GamePadButtons", buttons);
            info.AddValue("Player", (int)player);

            // We need to create a buttons struct from all the buttons that are pressed
            Buttons keys = new Buttons();
            if ((int)this.buttons.A == 1)
                keys = keys | Buttons.A;
            if ((int)this.buttons.B == 1)
                keys = keys | Buttons.B;
            if ((int)this.buttons.Back == 1)
                keys = keys | Buttons.Back;
            if ((int)this.buttons.BigButton == 1)
                keys = keys | Buttons.BigButton;
            if ((int)this.buttons.LeftShoulder == 1)
                keys = keys | Buttons.LeftShoulder;
            if ((int)this.buttons.LeftStick == 1)
                keys = keys | Buttons.LeftStick;
            if ((int)this.buttons.RightShoulder == 1)
                keys = keys | Buttons.RightShoulder;
            if ((int)this.buttons.RightStick == 1)
                keys = keys | Buttons.RightStick;
            if ((int)this.buttons.Start == 1)
                keys = keys | Buttons.Start;
            if ((int)this.buttons.X == 1)
                keys = keys | Buttons.X;
            if ((int)this.buttons.Y == 1)
                keys = keys | Buttons.Y;

            info.AddValue("Buttons", keys);
        }
        #endregion

    }
}
