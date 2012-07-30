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
    /// A class used for posting the selected blog entry to the user's Google+ History page.
    /// </summary>
    [DataContract]
    [MayhemModule("Google+: Post Blog Entry", "Posts the selected blog entry to Google+ History")]
    public class GooglePlusPostBlogEntry : GooglePlusBaseReaction, IWpfConfigurable
    {
        public override void Perform()
        {
            try
            {
                AddActivity("http://schemas.google.com/CreateActivity");

                ErrorLog.AddError(ErrorType.Message, Strings.GooglePlus_BlogEntrySuccesfulAdded);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                ErrorLog.AddError(ErrorType.Failure, Strings.GooglePlus_BlogEntryCouldntBeAdded);
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new GooglePlusAddMomentConfig(momentUrl, Strings.GooglePlusPostBlogEntry_Title, Strings.GooglePlus_DetailsPostBlogEntry, Strings.GooglePlus_PostBlogEntryUrlText); }
        }
    }
}
