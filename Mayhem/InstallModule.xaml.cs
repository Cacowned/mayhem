using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NuGet;

namespace Mayhem
{
    /// <summary>
    /// Interaction logic for InstallModule.xaml
    /// </summary>
    public partial class InstallModule : Window
    {
        public IPackage Package
        {
            get;
            set;
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(IPackage), typeof(InstallModule), new UIPropertyMetadata(null));

        public InstallModule(IPackage package)
        {
            if (!UriParser.IsKnownScheme("pack"))
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);

            ResourceDictionary dict = new ResourceDictionary();
            Uri uri = new Uri("/MayhemWpf;component/Styles.xaml", UriKind.Relative);
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
                    installPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");
                }
                
                // Create a repository pointing to the feed
                IPackageRepository repository;
                if (Environment.GetCommandLineArgs().Contains("-localrepo"))
                    repository = new LocalPackageRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\MayhemPackages\\Packages"));
                else
                {
                    repository = new DataServicePackageRepository(new Uri("http://makemayhem.com.cloudsites.gearhost.com/nuget/"));
                    ((DataServicePackageRepository)repository).ProgressAvailable += Repository_ProgressAvailable;
                }

                // Create a package manager to install and resolve dependencies
                var packageManager = new PackageManager(repository, installPath);
                packageManager.Logger = new Logger(this);

                // Install the package
                ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
                    {
                        packageManager.InstallPackage(Package, ignoreDependencies: false);
                        Dispatcher.Invoke((Action)delegate
                        {
                            stackPanelDefaultButtons.Visibility = Visibility.Hidden;
                            buttonClose.Visibility = Visibility.Visible;

                            Progress.Value = 100;
                            Progress.Dispatcher.Invoke(EmptyDelegate, DispatcherPriority.Render);
                        });
                    }));
            }
            catch (Exception ex)
            {
                // TODO: This should be a better message
                MessageBox.Show(ex.Message);
                Debug.WriteLine(ex.Message);

                DialogResult = false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private Action EmptyDelegate = delegate {};

        private void Repository_ProgressAvailable(object sender, ProgressEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate
                {
                    Progress.Value = e.PercentComplete;
                });
            Dispatcher.Invoke(EmptyDelegate, DispatcherPriority.Render);
        }

        public class Logger : ILogger
        {
            InstallModule parent;
            public Logger(InstallModule parent)
            {
                this.parent = parent;
            }
            public void Log(MessageLevel level, string message, params object[] args)
            {
                Debug.WriteLine(message, args);
                parent.Dispatcher.Invoke((Action)delegate
                    {
                        parent.listBox1.Items.Add(string.Format(message, args));
                        parent.listBox1.SelectedItem = parent.listBox1.Items.GetItemAt(parent.listBox1.Items.Count - 1);
                        parent.listBox1.ScrollIntoView(parent.listBox1.SelectedItem);
                    });
            }
        }
    }

}
