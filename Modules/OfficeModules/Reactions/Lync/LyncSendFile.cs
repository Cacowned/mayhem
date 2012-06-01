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
    /// This reaction sends the selected file to a predefined user
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Send File", "Sends a file to a predefined user")]
    public class LyncSendFile : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string userId;

        [DataMember]
        private string fileName;

        private LyncClient lyncClient = null;
        private Self self = null;

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

                contextData.Add(AutomationModalitySettings.FilePathToTransfer, fileName);
                contextData.Add(AutomationModalitySettings.FileIsShared, true);

                participants.Add(userId);

                automation.BeginStartConversation(AutomationModalities.FileTransfer, participants, contextData, null, automation);
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
            get { return new LyncSendFileConfig(userId, fileName); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            LyncSendFileConfig config = configurationControl as LyncSendFileConfig;

            userId = config.UserId;
            fileName = config.FileName;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Lync_SendFileConfigString, userId, fileName);
        }

        #endregion
    }
}
