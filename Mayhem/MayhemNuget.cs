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
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\");
			}
		}

		public static string SettingsPath
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", "settings.xml");
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
