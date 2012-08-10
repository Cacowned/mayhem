using System.Globalization;
using System.Runtime.Serialization;
using MayhemWpf.UserControls;
using SkypeModules.Resources;
using SkypeModules.Wpf;

namespace SkypeModules.Reactions
{
    /// <summary>
    /// An abstract class for defining the IWpfConfigurable methods and members used with the SkypeIDConfig user control.
    /// </summary>
    [DataContractAttribute]
    public abstract class SkypeIDWpfBase : SkypeIDReactionBase
    {
        #region IWpfConfigurable Methods

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as SkypeIDConfig;

            skypeID = config.SkypeID;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.SkypeID_ConfigString, skypeID);
        }

        #endregion
    }
}
