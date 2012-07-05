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
    /// A reaction that makes a video call to a predefined user.
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Make Video Call", "Makes a video call to a predefined user")]
    public class LyncMakeVideoCall : ReactionBase, IWpfConfigurable
    {
        /// <summary>
        /// The User ID of the predefined contact.
        /// </summary>
        [DataMember]
        private string userId;

        private LyncClient lyncClient = null;
        private Self self = null;

        /// <summary>
        /// This method will get the instance of the Lync Client application and will make a video call to the predefined userId.
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
                participants.Add(userId);

                automation.BeginStartConversation(AutomationModalities.Video, participants, null, null, automation);
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
            get { return new LyncSelectUserConfig(userId, Strings.LyncMakeVideoCall_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            LyncSelectUserConfig config = configurationControl as LyncSelectUserConfig;

            userId = config.UserId;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Lync_SelectUserConfigString, userId);
        }

        #endregion
    }
}
