using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using NuGet;
using System.IO;
using System.ComponentModel;

namespace Updater
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			string root = @"..\..";

			try
			{
				PackageManager manager = MayhemNuget.PackageManager;
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
								Directory.Delete(Path.Combine(root, "Mayhem"), true);

							string[] directories = Directory.GetDirectories(root, package.Id+"."+package.Version);

							if (directories.Length == 1) {
								Directory.Move(directories[0], Path.Combine(root, "Mayhem"));
							}
						}
					}
				}
			}
			catch (Exception erf)
			{
				Debug.WriteLine(erf);
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
