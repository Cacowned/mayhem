using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace MayhemCore.ModuleTypes
{
	/// <summary>
	/// Every module that has configuration
	/// that wants to be accessible from a WPF
	/// application needs to implement this interface
	/// </summary>
	public interface IWpfConfigurable
	{
        IWpfConfiguration ConfigurationControl { get; }
        void OnSaved(IWpfConfiguration configurationControl);
	}
}
