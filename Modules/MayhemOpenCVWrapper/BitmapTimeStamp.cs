using System;
using System.Drawing;

namespace MayhemOpenCVWrapper
{
    /// <summary> 
    /// Container class that attaches a Timestamp to standard Bitmaps, 
    /// to have some additional metadata for Modules to use
    /// </summary>
    public sealed class BitmapTimestamp : IDisposable, ICloneable
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

        public BitmapTimestamp(Bitmap i) :
            this(i, DateTime.Now)
        {
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
            if (Image != null)
                Image.Dispose();
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new BitmapTimestamp(new Bitmap(Image), TimeStamp);
        }
    }
}
