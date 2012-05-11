using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MayhemVisionModules.Components
{
    /// <summary>
    /// This interface allows the retrieval of buffer items from image providers implementing it 
    /// </summary>
    interface IBufferingImager
    {
        /// <summary>
        /// Gets the bitmap data at a certain bufer index
        /// </summary>
        /// <param name="index">index of buffer object</param>
        /// <returns>Bitmap of object at buffer index</returns>
        Bitmap GetBufferItemAtIndex(int index);

        /// <summary>
        /// Gets timestamp at certain buffer index
        /// </summary>
        /// <param name="index">index of buffer object</param>
        /// <returns>timestamp of object at buffer index</returns>
        DateTime GetBufferTimeStampAtIndex(int index);
    }
}
