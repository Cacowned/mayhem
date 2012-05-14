using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Parking space for static functions common to the vision modules
    /// GDI DeleteObject function used to cleanup hPtrs after painting Bitmaps to WPF Canvases
    /// </summary>
    public static class VisionModulesWpfCommon
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Wrapper with a more meaningful name for DeleteObject
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        public static bool DeleteGDIObject(IntPtr hObject)
        {
            return DeleteObject(hObject);
        }

        /// <summary>
        /// Convert Bitmap to byte array
        /// </summary>
        /// <param name="b">The Bitmap</param>
        /// <returns>array of bytes</returns>
        public static byte[] BitmapToArray(Bitmap b)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, b);
            byte[] bytes = ms.ToArray();
            return bytes;
        }

        /// <summary>
        /// Converts a byte array with stored image data to Bitmap
        /// </summary>
        /// <param name="bytes">Image data bytes</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ArrayToBitmap(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bytes);

            // binary formatter should not be required
            Bitmap b = (Bitmap)bf.Deserialize(ms);
            return b;
        }
    }
}
