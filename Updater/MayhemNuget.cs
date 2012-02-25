using System;
using System.IO;
using System.Linq;
using NuGet;
using System.Collections.Generic;

namespace Updater
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
					repository = new LocalPackageRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\MayhemPackages\Packages"));
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
				//installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem", "Packages");
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..");
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