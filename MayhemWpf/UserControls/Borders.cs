using System.Windows;
using System.Windows.Controls;

namespace MayhemWpf.UserControls
{

    public class Borders : ContentControl
    {
        static Borders() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Borders), new FrameworkPropertyMetadata(typeof(Borders)));
        }
    }
}
