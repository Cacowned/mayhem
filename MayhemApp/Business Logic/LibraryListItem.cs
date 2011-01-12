using System.Windows.Media.Imaging;

namespace MayhemApp
{
    public class LibraryListItem
    {
        public BitmapImage CategoryImage { get; set; }
        public BitmapImage NavImage { get; set; }
        public string Label { get; set; }

        public LibraryListItem() { }

        public LibraryListItem(string label, BitmapImage cat_img, BitmapImage nav_img)
        {
            Label = label;
            CategoryImage = cat_img;
            NavImage = nav_img;
        }

    }
}
