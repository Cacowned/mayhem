using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MayhemCore;

namespace Mayhem.UserControls
{
    internal class SingleAnalysisStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is ErrorType))
                throw new NotImplementedException("SingleAnalysisStatusConverter can only convert from SingleAnalysisStatus");

            string path = null;
            switch ((ErrorType)value)
            {
                case ErrorType.Failure:
                    path = "Images/error.png";
                    break;
                case ErrorType.Message:
                    path = "Images/message.png";
                    break;
                case ErrorType.Warning:
                    path = "Images/warning.png";
                    break;
                default:
                    throw new NotSupportedException();
            }

            return new BitmapImage(new Uri("/Mayhem;component/" + path, UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
