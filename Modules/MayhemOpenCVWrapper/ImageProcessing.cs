using System;
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

            float percent = 0;
            float percentW = 0;
            float percentH = 0;

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

                Graphics grPhoto = Graphics.FromImage(photo);
                grPhoto.Clear(System.Drawing.Color.Red);
                grPhoto.InterpolationMode =
                        InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                    new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
                grPhoto.Dispose();
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
