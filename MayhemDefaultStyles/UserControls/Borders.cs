using System.Windows;
using System.Windows.Controls;

namespace MayhemDefaultStyles.UserControls
{

    public class Borders : ContentControl
    {
        static Borders() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Borders), new FrameworkPropertyMetadata(typeof(Borders)));
        }
    }
}
