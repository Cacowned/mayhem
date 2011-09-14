using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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
    }
}
