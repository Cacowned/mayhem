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
        public DateTime timeStamp = DateTime.Now;
        public Bitmap image;

        public BitmapTimestamp(Bitmap i)
        {
            image = i;
        }

        public BitmapTimestamp(Bitmap i, DateTime ts)
        {
            image = i;
            timeStamp = ts; 
        }
      
        /// <summary>
        /// Explicitly dispose of the Bitmap, as they tend to stick around and mess up the memory in .net
        /// </summary>
        ~BitmapTimestamp()
        {
            if (image != null)
                image.Dispose();
        }

        public void Dispose()
        {
            image.Dispose();
            image = null; 
        }

        public object Clone()
        {
            Bitmap image_copy = new Bitmap(image);
            BitmapTimestamp obj_clone = new BitmapTimestamp(image_copy, this.timeStamp);
            return obj_clone; 
        }
    }
}
