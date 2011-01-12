using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;
using MayhemOpenCVWrapper;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using MessageBox = System.Windows.MessageBox;
using System.Drawing;
using System.Runtime.Serialization;



namespace MayhemApp.Business_Logic
{
    /**<summary>
     * This class takes a snapshot upon receiving an event
     * </summary>
     * **/
    [Serializable]
    class MayhemSnapshotAction : MayhemAction, ISerializable
    {
        public const string TAG = "[MayhemSnapshotAction] : ";

        private MayhemImageUpdater i = MayhemImageUpdater.Instance;

        private MayhemImageUpdater.ImageUpdateHandler image_updater; 

        // path to save image files to
        private string file_path = null;

        private MayhemTrigger sending_trigger = null;

        public override void OnDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(TAG+ "doubleClickEvent");
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.Description = "Please select the location to store the snapshots.";
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
           
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                file_path = dialog.SelectedPath;
                Debug.WriteLine(TAG + "Selected Path " + file_path); 
            }
               
        }

        /**<summary>
         * 
         * Makes a base-compatible constructor.
         * Ignores the input string!
         * </summary>
         **/
        public MayhemSnapshotAction(string s) : this() { }

        public MayhemSnapshotAction()
            : base("Take Snapshot",
                   "Takes a snapshot and saves it",
                   "When triggered, this Action takes a snapshot with the webcam and saves the image to disk. Double click to specify the folder in which the images will be saved")
        {
            image_updater = new MayhemImageUpdater.ImageUpdateHandler(i_OnImageUpdated); 
        }

        public override void PerformAction(MayhemTrigger sender)
        {
            //Debug.WriteLine("MayhemDebugAction " + ID + " got triggered!");

          // MessageBox.Show("Snapshot Got Triggered!");

            if (i.running == false)
            {
                i.StartFrameGrabbing();
            }

            i.OnImageUpdated += image_updater;
            sending_trigger = sender;
        }


        // string next_filename


        /**<summary>
         * Gets latest image from OpenCV and saves it to disk. 
         * </summary>
         * **/
        private void i_OnImageUpdated(object sender, EventArgs e)
        {

            // avoid capturing any more images
            i.OnImageUpdated -= image_updater;
            // access image buffer

            Bitmap BackBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 320, 240);

            // get at the bitmap data in a nicer way
            System.Drawing.Imaging.BitmapData bmpData =
                BackBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                BackBuffer.PixelFormat);

            int bufSize = i.bufSize;
            IntPtr ImgPtr = bmpData.Scan0;

            // grab the image

            lock (i.thread_locker)
            {
                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(i.imageBuffer, 0, ImgPtr, bufSize);
            }
            // Unlock the bits.
            BackBuffer.UnlockBits(bmpData);
            DateTime now = DateTime.Now;
            // TODO think of a better naming convention
            string filename = "Mayhem-Snapshot_"+sending_trigger.description+"_"+now.Year+now.Month+now.Month+now.Day+"_"+now.Hour+now.Minute+now.Second+".jpg";
            string path = this.file_path +"\\"+filename;
            Debug.WriteLine(TAG + "saving file to " + path);
            BackBuffer.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);

        }

        #region MayhemSnapshotAction Serialization

        public MayhemSnapshotAction(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //throw new NotImplementedException();
            image_updater = new MayhemImageUpdater.ImageUpdateHandler(i_OnImageUpdated);
            file_path = info.GetString("FilePath");
        }

        /**<summary>
         * Serialization Pickler
         * The only thing that gets saved for this Action is the path the files get saved to. 
         *</summary>
         */ 
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FilePath", file_path);
        }

        #endregion
    }
}
