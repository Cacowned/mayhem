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
    /// This reaction updates the personal note of the current user
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Update Personal Note", "Updates the personal note of the current user")]
    public class LyncUpdatePersonalNote : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string personalNote;

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
                contactInformation.Add(new KeyValuePair<PublishableContactInformationType, object>(PublishableContactInformationType.PersonalNote, personalNote));

                self.BeginPublishContactInformation(contactInformation, result => self.EndPublishContactInformation(result), "Publishing Personal Note");
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
            get { return new LyncUpdatePersonalNoteConfig(personalNote); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            LyncUpdatePersonalNoteConfig config = configurationControl as LyncUpdatePersonalNoteConfig;

            personalNote = config.PersonalNote;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Lync_UpdatePersonalNoteConfigString, personalNote);
        }

        #endregion
    }
}
