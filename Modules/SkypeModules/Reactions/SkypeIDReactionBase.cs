using System.Runtime.Serialization;
using MayhemCore;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Reactions
{
    /// <summary>
    /// An abstract base class that contains the Skype ID and a method for verifying if it's valid.
    /// </summary>
    [DataContract]
    public abstract class SkypeIDReactionBase : SkypeReactionBase
    {
        [DataMember]
        protected string skypeID;

        protected bool VerifySkypeIDValidity(string skypeId)
        {
            bool found = false;

            foreach (User user in skype.Friends)
            {
                if (user.Handle.Equals(skypeID))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_ContactNotFound, skypeID));
            }

            return found;
        }
    }
}
