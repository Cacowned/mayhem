using System;
using MayhemCore;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Events
{
    /// <summary>
    /// /// The base clase for any Skype event which deals with connecting with the Skype Client.
    /// </summary>
    public abstract class SkypeEventBase : EventBase
    {
        protected Skype skype;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                skype = new Skype();

                if (!skype.Client.IsRunning)
                {
                    skype.Client.Start();
                }

                skype.Attach(7, false);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                ErrorLog.AddError(ErrorType.Failure, Strings.Error_ConnectingSkype);
                Logger.Write(ex);
            }
        }
    }
}
