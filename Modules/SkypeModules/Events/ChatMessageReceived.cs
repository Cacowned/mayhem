using System;
using MayhemCore;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a Skype chat messege is received.
    /// </summary>
    [MayhemModule("Skype: Chat Message Received", "Triggers when a chat message is received")]
    public class ChatMessageReceived : SkypeEventBase
    {
        private _ISkypeEvents_MessageStatusEventHandler messageStatusEventHandler;

        protected override void OnAfterLoad()
        {
            messageStatusEventHandler = Skype_MessageStatus;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            base.OnEnabling(e);

            if (!e.Cancel)
            {
                skype.MessageStatus += new SKYPE4COMLib._ISkypeEvents_MessageStatusEventHandler(Skype_MessageStatus);
            }
        }

        void Skype_MessageStatus(SKYPE4COMLib.ChatMessage pMessage, TChatMessageStatus Status)
        {
            try
            {
                if (Status.Equals(TChatMessageStatus.cmsReceived))
                {
                    Trigger();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_EventCantBeTriggered, Strings.ReceiveChatMessage_Title));
                Logger.Write(ex);
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (skype != null)
            {
                skype.MessageStatus -= messageStatusEventHandler;
            }
        }
    }
}
