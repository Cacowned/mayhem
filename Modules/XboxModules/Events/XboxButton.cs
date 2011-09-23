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

        protected override void Initialize()
        {
            base.Initialize();

            Player = PlayerIndex.One;

            buttonWatcher = ButtonWatcher.Instance;
        }

        public override void SetConfigString()
        {
            ConfigString = XboxButtons.ButtonString();
        }

        #region Configuration Views
        public void OnSaved(IWpfConfiguration configurationControl)
        {
            buttonWatcher.RemoveCombinationHandler(XboxButtons, OnKeyCombinationActivated);
            XboxButtons = ((XboxButtonConfig)configurationControl).ButtonsToSave;
            buttonWatcher.AddCombinationHandler(XboxButtons, OnKeyCombinationActivated);
        }

        public IWpfConfiguration ConfigurationControl
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
            base.Disable();

            buttonWatcher.RemoveCombinationHandler(XboxButtons, OnKeyCombinationActivated);
        }
    }
}
