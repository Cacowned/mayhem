using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using NuGet;

namespace Mayhem
{
	public partial class App : Application
	{
		private readonly Dictionary<string, Assembly> dependencies;
		private bool wantsUpdates;

		private DateTime lastUpdated;

		public App()
		{
			dependencies = new Dictionary<string, Assembly>();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
			
			// if we don't have a packages folder, create it.
			if (!Directory.Exists(MayhemNuget.InstallPath))
			{
				Directory.CreateDirectory(MayhemNuget.InstallPath);
			}
			
			LoadDependencies();
			bool containsCore = false;
			foreach (string dependency in dependencies.Keys)
			{
				if (dependency.Contains("MayhemCore"))
				{
					containsCore = true;
					break;
				}
			}

			if (!containsCore)
			{
				MessageBox.Show("This is the first time Mayhem has been run on this computer. Setting up. This may take a few minutes.");
                try
                {
                    var packageManager = MayhemNuget.PackageManager;
                    packageManager.InstallPackage("DefaultModules");
                }
                catch
                {
                    MessageBox.Show("Unable to connect to the Mayhem servers to set up. Please connect to the internet and try again.", "Setup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }
				MessageBox.Show("Set up successfully. Click OK to start Mayhem.");
			}

			if (e.Args.Any())
			{
				if (e.Args.Contains("-installupdates"))
				{
					CheckForUpdates(true);
					Process.Start("Mayhem.exe");
					Shutdown();
					return;
				}
				else if (e.Args.Contains("-noupdates"))
				{
					// Do nothing
				}
				else
				{
					LoadDependencies();
					string packageFile = e.Args[0];
					var zipFile = new ZipPackage(packageFile);

					InstallModule window = new InstallModule(zipFile);

					if (window.ShowDialog() == true)
					{
						// Installation was a success
					}

					// Close the application 
					Shutdown();
					return;
				}
			}
			else
			{
				Current.Exit += Current_Exit;
				ThreadPool.QueueUserWorkItem(CheckForUpdates);
			}

			FileSystemWatcher fileWatcher = new FileSystemWatcher(MayhemNuget.InstallPath);
			fileWatcher.IncludeSubdirectories = true;
			fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
			fileWatcher.Changed += UpdateDependencies;
			fileWatcher.Created += UpdateDependencies;
			fileWatcher.Deleted += UpdateDependencies;
			fileWatcher.Renamed += UpdateDependencies;
			fileWatcher.EnableRaisingEvents = true;

			lastUpdated = DateTime.Now;

			// Load the correct dependency assemblies
			LoadDependencies();

			MainWindow main = new MainWindow();
			Current.MainWindow = main;

			main.Load();
			main.Show();
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

		private void UpdateDependencies(object source, FileSystemEventArgs e)
		{
			var nowTime = DateTime.Now;

			if ((nowTime - lastUpdated).TotalMilliseconds > 500)
			{
				dependencies.Clear();

				LoadDependencies();

				lastUpdated = nowTime;
			}
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
					}
				}
				catch
				{
				}
			}

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

		private void Current_Exit(object sender, ExitEventArgs e)
		{
			if (wantsUpdates)
			{
				if (Environment.GetCommandLineArgs().Contains("-localrepo"))
					Process.Start("Mayhem.exe", "-installupdates -localrepo");
				else
					Process.Start("Mayhem.exe", "-installupdates");
			}
		}

		private void CheckForUpdates(object obj)
		{
			CheckForUpdates(false);
		}

		private void CheckForUpdates(bool install)
		{
			try
			{
				var manager = MayhemNuget.PackageManager;

				List<IPackage> packagesLocal = manager.LocalRepository.GetPackages().ToList();

				List<IPackage> updates = manager.SourceRepository.GetUpdates(packagesLocal).ToList();
				if (updates.Count > 0)
				{
					if (install)
					{
						Dispatcher.Invoke((Action)delegate
						{
							UpdateModules window = new UpdateModules(updates);
							if (window.ShowDialog() == true)
							{
								// Installation was a success
							}
						});
					}
					else
					{
						Dispatcher.Invoke((Action)delegate
						{
							if (MessageBox.Show("Updates are available. Would you like to update?", "Mayhem Updates", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
							{
								wantsUpdates = true;
								Shutdown();
							}
						});
					}
				}
				else
				{
					Debug.WriteLine("No Updates");
				}
			}
			catch (Exception erf)
			{
				Debug.WriteLine(erf);
			}
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (dependencies.ContainsKey(args.Name))
				return dependencies[args.Name];

			return null;
		}
	}
}
