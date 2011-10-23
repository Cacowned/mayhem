using System.Drawing;
using System.Drawing.Drawing2D;

namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// Collect static image processing routines in managed code used by vision modules / config screens here
    /// </summary>
    public class ImageProcessing
    {
        /// <summary>
        /// Scale input image to fixed size in pixels. 
        /// </summary>
        /// <param name="imgPhoto">input image</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <returns>scaled Bitmap image</returns>
        public static Bitmap ScaleWithFixedSize(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float percent;
            float percentW;
            float percentH;

            percentW = (float)width / (float)sourceWidth;
            percentH = (float)height / (float)sourceHeight;
            if (percentH < percentW)
            {
                percent = percentH;
                destX = System.Convert.ToInt16((width -
                              (sourceWidth * percent)) / 2);
            }
            else
            {
                percent = percentW;
                destY = System.Convert.ToInt16((height -
                              (sourceHeight * percent)) / 2);
            }

            int destWidth = (int)(sourceWidth * percent);
            int destHeight = (int)(sourceHeight * percent);

            Bitmap photo = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            try
            {
                photo.SetResolution(imgPhoto.HorizontalResolution,
                                 imgPhoto.VerticalResolution);

                Graphics graphicsPhoto = Graphics.FromImage(photo);
                graphicsPhoto.Clear(Color.Red);
                graphicsPhoto.InterpolationMode =
                        InterpolationMode.HighQualityBicubic;

                graphicsPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
                graphicsPhoto.Dispose();
            }
            catch
            {
                photo.Dispose();
            }

            // bmPhoto is not disposed, as it is returned. 
            return photo;
        }
    }
}
