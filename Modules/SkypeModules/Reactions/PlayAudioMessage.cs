using System;
using System.Globalization;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SKYPE4COMLib;
using SkypeModules.Resources;
using SkypeModules.Wpf;

namespace SkypeModules.Reactions
{
    /// <summary>
    /// A class that will play an audio message to the user identified by the selected Skype ID when a call is in progress.
    /// </summary>
    [DataContract]
    [MayhemModule("Skype: Play Audio Message", "Plays the selected audio message to a predefined contact when a call is in progress")]
    public class PlayAudioMessage : SkypeIDReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string filePath;

        public override void Perform()
        {
            bool isCallActive = false;

            try
            {
                if (VerifySkypeIDValidity(skypeID))
                {
                    foreach (Call call in skype.ActiveCalls)
                    {
                        if (call.PartnerHandle.Equals(skypeID))
                        {
                            isCallActive = true;
                            call.set_InputDevice(TCallIoDeviceType.callIoDeviceTypeFile, filePath);
                        }
                    }

                    if (isCallActive)
                    {
                        ErrorLog.AddError(ErrorType.Message, Strings.Successful_AudioMessage);
                    }
                    else
                    {
                        ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_CallNotInProgress, skypeID));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_SendingAudioMessage, skypeID));
                Logger.Write(ex);
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new SkypeAudioMessageConfig(skypeID, filePath, Strings.SendAudioMessage_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as SkypeAudioMessageConfig;

            skypeID = config.SkypeID;
            filePath = config.FilePath;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.SkypeAudioMessage_ConfigString, skypeID, filePath);
        }

        #endregion
    }
}
