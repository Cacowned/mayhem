using System;
using System.Windows;
using NuGet;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Deployment.Application;

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
            if (!UriParser.IsKnownScheme("pack"))
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);

            ResourceDictionary dict = new ResourceDictionary();
            Uri uri = new Uri("/MayhemDefaultStyles;component/Styles.xaml", UriKind.Relative);
            dict.Source = uri;
            Application.Current.Resources.MergedDictionaries.Add(dict);

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

                string installPath = "";
                // if we are running as a clickonce application
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    installPath = ApplicationDeployment.CurrentDeployment.DataDirectory;
                }
                else
                {
                    // Modules path
                    //installPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    //installPath = AppDomain.CurrentDomain.BaseDirectory;

                    installPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");
                    //installPath = AppDomain.CurrentDomain.BaseDirectory;
                }
                
                // Create a repository pointing to the local feed
                var repository = new DataServicePackageRepository(new Uri("http://makemayhem.com.cloudsites.gearhost.com/nuget/"));

                repository.ProgressAvailable += new EventHandler<ProgressEventArgs>(repository_ProgressAvailable);

                // Create a package manager to install and resolve dependencies
                var packageManager = new PackageManager(repository, installPath);

                packageManager.Logger = new Logger();

                // Install the package
                packageManager.InstallPackage(Package, ignoreDependencies: false);

                Progress.Value = 100;
                Progress.Dispatcher.Invoke(EmptyDelagate, DispatcherPriority.Render);
                Thread.Sleep(200);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                // Fail
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);

                DialogResult = false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private Action EmptyDelagate = delegate() {};

        private void repository_ProgressAvailable(object sender, ProgressEventArgs e)
        {
            Progress.Value = e.PercentComplete;
            Progress.Dispatcher.Invoke(EmptyDelagate, DispatcherPriority.Render);
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
