using System;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Facebook;
using MayhemCore;

namespace FacebookModules
{
    /// <summary>
    /// Base class for all Facebook Events
    /// </summary>
    [DataContract]
    public abstract class FacebookEventBase : EventBase
    {

        private const string AppId = "156845751071300";
        private string[] extendedPermissions = new[] { "user_about_me", "offline_access" };

       

        protected virtual void Login_Click()
        {
           // var fbLogin = new FacebookLogin(AppId, extendedPermissions);
           // fbLogin.ShowDialog();
            //DisplayMessage(fbLogin.FacebookOAuth);
        }


        private void DisplayMessage(FacebookOAuthResult fbResult)
        {
           
        }

        protected virtual void LoadPerson(string id, BitmapImage bi) 
        {
        }
    }




}
