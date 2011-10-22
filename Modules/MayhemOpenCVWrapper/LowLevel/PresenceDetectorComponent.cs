using System;
using System.Drawing;
using System.Drawing.Imaging;
using MayhemCore;

namespace MayhemOpenCVWrapper.LowLevel
{
    /// <summary>
    /// Presence detector component
    /// Low level component interfacing with the OpenCVDLL presence detector component. 
    /// </summary>
    public class PresenceDetectorComponent : CameraImageListener, IDisposable
    {
        /// <summary>
        /// Returns the value of the default sensitivity set in the presence detector dll. 
        /// </summary>
        public static double DefaultSensitivity
        {
            get
            {
                return OpenCVDLL.PresenceDetector.DEFAULT_SENSITIVITY;
            }
        }

        private OpenCVDLL.PresenceDetector pd;

        public delegate void DetectionHandler(object sender, DetectionEventArgs e);

        public event DetectionHandler OnPresenceUpdate;

        /// <summary>
        /// Sets the decay sensitivity of the presence detector
        /// Basically this determines how long the presence is still valid when no movement has
        /// been detected. 
        /// </summary>
        public double Sensitivity
        {
            get
            {
                if (pd != null)
                {
                    return pd.Sensitivity;
                }

                return 0;
            }
            set
            {
                if (pd != null)
                {
                    pd.Sensitivity = value;
                }
            }
        }

        private bool presence = false;

        /// <summary>
        /// Returns true if presence has been detected in the last frame.
        /// </summary>
        public bool Presence
        {
            get { return presence; }
        }

        public PresenceDetectorComponent(int width, int height)
        {
            pd = new OpenCVDLL.PresenceDetector(width, height);
        }

        ~PresenceDetectorComponent()
        {
            Dispose(false);
            return;
        }

        /// <summary>
        /// Sends a new frame to the presence detector C++ implementation. 
        /// </summary>
        public override void UpdateFrame(object sender, EventArgs e)
        {
            Camera camera = sender as Camera;

            // copy over image data to the DLL 
            try
            {
                Bitmap cameraImage = camera.ImageAsBitmap();
                BitmapData bd = cameraImage.LockBits(new Rectangle(0, 0, cameraImage.Size.Width, cameraImage.Size.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, cameraImage.PixelFormat);
                IntPtr imgPointer = bd.Scan0;

                // transmit frame
                unsafe
                {
                    pd.ProcessFrame((byte*)imgPointer);
                }

                cameraImage.UnlockBits(bd);
                cameraImage.Dispose();
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Frame Grab Exception!\n" + ex);
            }

            // find out what the results are
            presence = pd.GetCurrentPresence();

            // TODO.....
            Point[] points = null;

            if (OnPresenceUpdate != null && pd.IsInitialized)
            {
                OnPresenceUpdate(this, new DetectionEventArgs(points));
            }
        }

        protected virtual void Dispose(bool dispose)
        {
            if (!dispose)
            {
                pd.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
