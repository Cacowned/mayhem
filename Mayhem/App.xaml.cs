using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using NuGet;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using MayhemCore;
using MayhemWpf.ModuleTypes;

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

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

			if (e.Args.Any())
			{
				//LoadDependencies();
				string packageFile = e.Args[0];

				ZipPackage zipFile = new ZipPackage(packageFile);

				InstallModule window = new InstallModule(zipFile);

				if (window.ShowDialog() == true)
				{
					// Installation was a success
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
				// Don't load the Updater. It has to remain unlocked.
				DirectoryInfo info = Directory.GetParent(file).Parent;
				string name = info.Name;
				if (name != "Updater" && !name.StartsWith("Updater."))
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
					// If we have the updater
					if (manager.LocalRepository.Exists("Updater"))
					{
						IPackage updater = manager.LocalRepository.FindPackage("Updater");
						List<IPackage> packages = new List<IPackage>();
						packages.Add(updater);

						// check it for updates
						updates = manager.SourceRepository.GetUpdates(packages).ToList();

						foreach (IPackage package in updates)
						{
							// and install it if it has any
							manager.UpdatePackage(package, updateDependencies: true);
						}
					}
					else
					{
						// otherwise, install the updater
						manager.InstallPackage("Updater");
					}

					Dispatcher.Invoke((Action)delegate
					{
						if (MessageBox.Show("Updates are available. Would you like to update?", "Mayhem Updates", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
						{
							string[] directories = Directory.GetDirectories(@"..\..\", "Updater.*");
							if (directories.Length == 0 || !File.Exists(Path.Combine(directories[directories.Length-1], "content", "Updater.exe")))
							{
								MessageBox.Show("Error: Unable to find the updater!");
							}
							else
							{
								Process.Start(Path.Combine(directories[directories.Length-1], "content", "Updater.exe"));
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
