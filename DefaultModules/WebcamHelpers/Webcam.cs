using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Drawing;
using System.Windows.Interop;
using System.IO;
using System.Windows.Media.Imaging;

namespace DefaultModules.WebcamHelpers
{
    public class Webcam
    {
        public static int Width { get; set; }
        public static int Height { get; set; }


        public int CaptureDevice { get; set; }


        protected IDataObject tempObj;
        protected System.Drawing.Image tempImg;

        public bool Running { get; set; }

        #region DLL Imports and Constants

        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        public static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);

        [DllImport("user32", EntryPoint = "OpenClipboard")]
        public static extern int OpenClipboard(int hWnd);

        [DllImport("user32", EntryPoint = "EmptyClipboard")]
        public static extern int EmptyClipboard();

        [DllImport("user32", EntryPoint = "CloseClipboard")]
        public static extern int CloseClipboard();

        public const int WM_USER = 1024;

        public const int WM_CAP_CONNECT = 1034;
        public const int WM_CAP_DISCONNECT = 1035;
        public const int WM_CAP_GET_FRAME = 1084;
        public const int WM_CAP_COPY = 1054;

        public const int WM_CAP_START = WM_USER;

        public const int WM_CAP_DLG_VIDEOFORMAT = WM_CAP_START + 41;
        public const int WM_CAP_DLG_VIDEOSOURCE = WM_CAP_START + 42;
        public const int WM_CAP_DLG_VIDEODISPLAY = WM_CAP_START + 43;
        public const int WM_CAP_GET_VIDEOFORMAT = WM_CAP_START + 44;
        public const int WM_CAP_SET_VIDEOFORMAT = WM_CAP_START + 45;
        public const int WM_CAP_DLG_VIDEOCOMPRESSION = WM_CAP_START + 46;
        public const int WM_CAP_SET_PREVIEW = WM_CAP_START + 50;

        #endregion

        protected static Webcam _instance;

        public static Webcam GetInstance() {
            if (_instance == null) {
                _instance = new Webcam();
            }

            return _instance;
        }

        private Webcam() {
            Width = 640;
            Height = 480;

            Running = false;
        }

        ~Webcam() {
            Stop();
        }

        public void Start() {
            if (Running)
                return;

            try {
                Stop();

                // setup a capture window
                CaptureDevice = capCreateCaptureWindowA(lpszWindowName: "WebCap",
                                                    dwStyle: 0,
                                                    X: 0,
                                                    Y: 0,
                                                    nWidth: Width,
                                                    nHeight: Height,
                                                    hwndParent: 0,
                                                    nID: 0);

                // Launch the choose camera dialog
                //Webcam.SendMessage(captureDevice, Webcam.WM_CAP_CONNECT, 0, 0);


                // Launch the choose camera dialog
                SendMessage(CaptureDevice, WM_CAP_CONNECT, 0, 0);

                // connect to the capture device
                SendMessage(CaptureDevice, WM_CAP_SET_PREVIEW, 0, 0);

                // set the frame number
                //m_FrameNumber = FrameNum;
                Running = true;

            } catch (Exception excep) {
                MessageBox.Show("An error ocurred while starting the video capture. Check that your webcamera is connected properly and turned on.\r\n\n" + excep.Message);
                Stop();
            }
        }

        public void Stop() {
            if (!Running)
                return;

            SendMessage(CaptureDevice, WM_CAP_DISCONNECT, 0, 0);
            Running = false;

        }

        public Bitmap GetFrame() {
            if (!Running)
                return null;

            // get the next frame;
            SendMessage(CaptureDevice, WM_CAP_GET_FRAME, 0, 0);

            // copy the frame to the clipboard
            SendMessage(CaptureDevice, WM_CAP_COPY, 0, 0);

            // paste the frame into the event args image
            
            // get from the clipboard
            //Clipboard.GetImage();
            //tempObj = Clipboard.GetDataObject();
            //tempImg = tempObj.GetData(System.Windows.Forms.DataFormats.Bitmap) as Bitmap;
            return BitmapFromSource(Clipboard.GetImage());
            //tempImg = (Image)BitmapFromSource(Clipboard.GetImage());

            
            /*
            * For some reason, the API is not resizing the video
            * feed to the width and height provided when the video
            * feed was started, so we must resize the image here
            
            tempImg
            x.WebCamImage = tempImg.GetThumbnailImage(m_Width, m_Height, null, System.IntPtr.Zero);
            */

            //return tempImg;

        }

        private System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource) {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            { 
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            } 
            return bitmap;
        }
    }
}
