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

namespace DefaultModules.Actions.Xbox
{
    public class AButton : ActionBase
    {
        protected BackgroundWorker bw = new BackgroundWorker();

        public AButton()
            : base("Xbox: A Button", "Triggers when the A button on an Xbox 360 controller is pushed")
        {
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
            var state = GamePad.GetState(PlayerIndex.One);
            
            BackgroundWorker worker = sender as BackgroundWorker;

            while (true) {

                if (worker.CancellationPending == true)
                    return;

                var state2 = GamePad.GetState(PlayerIndex.One);
                if (state2.IsConnected && state2.IsButtonDown(Buttons.A) &&
                                          state.IsButtonUp(Buttons.A)) {
                    // It doesn't matter what we return
                    worker.ReportProgress(0);
                }

                state = state2;
                Thread.Sleep(100);
            }
        }

        protected void ProgressChanged(object sender, ProgressChangedEventArgs e) {
            Debug.WriteLine("A Pressed");
        }

    }
}
