using System.Runtime.Serialization;
using System.Timers;
using MayhemCore;

namespace GoogleModules.Events
{
    /// <summary>
    /// Base class for all the GoogleModules Events.
    /// </summary>
    [DataContractAttribute]
    public abstract class GoogleModulesEventBase : EventBase
    {
        protected Timer timer;

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

        protected abstract void timer_Elapsed(object sender, ElapsedEventArgs e);
    }
}
