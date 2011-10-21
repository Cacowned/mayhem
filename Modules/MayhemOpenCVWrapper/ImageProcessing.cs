/*
 * ImageProcessing.cs
 * 
 * Collect static image processing routines in managed code used by vision modules / config screens here
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

using System.Drawing;
using System.Drawing.Drawing2D;
using System;

namespace MayhemOpenCVWrapper
{
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

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            try
            {
                bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                 imgPhoto.VerticalResolution);

                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.Clear(System.Drawing.Color.Red);
                grPhoto.InterpolationMode =
                        InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                    new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
                grPhoto.Dispose();
            }
            catch (Exception ex)
            {
                bmPhoto.Dispose();
            }
            
           

            
            // bmPhoto is not disposed, as it is returned. 
            return  bmPhoto;
        }
    }
}
