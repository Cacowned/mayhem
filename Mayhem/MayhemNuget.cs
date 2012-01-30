using System;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using NuGet;
using System.Collections.Generic;

namespace Mayhem
{
	public class MayhemNuget
	{
		public static IPackageRepository Repository
		{
			get
			{
				// Create a repository pointing to the feed
				IPackageRepository repository;
				if (Environment.GetCommandLineArgs().Contains("-localrepo"))
				{
					repository = new LocalPackageRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\MayhemPackages\\Packages"));
				}
				else
				{
					repository = new DataServicePackageRepository(new Uri("http://makemayhem.com/www/nuget/"));
				}

				return repository;
			}
		}

		public static string InstallPath
		{
			get
			{
				// Get the package file
				string installPath;

				// if we are running as a click once application
				if (ApplicationDeployment.IsNetworkDeployed)
				{
					//installPath = Path.Combine(ApplicationDeployment.CurrentDeployment.DataDirectory, "Packages");
					
					// Use the user's documents folder:
					installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem", "Packages");

				}
				else
				{
					installPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");
				}

				return installPath;
			}
		}

		public static string[] InstallPaths
		{
			get
			{
				List<string> locations = new List<string>();
				// Add the local location
				locations.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages"));
				
				// Add the user document's folder
				locations.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem", "Packages"));
			}
		}

		public static string SettingsPath
		{
			get
			{
				// Get the package file
				string settingsPath;

				// if we are running as a click once application
				if (ApplicationDeployment.IsNetworkDeployed)
				{
					//installPath = Path.Combine(ApplicationDeployment.CurrentDeployment.DataDirectory, "Packages");

					// Use the user's documents folder:
					settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem", "settings.xml");

				}
				else
				{
					settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
				}

				return settingsPath;
			}
		}

		private static PackageManager manager;

		public static PackageManager PackageManager
		{
			get
			{
				if (manager == null)
					manager = new PackageManager(MayhemNuget.Repository, MayhemNuget.InstallPath);

				return manager;
			}
		}
	}
}
