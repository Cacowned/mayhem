using System.Collections;
using System.Windows;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for EventList.xaml
    /// </summary>
    public partial class ModuleList : Window
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ModuleList), new UIPropertyMetadata(string.Empty));


        public ModuleList(IEnumerable list, string headerText)
        {
            Text = headerText;
            InitializeComponent();

            ModulesList.ItemsSource = list;
        }

        private void ChooseButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
