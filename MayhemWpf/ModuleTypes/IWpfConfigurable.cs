using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace MayhemCore.ModuleTypes
{
	/// <summary>
	/// Every module that has configuration
	/// that wants to be accessible from a WPF
	/// application needs to implement this interface
	/// </summary>
	public interface IWpfConfigurable
	{
        WpfConfiguration ConfigurationControl { get; }
        void OnSaved(WpfConfiguration configurationControl);
	}
}
