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

		private NameValueCollection GetQueryStringParameters()
		{
			NameValueCollection nameValueTable = new NameValueCollection();

			if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.ActivationUri != null)
			{
				string queryString = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;
				nameValueTable = System.Web.HttpUtility.ParseQueryString(queryString);
			}

			return nameValueTable;
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// TODO: Hack, if we are network deployed, restart as regular executable
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				string arguments = string.Empty;

				var query = GetQueryStringParameters();
				if (query["addon"] != null)
				{
					arguments = query["addon"];
				}

				Process.Start(Assembly.GetEntryAssembly().Location, arguments);
				Shutdown();
				return;
			}

			Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

			// if we don't have a packages folder, create it.
			if (!Directory.Exists(MayhemNuget.InstallPath))
			{
				Directory.CreateDirectory(MayhemNuget.InstallPath);
			}

			if (e.Args.Any())
			{
				if (e.Args.Contains("-installupdates"))
				{
					CheckForUpdates(true);
					Process.Start(Assembly.GetEntryAssembly().Location);
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
					
					ZipPackage zipFile;
					try
					{
						zipFile = new ZipPackage(packageFile);
					}
					catch (ArgumentException)
					{
						// It's likely a URL, try to read it as a stream
						WebRequest req = WebRequest.Create(e.Args[0]);
						WebResponse response = req.GetResponse();
						Stream stream = response.GetResponseStream();
						zipFile = new ZipPackage(stream);
					}

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
				InstallStart window = new InstallStart();

				if (window.ShowDialog() == true)
				{
					// Installation was a success
					// So lets restart with the new modules
					Process.Start(Assembly.GetEntryAssembly().Location);
				}

				// Close the application 
				Shutdown();
				return;
			}
/*
			FileSystemWatcher fileWatcher = new FileSystemWatcher(MayhemNuget.InstallPath);
			fileWatcher.IncludeSubdirectories = true;
			fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
			fileWatcher.Changed += UpdateDependencies;
			fileWatcher.Created += UpdateDependencies;
			fileWatcher.Deleted += UpdateDependencies;
			fileWatcher.Renamed += UpdateDependencies;
			fileWatcher.EnableRaisingEvents = true;

			lastUpdated = DateTime.Now;
			*/
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
		/*
		private void UpdateDependencies(object source, FileSystemEventArgs e)
		{
			var nowTime = DateTime.Now;

			if ((nowTime - lastUpdated).TotalMilliseconds > 500)
			{
				dependencies.Clear();

				LoadDependencies();

				lastUpdated = nowTime;
			}
		}*/

		private void LoadDependencies()
		{
			foreach (string location in MayhemNuget.InstallPaths)
			{
				string[] files = Directory.GetFiles(location, "*.dll", SearchOption.AllDirectories);
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
			}

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

		private void Current_Exit(object sender, ExitEventArgs e)
		{
			if (wantsUpdates)
			{
				if (Environment.GetCommandLineArgs().Contains("-localrepo"))
					Process.Start(Assembly.GetEntryAssembly().Location, "-installupdates -localrepo");
				else
					Process.Start(Assembly.GetEntryAssembly().Location, "-installupdates");
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
