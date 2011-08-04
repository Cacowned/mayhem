using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace MayhemCore.ModuleTypes
{
	/// <summary>
	/// Every module that has configuration
	/// that wants to be accessible from a WPF
	/// application needs to implement this interface
	/// </summary>
	public interface IWpf
	{
//		void WpfConfig();
        IWpfConfig ConfigurationControl { get; }
        void OnSaved(IWpfConfig configurationControl);
	}
}
