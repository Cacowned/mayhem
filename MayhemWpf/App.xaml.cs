using System;
using System.Linq;
using System.Windows;
using NuGet;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace MayhemWpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        Dictionary<string, Assembly> dependencies = new Dictionary<string, Assembly>();
        bool wantsUpdates = false;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        HashSet<string> setDirectories = new HashSet<string>();

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
            /*
            var repository = PackageRepositoryFactory.Default.CreateRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\MayhemPackages"));
            var manager = new PackageManager(repository, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages"));
            IQueryable<IPackage> packages = repository.GetPackages();

            List<IPackage> packagesList = packages.ToList();
            foreach (IPackage package in packagesList)
            {
                manager.LocalRepository.GetPackages();

                Logger.WriteLine(package.GetFullName());

                //var existingPackage = manager.LocalRepository.FindPackage(package.Id);
                //if (existingPackage == null)
                //{
                    manager.InstallPackage(package, false);
                //}
                //else if (package.Version > existingPackage.Version)
                //{
                //    manager.UpdatePackage(package, false);
                //}
            }
            */

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
                if (!setDirectories.Contains(fi.DirectoryName))
                {
                    setDirectories.Add(fi.DirectoryName);
                    //set the dll path so it can find the dlls
                    SetDllDirectory(fi.DirectoryName);
                }
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    dependencies.Add(assembly.FullName, assembly);
                }
                catch { }
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            if (wantsUpdates)
            {
                Process.Start("MayhemWpf.exe", "-installupdates");
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
                var repository = PackageRepositoryFactory.Default.CreateRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\MayhemPackages"));
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
