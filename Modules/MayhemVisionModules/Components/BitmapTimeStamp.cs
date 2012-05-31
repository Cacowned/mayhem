using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MayhemVisionModules.Components
{
    /// <summary> 
    /// Container class that attaches a Timestamp to standard Bitmaps, 
    /// to have some additional metadata for Modules to use
    /// </summary>
    public sealed class BitmapTimeStamp : IDisposable, ICloneable
    {
        public DateTime TimeStamp
        {
            get;
            set;
        }

        public Bitmap Image
        {
            get;
            set;
        }

        public BitmapTimeStamp(Bitmap i) :
            this(i, DateTime.Now)
        {
        }

        public BitmapTimeStamp(Bitmap i, DateTime ts)
        {
            Image = i;
            TimeStamp = ts;
        }

        /// <summary>
        /// Explicitly dispose of the Bitmap, as they tend to stick around and mess up the memory in .net
        /// </summary>
        ~BitmapTimeStamp()
        {
            if (Image != null)
                Image.Dispose();
        }

        public void Dispose()
        {
            if (Image != null)
                Image.Dispose();
            //GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new BitmapTimeStamp(new Bitmap(Image), TimeStamp);
        }
    }
}
