/*
 * BitmapTimestamp.cs
 * 
 * Container class that attaches a Timestamp to standard Bitmaps, 
 * to have some additional metadata for Modules to use
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Drawing;

namespace MayhemOpenCVWrapper
{
    public class BitmapTimestamp : IDisposable, ICloneable
    {
        public DateTime TimeStamp = DateTime.Now;
        public Bitmap Image;

        public BitmapTimestamp(Bitmap i)
        {
            Image = i;
        }

        public BitmapTimestamp(Bitmap i, DateTime ts)
        {
            Image = i;
            TimeStamp = ts; 
        }
      
        /// <summary>
        /// Explicitly dispose of the Bitmap, as they tend to stick around and mess up the memory in .net
        /// </summary>
        ~BitmapTimestamp()
        {
            if (Image != null)
                Image.Dispose();
        }

        public void Dispose()
        {
            Image.Dispose();
            Image = null; 
        }

        public object Clone()
        {
            Bitmap imageCopy = new Bitmap(Image);
            BitmapTimestamp objClone = new BitmapTimestamp(imageCopy, this.TimeStamp);
            return objClone; 
        }
    }
}
