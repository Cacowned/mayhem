using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;
using OfficeModules.Resources;
using OfficeModules.Wpf;

namespace OfficeModules.Reactions.Lync
{
    /// <summary>
    /// A reaction that sends a message to a predefined user
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Send Instant Message", "Send an instant message to a predefined user")]
    public class LyncSendMessage : ReactionBase, IWpfConfigurable
    {
        /// <summary>
        /// The User ID of the predefined contact.
        /// </summary>
        [DataMember]
        private string userId;

        /// <summary>
        /// The message that will be sent.
        /// </summary>
        [DataMember]
        private string message;

        private LyncClient lyncClient = null;
        private Self self = null;

        /// <summary>
        /// This method will get the instance of the Lync Client application and will send the setted message to the predefined user.
        /// </summary>
        public override void Perform()
        {
            try
            {
                lyncClient = LyncClient.GetClient();

            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Lync_ApplicationNotFound);
                Logger.Write(ex);

                return;
            }

            try
            {
                self = lyncClient.Self;

                if (self == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Lync_NotLoggedIn);
                    return;
                }

                try
                {
                    Contact contact = self.Contact.ContactManager.GetContactByUri(userId);
                }
                catch (Exception ex)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Lync_NoUserId);
                    Logger.Write(ex);
                    return;
                }

                Automation automation = LyncClient.GetAutomation();

                var participants = new List<string>();
                var contextData = new Dictionary<AutomationModalitySettings, object>();

                participants.Add(userId);
                contextData.Add(AutomationModalitySettings.FirstInstantMessage, message);
                contextData.Add(AutomationModalitySettings.SendFirstInstantMessageImmediately, true);

                automation.BeginStartConversation(AutomationModalities.InstantMessage, participants, contextData, null, automation);
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Lync_ReactionCouldntPerform);
                Logger.Write(ex);
            }
            finally
            {
                lyncClient = null;
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new LyncSendMessageConfig(userId, message); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            LyncSendMessageConfig config = configurationControl as LyncSendMessageConfig;

            userId = config.UserId;
            message = config.Message;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Lync_SendMessageConfigString, userId, message);
        }

        #endregion
    }
}
