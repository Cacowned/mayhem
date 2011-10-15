using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;

namespace MayhemWpf.ModuleTypes
{
    /// <summary>
    /// Every module that has configuration
    /// that wants to be accessible from a WPF
    /// application needs to implement this interface
    /// </summary>
    public interface IWpfConfigurable : IConfigurable
    {
        WpfConfiguration ConfigurationControl
        {
            get;
        }

        void OnSaved(WpfConfiguration configurationControl);
    }
}
