using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XboxModules
{
    public enum ControllerStatus
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

        protected BackgroundWorker Worker;

        #region Singleton
        private int refCount = 0;

        private static ButtonEvents instance;
        public static ButtonEvents Instance
        {
            get
            {
                if (instance == null)
                    instance = new ButtonEvents();

                return instance;
            }
        }

        ButtonEvents()
        {
        }

        public void Dispose()
        {
            Worker.CancelAsync();
            Worker.Dispose();
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
            Worker = new BackgroundWorker();

            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += new DoWorkEventHandler(WatchButtons);

            Worker.RunWorkerAsync();
        }

        private void StopWatching()
        {
            Worker.CancelAsync();
        }

        /// <summary>
        /// This method checks for input on a gamepad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatchButtons(object sender, DoWorkEventArgs e)
        {
            GamePadButtons prevState = new GamePadButtons();

            BackgroundWorker worker = sender as BackgroundWorker;

            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    return;
                }

                var controller = GamePad.GetState(PlayerIndex.One);
                GamePadButtons currentState = controller.Buttons;

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
                    if (prevState.A == ButtonState.Released && currentState.A == ButtonState.Pressed)
                        ButtonDown(Buttons.A);
                    if (prevState.B == ButtonState.Released && currentState.B == ButtonState.Pressed)
                        ButtonDown(Buttons.B);
                    if (prevState.Back == ButtonState.Released && currentState.Back == ButtonState.Pressed)
                        ButtonDown(Buttons.Back);
                    if (prevState.BigButton == ButtonState.Released && currentState.BigButton == ButtonState.Pressed)
                        ButtonDown(Buttons.BigButton);
                    if (prevState.LeftShoulder == ButtonState.Released && currentState.LeftShoulder == ButtonState.Pressed)
                        ButtonDown(Buttons.LeftShoulder);
                    if (prevState.LeftStick == ButtonState.Released && currentState.LeftStick == ButtonState.Pressed)
                        ButtonDown(Buttons.LeftStick);
                    if (prevState.RightShoulder == ButtonState.Released && currentState.RightShoulder == ButtonState.Pressed)
                        ButtonDown(Buttons.RightShoulder);
                    if (prevState.RightStick == ButtonState.Released && currentState.RightStick == ButtonState.Pressed)
                        ButtonDown(Buttons.RightStick);
                    if (prevState.Start == ButtonState.Released && currentState.Start == ButtonState.Pressed)
                        ButtonDown(Buttons.Start);
                    if (prevState.X == ButtonState.Released && currentState.X == ButtonState.Pressed)
                        ButtonDown(Buttons.X);
                    if (prevState.Y == ButtonState.Released && currentState.Y == ButtonState.Pressed)
                        ButtonDown(Buttons.Y);

                    // check for new key ups (was pressed, now released)
                    if (prevState.A == ButtonState.Pressed && currentState.A == ButtonState.Released)
                        ButtonUp(Buttons.A);
                    if (prevState.B == ButtonState.Pressed && currentState.B == ButtonState.Released)
                        ButtonUp(Buttons.B);
                    if (prevState.Back == ButtonState.Pressed && currentState.Back == ButtonState.Released)
                        ButtonUp(Buttons.Back);
                    if (prevState.BigButton == ButtonState.Pressed && currentState.BigButton == ButtonState.Released)
                        ButtonUp(Buttons.BigButton);
                    if (prevState.LeftShoulder == ButtonState.Pressed && currentState.LeftShoulder == ButtonState.Released)
                        ButtonUp(Buttons.LeftShoulder);
                    if (prevState.LeftStick == ButtonState.Pressed && currentState.LeftStick == ButtonState.Released)
                        ButtonUp(Buttons.LeftStick);
                    if (prevState.RightShoulder == ButtonState.Pressed && currentState.RightShoulder == ButtonState.Released)
                        ButtonUp(Buttons.RightShoulder);
                    if (prevState.RightStick == ButtonState.Pressed && currentState.RightStick == ButtonState.Released)
                        ButtonUp(Buttons.RightStick);
                    if (prevState.Start == ButtonState.Pressed && currentState.Start == ButtonState.Released)
                        ButtonUp(Buttons.Start);
                    if (prevState.X == ButtonState.Pressed && currentState.X == ButtonState.Released)
                        ButtonUp(Buttons.X);
                    if (prevState.Y == ButtonState.Pressed && currentState.Y == ButtonState.Released)
                        ButtonUp(Buttons.Y);

                    //DownButtons = current_state.AsButtons();
                    prevState = currentState;
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
