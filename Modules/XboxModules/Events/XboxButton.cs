using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XboxModules.Resources;
using XboxModules.Wpf;

namespace XboxModules.Events
{
    [DataContract]
    [MayhemModule("Xbox Controller: Button", "Triggers when buttons on an Xbox 360 controller are pushed")]
    public class XboxButton : EventBase, IWpfConfigurable
    {
        private ButtonWatcher buttonWatcher;

        public static bool IsConfigOpen { get; set; }

        #region Configuration
        [DataMember]
        private Buttons XboxButtons;

        [DataMember]
        private PlayerIndex Player;
       
        #endregion

        protected override void Initialize()
        {
            Player = PlayerIndex.One;

            buttonWatcher = ButtonWatcher.Instance;
        }

        public override void SetConfigString()
        {
            ConfigString = XboxButtons.ButtonString();
        }

        #region Configuration Views
        public void OnSaved(WpfConfiguration configurationControl)
        {
            buttonWatcher.RemoveCombinationHandler(XboxButtons, OnKeyCombinationActivated);
            XboxButtons = ((XboxButtonConfig)configurationControl).ButtonsToSave;
            buttonWatcher.AddCombinationHandler(XboxButtons, OnKeyCombinationActivated);
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new XboxButtonConfig(XboxButtons); }
        }
        #endregion

        private void OnKeyCombinationActivated()
        {
            if (!IsConfigOpen)
            {
                if (Enabled)
                {
                    Trigger();
                }
            }
        }

        public override bool Enable()
        {
            var state = GamePad.GetState(Player);
            if (!state.IsConnected)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.XboxButton_CantEnable);
            }
            else
            {
                // If it is connected, lets enable
                buttonWatcher.AddCombinationHandler(XboxButtons, OnKeyCombinationActivated);
            }

            return state.IsConnected;
        }

        public override void Disable()
        {
            buttonWatcher.RemoveCombinationHandler(XboxButtons, OnKeyCombinationActivated);
        }
    }
}
