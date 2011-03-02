using System.Windows;
using System.Windows.Controls;

namespace MayhemDefaultStyles.UserControls
{
	/// <summary>
	/// Interaction logic for WindowHeader.xaml
	/// </summary>
	public partial class WindowHeader : UserControl
	{


		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(WindowHeader), new UIPropertyMetadata(string.Empty));

		public WindowHeader() {
			InitializeComponent();
		}
	}
}
