using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
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
        private Buttons xboxButtons;

        [DataMember]
        private PlayerIndex player;
       
        #endregion

        public XboxButton()
        {
            player = PlayerIndex.One;
        }

        protected override void Initialize()
        {
            buttonWatcher = ButtonWatcher.Instance;
        }

        public string GetConfigString()
        {
            return xboxButtons.ButtonString();
        }

        #region Configuration Views
        public void OnSaved(WpfConfiguration configurationControl)
        {
            buttonWatcher.RemoveCombinationHandler(xboxButtons, OnKeyCombinationActivated);
            xboxButtons = ((XboxButtonConfig)configurationControl).ButtonsToSave;
            buttonWatcher.AddCombinationHandler(xboxButtons, OnKeyCombinationActivated);
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new XboxButtonConfig(xboxButtons); }
        }
        #endregion

        private void OnKeyCombinationActivated()
        {
            if (!IsConfigOpen)
            {
                if (IsEnabled)
                {
                    Trigger();
                }
            }
        }

        protected override bool OnEnable()
        {
            var state = GamePad.GetState(player);
            if (!state.IsConnected)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.XboxButton_CantEnable);
            }
            else
            {
                // If it is connected, lets enable
                buttonWatcher.AddCombinationHandler(xboxButtons, OnKeyCombinationActivated);
            }

            return state.IsConnected;
        }

        protected override void OnDisable()
        {
            buttonWatcher.RemoveCombinationHandler(xboxButtons, OnKeyCombinationActivated);
        }
    }
}
