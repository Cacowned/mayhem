﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemOpenCVWrapper;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Runtime.Serialization;

namespace MayhemApp.Business_Logic
{
    /**<summary>
     * This class implements a motion detection trigger for Mayhem
     * It builds upon the Mayhem OpenCV Motion Detector
     * </summary>
     **/
    [Serializable]
    public class MayhemMotionDetectionTrigger : MayhemTrigger, ISerializable
    {


        private const string TAG = "[MayhemMotiondDetectionTrigger] : ";

        private DateTime lastMotionDetected = DateTime.Now;

        private const int detectionInterval = 5000; //ms

        private MotionDetector m;
        private MotionDetectionSetupWindow setupWindow;

        //public delegate void triggerActivateHandler(object sender, EventArgs e);
        public override event triggerActivateHandler onTriggerActivated;
        private MotionDetector.MotionUpdateHandler motionUpdateHandler;

        Rect boundingRect = new Rect();

        /**<summary>
         * A Base-Compatible Constructor
         * Ignores the input string! 
         * </summary>
         * **/
        public MayhemMotionDetectionTrigger(string s)
            : this() { }
       


        public MayhemMotionDetectionTrigger()
            : base("Motion Detector",
                   "Fires when motion is detected",
                   "Use your webcam as a motion detector. Double click to define the detection area.")
        {
            m = new MotionDetector(320, 240);
            motionUpdateHandler = new MotionDetector.MotionUpdateHandler(m_OnMotionUpdate);
            setupWindow = new MotionDetectionSetupWindow();
            setupWindow.OnSetButtonClicked += new MotionDetectionSetupWindow.SetButtonClickedHandler(setupWindow_OnSetButtonClicked);
        }

        void m_OnMotionUpdate(object sender, List<System.Drawing.Point> points)
        {
            
            TimeSpan ts = DateTime.Now - lastMotionDetected;

            if (ts.TotalMilliseconds > detectionInterval)
            {

                Debug.WriteLine(TAG + "m_OnMotionUpdate");
                if (onTriggerActivated != null)
                {
                    onTriggerActivated(this, new EventArgs());
                }

                lastMotionDetected = DateTime.Now;
            }
        }

        void setupWindow_OnSetButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            // todo: package the rect in the eventArgs
            Debug.WriteLine("[MayhemMotionDetectionTrigger] : setupWindow_OnSetButtonClicked");
            Rect bRect = setupWindow.GetBoundingRect();
            m.SetMotionBoundaryRect(bRect);
            boundingRect = bRect;
        }

        public override void OnDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("[MayhemMotionDetectionTrigger] : doubleClickEvent");
            MayhemImageUpdater i = MayhemImageUpdater.Instance;
            if (!i.running)
                i.StartFrameGrabbing();
            DimMainWindow(true);
            setupWindow.ShowDialog();
            DimMainWindow(false);
        }

        /** <summary>
         *  Starts the trigger: first checks if the image capture device is delivering images
         *      if not, it gets turned on. 
         * </summary>
         **/

        public override void EnableTrigger()
        {
            Debug.WriteLine(TAG + "EnableTrigger");
            
            MayhemImageUpdater i = MayhemImageUpdater.Instance;
            if (!i.running)
                i.StartFrameGrabbing();

            // register the trigger's motion update handler
            m.RegisterForImages(MayhemImageUpdater.Instance);
            m.OnMotionUpdate += motionUpdateHandler;
            base.EnableTrigger();
        }

        public override void DisableTrigger()
        {
            Debug.WriteLine(TAG + "DisableTrigger");
            // de-register the trigger's motion update handler
            m.OnMotionUpdate -= motionUpdateHandler;
            base.DisableTrigger();
        }


        ~MayhemMotionDetectionTrigger()
        {
            m.OnMotionUpdate -= motionUpdateHandler;
        }



        #region MayhemMotionDetectionTrigger Serialization


        public MayhemMotionDetectionTrigger(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            boundingRect = (Rect)info.GetValue("BoundingRectSelected", typeof(Rect));
            System.Windows.Point boundingRectOrigin = (System.Windows.Point)info.GetValue("DisplayBoundingrectOrigin", typeof(System.Windows.Point));
            System.Windows.Point boundingRectSize = (System.Windows.Point)info.GetValue("DisplayBoundingrectSize", typeof(System.Windows.Point));


            setup_window = new MotionDetectionSetupWindow();

            if (boundingRectSize.X > 0 && boundingRectSize.Y > 0)
            {
                ((MotionDetectionSetupWindow)setup_window).SetSelectionDisplayRect(boundingRectOrigin, boundingRectSize);
            }

            m = new MotionDetector(320, 240);
            m.SetMotionBoundaryRect(boundingRect);

            motionUpdateHandler = new MotionDetector.MotionUpdateHandler(m_OnMotionUpdate);
            setupWindow.OnSetButtonClicked += new MotionDetectionSetupWindow.SetButtonClickedHandler(setupWindow_OnSetButtonClicked);

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
           // throw new NotImplementedException();

            base.GetObjectData(info, context);
            info.AddValue("BoundingRectSelected", boundingRect);
            info.AddValue("DisplayBoundingrectOrigin", ((MotionDetectionSetupWindow)setup_window).boundingRectOrigin);
            info.AddValue("DisplayBoundingrectSize", ((MotionDetectionSetupWindow)setup_window).boundingRectSize);

        }

        #endregion
    }

}
