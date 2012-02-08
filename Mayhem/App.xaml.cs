using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using NuGet;
using System.Security.Principal;
using System.ComponentModel;

namespace Mayhem
{
	public partial class App : Application
	{
		private readonly Dictionary<string, Assembly> dependencies;

		private bool wantsUpdates;

		private DateTime lastUpdated;


		public delegate void DependenciesUpdatedHandler(object sender, DependenciesEventArgs e);
		public event DependenciesUpdatedHandler DependenciesUpdated;

		public App()
		{
			dependencies = new Dictionary<string, Assembly>();
		}

		private static void RunElevated(string fileName, string arguments = "")
		{
			ProcessStartInfo processInfo = new ProcessStartInfo();
			processInfo.Verb = "runas";
			processInfo.FileName = fileName;
			processInfo.Arguments = arguments;

			try
			{
				Process.Start(processInfo);
			}
			catch (Win32Exception)
			{
				//Do nothing. Probably the user canceled the UAC window 
			}
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

			//MessageBox.Show("Attach");

			if (e.Args.Length == 1)
			{
				// One argument, it's a file location

				WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

				if (!pricipal.IsInRole(WindowsBuiltInRole.Administrator))
				{
					string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

					RunElevated(Path.Combine(path, "Mayhem.exe"), e.Args[0]);
				}
				else
				{
					LoadDependencies();

					string packageFile = e.Args[0];

					ZipPackage zipFile = new ZipPackage(packageFile);

					InstallModule window = new InstallModule(zipFile);

					if (window.ShowDialog() == true)
					{
						// Installation was a success
					}
				}

				// Close the application 
				Shutdown();
				return;
			}

			ThreadPool.QueueUserWorkItem(CheckForUpdates);

			FileWatcher watcher = new FileWatcher(MayhemNuget.InstallPath, LoadDependencies);

			// Load the correct dependency assemblies
			LoadDependencies();

			MainWindow main = new MainWindow(this);
			Current.MainWindow = main;

			AnnounceDependencies();

			main.Load();
			main.Show();
		}

		private void LoadDependencies()
		{
			string[] files = Directory.GetFiles(MayhemNuget.InstallPath, "*.dll", SearchOption.AllDirectories);
			foreach (string file in files)
			{
				try
				{
					Assembly assembly = Assembly.LoadFrom(file);

					// If we haven't already added it.
					// This is in the case that we happen to have two copies of the same
					// package installed (Visual Studio and NuGet version for example)
					if (!dependencies.ContainsKey(assembly.FullName))
					{
						dependencies.Add(assembly.FullName, assembly);

						// Scan for modules in the module directory
					}
				}
				catch
				{
				}
			}
			AnnounceDependencies();

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

		private void AnnounceDependencies()
		{
			if (DependenciesUpdated != null)
				DependenciesUpdated(this, new DependenciesEventArgs(dependencies.Values));
		}

		// This allows it to be called in a different thread
		private void CheckForUpdates(object obj)
		{
			CheckForUpdates();
		}

		private void CheckForUpdates()
		{
			try
			{
				var manager = MayhemNuget.PackageManager;

				List<IPackage> packagesLocal = manager.LocalRepository.GetPackages().ToList();

				List<IPackage> updates = manager.SourceRepository.GetUpdates(packagesLocal).ToList();
				if (updates.Count > 0)
				{
					var updaterManager = new PackageManager(MayhemNuget.Repository, MayhemNuget.AppDataFolder);
					// If we have the updater
					if (updaterManager.LocalRepository.Exists("Updater"))
					{
						IPackage updater = updaterManager.LocalRepository.FindPackage("Updater");
						List<IPackage> packages = new List<IPackage>();
						packages.Add(updater);

						// check it for updates
						updates = updaterManager.SourceRepository.GetUpdates(packages).ToList();

						foreach (IPackage package in updates)
						{
							// and install it if it has any
							updaterManager.UpdatePackage(package, updateDependencies: true);
						}
					}
					else
					{
						// otherwise, install the updater
						updaterManager.InstallPackage("Updater");
					}

					Dispatcher.Invoke((Action)delegate
					{
						if (MessageBox.Show("Updates are available. Would you like to update?", "Mayhem Updates", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
						{
							string[] directories = Directory.GetDirectories(MayhemNuget.AppDataFolder, "Updater.*");
							if (directories.Length == 0 || !File.Exists(Path.Combine(directories[directories.Length - 1], "content", "Updater.exe")))
							{
								MessageBox.Show("Error: Unable to find the updater!");
							}
							else
							{
								Process.Start(Path.Combine(directories[directories.Length - 1], "content", "Updater.exe"), "\"" + MayhemNuget.InstallPath + "\"");
								Shutdown();
							}
						}
					});
				}
				else
				{
					Debug.WriteLine("No Updates");
				}
			}
			catch (Exception erf)
			{
				OutputException(erf);
			}
		}

		public static void OutputException(Exception e, int indent = 0)
		{
			if (e != null)
			{
				string prefix = new string(' ', indent * 4);
				Debug.WriteLine(prefix + "Message: " + e.Message);
				Debug.WriteLine(prefix + "Target Site: " + e.TargetSite);
				OutputException(e.InnerException, indent + 1);
			}
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (dependencies.ContainsKey(args.Name))
				return dependencies[args.Name];

			return null;
		}
	}

	public class DependenciesEventArgs : EventArgs
	{
		public IEnumerable<Assembly> Assemblies
		{
			get;
			private set;
		}

		public DependenciesEventArgs(IEnumerable<Assembly> assemblies)
		{
			this.Assemblies = assemblies;
		}
	}
}
