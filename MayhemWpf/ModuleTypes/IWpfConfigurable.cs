using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;

namespace MayhemWpf.ModuleTypes
{
    /// <summary>
    /// Every module that has configuration and wants to be accessible from the WPF
    /// application needs to implement this interface
    /// </summary>
    public interface IWpfConfigurable : IConfigurable
    {
        /// <summary>
        /// The instance of the relevant configuration user control
        /// </summary>
        WpfConfiguration ConfigurationControl
        {
            get;
        }

        /// <summary>
        /// Called when the configuration is saved.
        /// </summary>
        /// <param name="configurationControl">The instance of the configuration control that was saved</param>
        void OnSaved(WpfConfiguration configurationControl);
    }
}
