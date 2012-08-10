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
    /// A class that will modify the status of the current user to the predefined status.
    /// </summary>
    [DataContract]
    [MayhemModule("Skype: Change Status", "Changes the status of the current user")]
    public class ChangeStatus : SkypeReactionBase, IWpfConfigurable
    {
        [DataMember]
        private TUserStatus status;

        public override void Perform()
        {
            try
            {
                skype.ChangeUserStatus(status);
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Error_CantChangeStatus);
                Logger.Write(ex);
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new SkypeStatusConfig(status); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as SkypeStatusConfig;

            status = config.Status;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Status_ConfigString, status.ToString().Replace("cus", ""));
        }

        #endregion
    }
}
