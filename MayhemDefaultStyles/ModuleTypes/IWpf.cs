
using System.Windows.Controls;
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
        UserControl ConfigurationControl { get; }
        void OnSaved(UserControl configurationControl);
	}
}
