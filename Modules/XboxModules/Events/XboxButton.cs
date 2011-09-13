using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XboxModules.Wpf;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace XboxModules.Events
{
    [DataContract]
    [MayhemModule("Xbox Controller: Button", "Triggers when buttons on an Xbox 360 controller are pushed")]
    public class XboxButton : EventBase, IWpfConfigurable
    {
        private ButtonWatcher buttonWatcher;

        public static bool IsConfigOpen = false;

        #region Configuration
        [DataMember]
        private Buttons XboxButtons
        {
            get;
            set;
        }

        [DataMember]
        private PlayerIndex Player
        {
            get;
            set;
        }


        #endregion

        protected const string TAG = "[Xbox]";

        protected override void Initialize()
        {
            base.Initialize();

            buttonWatcher = ButtonWatcher.Instance;
        }

        public override void SetConfigString()
        {
            ConfigString = XboxButtons.ButtonString();
        }

        public override void Enable()
        {
            var state = GamePad.GetState(Player);
            if (!state.IsConnected)
            {
                ErrorLog.AddError(ErrorType.Failure, "Xbox Controller is not plugged in. Not Enabling.");
            }
            else
            {
                // If it is connected, lets enable
                base.Enable();
                buttonWatcher.AddCombinationHandler(XboxButtons, OnKeyCombinationActivated);
            }
        }

        public override void Disable()
        {
            base.Disable();

            buttonWatcher.RemoveCombinationHandler(XboxButtons, OnKeyCombinationActivated);
        }

        void OnKeyCombinationActivated()
        {
            if (!IsConfigOpen)
            {
                if (Enabled)
                {
                    OnEventActivated();
                }
            }
        }

        
        public void OnSaved(IWpfConfiguration configurationControl)
        {

            buttonWatcher.RemoveCombinationHandler(XboxButtons, OnKeyCombinationActivated);
            XboxButtons = ((XboxButtonConfig)configurationControl).SaveButtons;
            buttonWatcher.AddCombinationHandler(XboxButtons, OnKeyCombinationActivated);
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new XboxButtonConfig(XboxButtons); }
        }

        /// <summary>
        /// Checks the gamepad state against the one saved from configuration
        /// </summary>
        /// <param name="checkState"></param>
        public bool StateEquals(GamePadButtons checkState)
        {
            return XboxButtons.Equals(checkState);
        }
    }
}
