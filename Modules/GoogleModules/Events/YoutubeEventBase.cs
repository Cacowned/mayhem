using System.Runtime.Serialization;
using Google.YouTube;
using GoogleModules.Resources;

namespace GoogleModules.Events
{
    /// <summary>
    /// This is the base class for the Youtube Events.
    /// </summary>
    [DataContractAttribute]
    public abstract class YouTubeEventBase : GoogleModulesEventBase
    {
        protected YouTubeRequestSettings settings;
        protected YouTubeRequest request;

        protected virtual void InitializeYoutubeConnection()
        {
            settings = new YouTubeRequestSettings(Strings.YouTube_ProductName, Strings.YouTube_DeveloperKey);
            request = new YouTubeRequest(settings);
        }
    }
}
