using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitterizer;
using System.Diagnostics;

namespace MayhemApp.Business_Logic.Twitter
{
    /**<summary>
     * Singleton Class that encapsulates all the Twitter stuff used by Mayhem
     * </summary>
     * */
    class MayhemTwitter
    {

        public enum TWITTER_STATE
        {
            OFF_NOTOKEN,
            OFF_TOKEN,
            ON
        }

        private static MayhemTwitter _instance = null;

        public static MayhemTwitter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MayhemTwitter();
                }
                return _instance;
            }

        }

        public static string TAG = "[MayhemTwitter] :";

        private string TWITTER_CONSUMER_KEY = "iw29isfyKmNca4sVfVGTLA";
        private string TWITTER_CONSUMER_SECRET = "dvgRxKbpG1U5uRRoezfI3wXEaueFovovQU1jNpxng";
        private string TWITTER_CALLBACK_DOMAIN = "oob";


        private string TWITTER_REQUEST_TOKEN = null;
        private string TWITTER_ACCESS_TOKEN = null;
        private string TWITTER_ACCESS_TOKEN_SECRET = null;

        public TWITTER_STATE twitterState = TWITTER_STATE.OFF_NOTOKEN;

        private Dictionary<string, string> myTwitterSettings = null;

        private OAuthTokens TOKENS = null;

        private static string TWITTER_SETTINGS_KEY = "TwitterSettings";

        // references to the getter/setter functions of the properties list
        // this allows me to keep the dictionary loader unspecific to the property in question
        private Action<String> TWITTER_PROPERTY_SET = (value => Properties.Settings.Default.TwitterSettings = value);
        private Func<String> TWITTER_PROPERTY_GET = (() => Properties.Settings.Default.TwitterSettings);




        public MayhemTwitter()
        {
            Properties.Settings.Default.Reload();
            TWITTER_SETTINGS_KEY = Properties.Settings.Default.TwitterSettings;
            Dictionary<string, string> stored_twitter_settings = MayhemSettingsDictionaryLoader.LoadDictionaryWithKey(TWITTER_PROPERTY_GET);

            if (stored_twitter_settings == null || stored_twitter_settings.Count == 0)
            {
                Debug.WriteLine(TAG + "Twitter Settings Empty!");
                myTwitterSettings = new Dictionary<string, string>();
            }
            else
            {
                try
                {
                    TWITTER_REQUEST_TOKEN = stored_twitter_settings["TWITTER_REQUEST_TOKEN"];
                    TWITTER_ACCESS_TOKEN = stored_twitter_settings["TWITTER_ACCESS_TOKEN"];
                    TWITTER_ACCESS_TOKEN_SECRET = stored_twitter_settings["TWITTER_ACCESS_TOKEN_SECRET"];
                    twitterState = (TWITTER_STATE)Enum.Parse(typeof(TWITTER_STATE), stored_twitter_settings["TWITTER_STATE"]);

                    this.TOKENS = GenerateTokens();

                }
                catch (KeyNotFoundException)
                {
                    Debug.WriteLine(TAG + "Exception parsing config dictionary -- KeyNotFoundException");
                }


                myTwitterSettings = stored_twitter_settings;
            }

        }

        public void SaveTwitterSettings()
        {
            if (myTwitterSettings != null)
            {
                myTwitterSettings["TWITTER_REQUEST_TOKEN"] = TWITTER_REQUEST_TOKEN;
                myTwitterSettings["TWITTER_ACCESS_TOKEN"] = TWITTER_ACCESS_TOKEN;
                myTwitterSettings["TWITTER_STATE"] = twitterState.ToString();
                myTwitterSettings["TWITTER_ACCESS_TOKEN_SECRET"] = TWITTER_ACCESS_TOKEN_SECRET;

                // Evil Kludge 
                if (MayhemSettingsDictionaryLoader.SaveDictionaryWithKey(myTwitterSettings, TWITTER_PROPERTY_SET))
                {
                    Debug.WriteLine(TAG + "Settings Saved Successfully");
                }
                else
                {
                    Debug.WriteLine(TAG + "Problem Saving Settings");
                }
            }
            else
            {
                Debug.WriteLine(TAG + "Twitter Settings NULL");
            }
        }

        ~MayhemTwitter()
        {
            Debug.WriteLine(TAG + "Saving Current Twitter Settings");

            // save the current settings on destruct
            SaveTwitterSettings();
        }


        /**<summary>
         * Generates an authentication token, to allow generation of an authentication URI.
         * </summary>
         * */
        public bool GetRequestToken()
        {
            Twitterizer.OAuthTokenResponse resp = null;

            try
            {
                resp = Twitterizer.OAuthUtility.GetRequestToken(TWITTER_CONSUMER_KEY, TWITTER_CONSUMER_SECRET, TWITTER_CALLBACK_DOMAIN);
                TWITTER_REQUEST_TOKEN = resp.Token;

            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception!!! " + e);
                return false;
            }

            return true;
        }

        /**<summary>
         * Builds the authentication URI that must be shown to the user in order to obtain a passcode.
         * </summary>
         * */
        public Uri BuildAuthURI()
        {
            if (TWITTER_REQUEST_TOKEN != null)
            {
                Uri auth_uri = Twitterizer.OAuthUtility.BuildAuthorizationUri(TWITTER_REQUEST_TOKEN);

                Debug.WriteLine(auth_uri);
                return auth_uri;
            }
            else
            {
                Debug.WriteLine(TAG + " Get a Request Token first!");
                return null;
            }
        }

        /**<summary>
         * Used to get the access token from Twitter
         * This token can be re-used, so try not to loose it. 
         * </summary>
         * */
        public bool GetAccessToken(string verifier)
        {
            try
            {
                Twitterizer.OAuthTokenResponse r = Twitterizer.OAuthUtility.GetAccessToken(TWITTER_CONSUMER_KEY, TWITTER_CONSUMER_SECRET, TWITTER_REQUEST_TOKEN, verifier);
                TWITTER_ACCESS_TOKEN = r.Token;
                TWITTER_ACCESS_TOKEN_SECRET = r.TokenSecret;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(TAG + " exception in GetAccessToken");
                Debug.WriteLine(e);
                return false;
            }
        }

        private OAuthTokens GenerateTokens()
        {
            OAuthTokens tokens = new OAuthTokens();

            tokens.AccessToken = this.TWITTER_ACCESS_TOKEN;
            tokens.AccessTokenSecret = this.TWITTER_ACCESS_TOKEN_SECRET;
            tokens.ConsumerKey = this.TWITTER_CONSUMER_KEY;
            tokens.ConsumerSecret = this.TWITTER_CONSUMER_SECRET;

            return tokens;
        }

        public bool SendTweet(string tweet)
        {
            if (tweet.Length > 140)
            {
                Debug.WriteLine(TAG + "Error: Tweet is too long");
                return false;
            }
            else
            {

                if (this.twitterState == TWITTER_STATE.ON && TWITTER_ACCESS_TOKEN != null)
                {
                    TwitterResponse<TwitterStatus> resp = Twitterizer.TwitterStatus.Update(TOKENS, tweet);

                    if (resp.Result == RequestResult.Success)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine(TAG + "Error sending the tweet: " + resp.Result.ToString());
                        Debug.WriteLine(resp.ErrorMessage + "\n" + resp.Content);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
