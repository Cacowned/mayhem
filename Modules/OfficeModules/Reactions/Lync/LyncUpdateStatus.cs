using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Microsoft.Lync.Model;
using OfficeModules.Resources;
using OfficeModules.Wpf;

namespace OfficeModules.Reactions.Lync
{
    /// <summary>
    /// This reaction updates the status of the current user
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Update Status", "Updates the status of the current user")]
    public class LyncUpdateStatus : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private int statusId;

        [DataMember]
        private string statusText;

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

                var contactInformation = new List<KeyValuePair<PublishableContactInformationType, object>>();
                contactInformation.Add(new KeyValuePair<PublishableContactInformationType, object>(PublishableContactInformationType.Availability, statusId));

                self.BeginPublishContactInformation(contactInformation, result => self.EndPublishContactInformation(result), "Publishing Status");
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
            get { return new LyncUpdateStatusConfig(statusId, statusText); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            LyncUpdateStatusConfig config = configurationControl as LyncUpdateStatusConfig;

            statusId = config.StatusId;
            statusText = config.StatusText;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Lync_UpdateStatusConfigString, statusText);
        }

        #endregion
    }
}
