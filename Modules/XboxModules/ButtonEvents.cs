using System;
using System.ComponentModel;
using System.Threading;
using MayhemCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XboxModules.Resources;

namespace XboxModules
{
    enum ControllerStatus
    {
        Attached,
        Detached
    }

    class ButtonEvents : IDisposable
    {
        public delegate void ButtonDownHandler(Buttons button);
        public event ButtonDownHandler OnButtonDown;

        public delegate void ButtonUpHandler(Buttons button);
        public event ButtonUpHandler OnButtonUp;

        public delegate void ControllerStatusChanged(ControllerStatus status);
        public event ControllerStatusChanged OnStatusChanged;

        protected BackgroundWorker bw;

        #region Singleton
        private int refCount = 0;

        private static ButtonEvents _instance;

        public static ButtonEvents Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ButtonEvents();

                return _instance;
            }
        }

        ButtonEvents()
        {
        }

        public void Dispose()
        {
            bw.CancelAsync();
            bw.Dispose();
        }

        public void AddRef()
        {
            refCount++;
            StartWatching();
        }

        public void RemoveRef()
        {
            refCount--;
            if (refCount == 0)
            {
                StopWatching();
            }
        }

        #endregion

        private ControllerStatus status;
        public ControllerStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                // Only do this if the flag is being switched
                if (value != status)
                {
                    status = value;

                    if (OnStatusChanged != null)
                    {
                        OnStatusChanged(status);
                    }
                }
            }
        }

        private void StartWatching()
        {
            bw = new BackgroundWorker();

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(WatchButtons);

            bw.RunWorkerAsync();
        }

        private void StopWatching()
        {
            bw.CancelAsync();
        }

        /// <summary>
        /// This method checks for input on a gamepad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatchButtons(object sender, DoWorkEventArgs e)
        {
            GamePadButtons prev_state = new GamePadButtons();

            BackgroundWorker worker = sender as BackgroundWorker;

            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    return;
                }

                var controller = GamePad.GetState(PlayerIndex.One);
                GamePadButtons current_state = controller.Buttons;

                Status = controller.IsConnected ? ControllerStatus.Attached : ControllerStatus.Detached;

                if (!controller.IsConnected)
                {
                    // It's not plugged in, we might as well wait a full second
                    // before checking again
                    Thread.Sleep(1000);
                }
                else
                {
                    // check for new key downs (was released, now pressed)
                    if (prev_state.A == ButtonState.Released && current_state.A == ButtonState.Pressed)
                        ButtonDown(Buttons.A);
                    if (prev_state.B == ButtonState.Released && current_state.B == ButtonState.Pressed)
                        ButtonDown(Buttons.B);
                    if (prev_state.Back == ButtonState.Released && current_state.Back == ButtonState.Pressed)
                        ButtonDown(Buttons.Back);
                    if (prev_state.BigButton == ButtonState.Released && current_state.BigButton == ButtonState.Pressed)
                        ButtonDown(Buttons.BigButton);
                    if (prev_state.LeftShoulder == ButtonState.Released && current_state.LeftShoulder == ButtonState.Pressed)
                        ButtonDown(Buttons.LeftShoulder);
                    if (prev_state.LeftStick == ButtonState.Released && current_state.LeftStick == ButtonState.Pressed)
                        ButtonDown(Buttons.LeftStick);
                    if (prev_state.RightShoulder == ButtonState.Released && current_state.RightShoulder == ButtonState.Pressed)
                        ButtonDown(Buttons.RightShoulder);
                    if (prev_state.RightStick == ButtonState.Released && current_state.RightStick == ButtonState.Pressed)
                        ButtonDown(Buttons.RightStick);
                    if (prev_state.Start == ButtonState.Released && current_state.Start == ButtonState.Pressed)
                        ButtonDown(Buttons.Start);
                    if (prev_state.X == ButtonState.Released && current_state.X == ButtonState.Pressed)
                        ButtonDown(Buttons.X);
                    if (prev_state.Y == ButtonState.Released && current_state.Y == ButtonState.Pressed)
                        ButtonDown(Buttons.Y);

                    // check for new key ups (was pressed, now released)
                    if (prev_state.A == ButtonState.Pressed && current_state.A == ButtonState.Released)
                        ButtonUp(Buttons.A);
                    if (prev_state.B == ButtonState.Pressed && current_state.B == ButtonState.Released)
                        ButtonUp(Buttons.B);
                    if (prev_state.Back == ButtonState.Pressed && current_state.Back == ButtonState.Released)
                        ButtonUp(Buttons.Back);
                    if (prev_state.BigButton == ButtonState.Pressed && current_state.BigButton == ButtonState.Released)
                        ButtonUp(Buttons.BigButton);
                    if (prev_state.LeftShoulder == ButtonState.Pressed && current_state.LeftShoulder == ButtonState.Released)
                        ButtonUp(Buttons.LeftShoulder);
                    if (prev_state.LeftStick == ButtonState.Pressed && current_state.LeftStick == ButtonState.Released)
                        ButtonUp(Buttons.LeftStick);
                    if (prev_state.RightShoulder == ButtonState.Pressed && current_state.RightShoulder == ButtonState.Released)
                        ButtonUp(Buttons.RightShoulder);
                    if (prev_state.RightStick == ButtonState.Pressed && current_state.RightStick == ButtonState.Released)
                        ButtonUp(Buttons.RightStick);
                    if (prev_state.Start == ButtonState.Pressed && current_state.Start == ButtonState.Released)
                        ButtonUp(Buttons.Start);
                    if (prev_state.X == ButtonState.Pressed && current_state.X == ButtonState.Released)
                        ButtonUp(Buttons.X);
                    if (prev_state.Y == ButtonState.Pressed && current_state.Y == ButtonState.Released)
                        ButtonUp(Buttons.Y);

                    //DownButtons = current_state.AsButtons();
                    prev_state = current_state;
                }
                Thread.Sleep(50);
            }


        }

        private void ButtonDown(Buttons button)
        {
            if (OnButtonDown != null)
            {
                OnButtonDown(button);
            }
        }

        private void ButtonUp(Buttons button)
        {
            if (OnButtonUp != null)
            {
                OnButtonUp(button);
            }
        }
    }
}
