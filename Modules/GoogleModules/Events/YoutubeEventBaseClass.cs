using System.Runtime.Serialization;
using System.Timers;
using Google.YouTube;
using GoogleModules.Resources;
using MayhemCore;

namespace GoogleModules.Events
{
    /// <summary>
    /// This is the base class for the Youtube Events.
    /// </summary>
    [DataContractAttribute]
    public abstract class YoutubeEventBaseClass : EventBase
    {
        protected Timer timer;

        protected YouTubeRequestSettings settings;
        protected YouTubeRequest request;

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= timer_Elapsed;
                timer.Dispose();
            }
        }

        protected void StartTimer(int interval)
        {
            timer = new Timer();
            timer.Interval = interval;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            timer.Start();
        }

        protected virtual void InitializeYoutubeConnection()
        {
            settings = new YouTubeRequestSettings(Strings.Youtube_ProductName, Strings.Youtube_DeveloperKey);
            request = new YouTubeRequest(settings);
        }

        protected abstract void timer_Elapsed(object sender, ElapsedEventArgs e);
    }
}
