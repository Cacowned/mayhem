using System;
using System.Windows;
using NuGet;
using System.IO;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for InstallModule.xaml
    /// </summary>
    public partial class InstallModule : Window
    {



        public IPackage Package
        {
            get { return (IPackage)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(IPackage), typeof(InstallModule), new UIPropertyMetadata(null));


        public InstallModule(IPackage package)
        {
            this.Package = package;

            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ModuleName.Text = Package.Id;
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the package file

                // Modules path
                string installPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");



                // Create a repository pointing to the local feed
                var repository = new DataServicePackageRepository(new Uri("http://localhost:58108/nuget/"));

                //repository.ProgressAvailable += new EventHandler<ProgressEventArgs>(repository_ProgressAvailable);

                // Create a package manager to install and resolve dependencies
                var packageManager = new PackageManager(repository, installPath);

                packageManager.Logger = new Logger();

                // Install the package
                packageManager.InstallPackage(Package, ignoreDependencies: false);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                // Fail
                Console.WriteLine(ex.Message);

                DialogResult = false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        void repository_ProgressAvailable(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("Progress!! {0}%", e.PercentComplete);
        }

        public class Logger : ILogger
        {

            public void Log(MessageLevel level, string message, params object[] args)
            {
                if (level == MessageLevel.Info)
                {
                    Console.WriteLine(message, args);
                }
            }
        }
    }

}
