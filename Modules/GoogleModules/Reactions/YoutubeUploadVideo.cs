using System.Globalization;
using System.Runtime.Serialization;
using GoogleModules.Resources;
using GoogleModules.Wpf;
using MayhemCore;
using MayhemWpf.UserControls;
using MayhemWpf.ModuleTypes;

namespace GoogleModules.Reactions
{
    /// <summary>
    /// A class used for uploading a video to an youtube chanel.
    /// </summary>
    [DataContract]
    [MayhemModule("Youtube: Upload Video", "Uploads a video to the predefined user's youtube channel")]
    public class YoutubeUploadVideo : ReactionBase, IWpfConfigurable
    {

        public override void Perform()
        {
 
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new YoutubeUploadVideoConfig(); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as YoutubeUploadVideoConfig;

            if (config == null)
            {
                return;
            }
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.MomentUrl_ConfigString, "lalalla");
        }

        #endregion
    }
}
