using System;
using System.Runtime.Serialization;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace GoogleModules.Reactions
{
    /// <summary>
    /// A class used for posting the selected video to the user's Google+ History page.
    /// </summary>
    [DataContract]
    [MayhemModule("Google+: Post Video", "Posts the selected video to Google+ History")]
    public class GooglePlusPostVideo : GooglePlusBaseReaction, IWpfConfigurable
    {
        public override void Perform()
        {
            try
            {
                Authentificate();

                AddActivity("http://schemas.google.com/ListenActivity");

                ErrorLog.AddError(ErrorType.Message, Strings.GooglePlus_VideoSuccesfulAdded);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                ErrorLog.AddError(ErrorType.Failure,  Strings.GooglePlus_VideoCouldntBeAdded);
            }
        }
        
        public WpfConfiguration ConfigurationControl
        {
            get { return new GooglePlusAddMomentConfig(MomentUrl, Strings.GooglePlusPostVideo_Title, Strings.GooglePlus_DetailsPostVideo, Strings.GooglePlus_PostVideoUrlText); }
        }
    }
}
