using System;
using System.Linq;
using System.Windows;
using NuGet;
using System.IO;


namespace MayhemWpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
            MainWindow main = new MainWindow();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Application.Current.MainWindow = main;
			if (e.Args.Any())
			{
			    string packageFile = e.Args[0];
			    var zipFile = new ZipPackage(packageFile);

			    InstallModule window = new InstallModule(zipFile);
			
			    if (window.ShowDialog() == true)
			    {
				    // Installation was a success                
                    // Rescan for new modules
                    main.Mayhem.RescanModules();
			    }
            }

            main.Load();
            main.Show();
			
		}
	}
}
