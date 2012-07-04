using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using MayhemCore;
using NativeWifi;

namespace ConnectivityModule.Reactions
{
    /// <summary>
    /// An abstract base class that contains common code for the reactions that connect or disconnect to/from a network.
    /// </summary>
    [DataContract]
    public abstract class WiFiReactionBaseClass : ReactionBase
    {
        /// <summary>
        /// The name of the network.
        /// </summary>
        [DataMember]
        protected string networkName;

        protected WlanClient client;

        /// <summary>
        /// This method will initialize the objects of this class.
        /// </summary>
        protected override void OnAfterLoad()
        {
            client = null;
        }

        /// <summary>
        /// This method will connect or disconnect to/from a network executing the command received as a parameter.
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <returns>Returns true if the connectivity mode was changed successfully, false otherwise</returns>
        protected bool ChangeConnectivityMode(string command)
        {
            try
            {
                if (client == null)
                    client = new WlanClient();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_WiFiNotAvailable);
                Logger.Write(ex);

                return false;
            }

            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    if (network.profileName.Equals(networkName) || (network.profileName.Equals("") && networkName.Equals(GetStringForSSID(network.dot11Ssid))))
                    {
                        string profileXml = string.Format(@"<?xml version=""1.0""?>
<WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">
    <name>{0}</name>
    <SSIDConfig>
        <SSID>
            <name>{0}</name>
        </SSID>
            <nonBroadcast>false</nonBroadcast>
     </SSIDConfig>
     <connectionType>ESS</connectionType>
     <connectionMode>manual</connectionMode>
    <MSM>
        <security>
            <authEncryption>
                <authentication>open</authentication>
                <encryption>none</encryption>
                <useOneX>false</useOneX>
            </authEncryption>
        </security>
    </MSM>
</WLANProfile>", networkName);

                        bool found = false;
                        foreach (Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles())
                        {
                            if (profileInfo.profileName.Equals(network.profileName))
                            {
                                found = true;

                                break;
                            }
                        }

                        if (!found)
                            wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);

                        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/C " + command);

                        processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(processInfo);

                        Thread.Sleep(2000); // We wait for the connect/disconnect command to finish.

                        return true;
                    }
                }
            }

            // If the desired network wasn't found we assume is not in reach.
            ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.WiFi_NetworkNotVisible, networkName));

            return false;
        }

        /// <summary>
        /// Transforms the ssid of a network into a string representing it's name.
        /// </summary>
        /// <param name="ssid">The ssid of the network</param>
        /// <returns>The name of the network</returns>
        protected string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        /// <summary>
        /// This method will be implemented by the classes that inherit this class and will be called when the event associated with the reaction is triggered.
        /// It contains the functionality of this reaction.
        /// </summary>
        public abstract override void Perform();
    }
}
