/*
 * IBufferingImager.cs
 * 
 * Ongoing work to abstract away from the camera as only imaging source, and to unify image providers within Mayhem. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MayhemOpenCVWrapper
{
    public interface IBufferingImager
    {
        /// <summary>
        /// Gets the bitmap data at a certain bufer index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
         Bitmap GetBufferItemAtIndex(int index);

        /// <summary>
        /// Gets timestamp at certain buffer index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
         DateTime GetBufferTimeStampAtIndex(int index); 
    }
}
