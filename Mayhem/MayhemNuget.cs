using System;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using NuGet;

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
					installPath = ApplicationDeployment.CurrentDeployment.DataDirectory;
				}
				else
				{
					installPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");
				}

				return installPath;
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
