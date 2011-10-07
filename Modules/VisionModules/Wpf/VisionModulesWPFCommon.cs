/*  VisionModulesWPFCommon.cs
 * 
 * Parking space for static functions common to the vision modules
 * 
 * (c) 2011 Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VisionModules.Wpf
{
    /// <summary>
    /// GDI DeleteObject function used to cleanup hPtrs after painting Bitmaps to WPF Canvases
    /// </summary>
    public static class VisionModulesWPFCommon
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteGDIObject(IntPtr hObject);

        /// <summary>
        /// Convert Bitmap to byte array
        /// </summary>
        /// <param name="b">The Bitmap</param>
        /// <returns>array of bytes</returns>
        public static Byte[] BitmapToArray(Bitmap b)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, b);
            Byte[] bytes =  ms.ToArray();
            return bytes;
        }

        /// <summary>
        /// Converts a byte array with stored image data to Bitmap
        /// </summary>
        /// <param name="bytes">Image data bytes</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ArrayToBitmap(Byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bytes);
            // binary formatter should not be required
            Bitmap b = (Bitmap) bf.Deserialize(ms); 
            return b;
        }

    }

}
