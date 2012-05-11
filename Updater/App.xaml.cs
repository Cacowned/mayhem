using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using NuGet;

namespace Updater
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			//MessageBox.Show("Attach");
			string root = @"..\..";

			if (e.Args.Length > 0) {
				root = e.Args[0];
			}

			try
			{
				PackageManager manager = new PackageManager(MayhemNuget.Repository, root);
				//manager.InstallPackage("Mayhem");
				//manager.InstallPackage("DebugModules");

				List<IPackage> packagesLocal = manager.LocalRepository.GetPackages().ToList();

				List<IPackage> updates = manager.SourceRepository.GetUpdates(packagesLocal).ToList();
				if (updates.Count > 0)
				{
					foreach (IPackage package in updates)
					{
						manager.UpdatePackage(package, updateDependencies: true);

						if (package.Id == "Mayhem")
						{
							if (Directory.Exists(Path.Combine(root, "Mayhem")))
							{
								// We have to do this to unlock the directory
								Directory.SetCurrentDirectory(root);
								Directory.Delete(Path.Combine(root, "Mayhem"), true);
							}

							string[] directories = Directory.GetDirectories(root, package.Id+"."+package.Version);

							if (directories.Length == 1) {
								Directory.Move(directories[0], Path.Combine(root, "Mayhem"));
							}
						}
					}
				}
			}
			catch (WebException)
			{
				MessageBox.Show("Server is unavailable. Please try again later.");
			}

			try
			{
				Process.Start(Path.Combine(root, "Mayhem", "content", "Mayhem.exe"));
			}
			catch(Win32Exception)
			{
				MessageBox.Show("Unable to locate the Mayhem application");
			}
			Shutdown();
		}
	}
}
