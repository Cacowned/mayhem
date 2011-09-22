using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MayhemCore;

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
