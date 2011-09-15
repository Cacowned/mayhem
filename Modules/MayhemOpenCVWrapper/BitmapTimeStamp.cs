using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    public class BitmapTimestamp
    {
        public DateTime timeStamp = DateTime.Now;
        public Bitmap image;

        public BitmapTimestamp(Bitmap i)
        {
            image = i;
        }
 
        /// <summary>
        /// Explicitly dispose of the Bitmap, as they tend to stick around and mess up the memory in .net
        /// </summary>
        ~BitmapTimestamp()
        {
            image.Dispose();  
        }
    }
}
