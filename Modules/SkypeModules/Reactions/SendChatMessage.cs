using System;
using System.Globalization;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SkypeModules.Resources;
using SkypeModules.Wpf;

namespace SkypeModules.Reactions
{
    /// <summary>
    /// A class that will send a predefined message to the selected user identified by it's Skype ID.
    /// </summary>
    [DataContract]
    [MayhemModule("Skype: Send Chat Message", "Sends a chat message to a predefined contact")]
    public class SendChatMessage : SkypeIDReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string message;

        public override void Perform()
        {
            try
            {
                if (VerifySkypeIDValidity(skypeID))
                {
                    skype.SendMessage(skypeID, message);

                    ErrorLog.AddError(ErrorType.Message, Strings.Successful_ChatMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_SendingChatMessage, skypeID));
                Logger.Write(ex);
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new SkypeMessageConfig(skypeID, message, Strings.SendMessage_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as SkypeMessageConfig;

            skypeID = config.SkypeID;
            message = config.Message;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.SkypeMessage_ConfigString, skypeID, message);
        }

        #endregion
    }
}
