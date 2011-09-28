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
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        Dictionary<string, Assembly> dependencies = new Dictionary<string, Assembly>();
        bool wantsUpdates = false;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            //foreach (string arg in e.Args)
            //{
            //    Logger.WriteLine(arg);
            //}

            if (e.Args.Any())
            {
                if (e.Args.Contains("-installupdates"))
                {
                    CheckForUpdates(true);
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
                    else
                    {
                        // Installation failed?
                    }

                    // Close the application 
                    this.Shutdown();
                    return;
                }
            }
            else
            {
                Application.Current.Exit += new ExitEventHandler(Current_Exit);
                ThreadPool.QueueUserWorkItem(new WaitCallback(CheckForUpdates));
            }

            //Load the correct dependency assemblies
            LoadDependencies();

            MainWindow main = new MainWindow();
            Application.Current.MainWindow = main;

            main.Load();
            main.Show();
		}

        void LoadDependencies()
        {
            string[] files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Packages"), "*.dll", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    dependencies.Add(assembly.FullName, assembly);
                }
                catch
                {
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            if (wantsUpdates)
            {
                if (Environment.GetCommandLineArgs().Contains("-localrepo"))
                    Process.Start("Mayhem.exe", "-installupdates -localrepo");
                else
                    Process.Start("Mayhem.exe", "-installupdates");

            }
        }

        void CheckForUpdates(object obj)
        {
            CheckForUpdates(false);
        }

        void CheckForUpdates(bool install)
        {
            try
            {
                IPackageRepository repository;
                if(Environment.GetCommandLineArgs().Contains("-localrepo"))
                    repository = new LocalPackageRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\MayhemPackages\\Packages"));
                else
                    repository = new DataServicePackageRepository(new Uri("http://makemayhem.com.cloudsites.gearhost.com/nuget/"));

                var manager = new PackageManager(repository, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages"));

                List<IPackage> packagesLocal = manager.LocalRepository.GetPackages().ToList();

                List<IPackage> updates = manager.SourceRepository.GetUpdates(packagesLocal).ToList();
                if (updates.Count > 0)
                {
                    if (install)
                    {
                        foreach (IPackage package in updates)
                        {
                            Debug.WriteLine("Update: " + package.GetFullName());
                            manager.UpdatePackage(package, true);
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke((Action)delegate
                        {
                            if (MessageBox.Show("Press yes to get updates", "Mayhem Updates", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                wantsUpdates = true;
                                this.Shutdown();
                            }
                        });
                    }
                }
            }
            catch (Exception erf)
            {
                Debug.WriteLine(erf);
            }
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (dependencies.ContainsKey(args.Name))
                return dependencies[args.Name];
            return null;
        }
	}
}
