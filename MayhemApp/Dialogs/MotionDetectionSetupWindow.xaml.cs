using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;


namespace MayhemApp
{
    /// <summary>
    /// Interaction logic for MotionDetectionSetupWindow.xaml
    /// </summary>
    /// 



    public partial class MotionDetectionSetupWindow : Window
    {

        private MayhemImageUpdater i = MayhemImageUpdater.Instance;
        private  MayhemImageUpdater.ImageUpdateHandler imageUpdateHandler;
        private delegate void SetImage1Source ();
        private bool startDrag = false;
        private System.Windows.Point prev_position = new System.Windows.Point(-1.0,-1.0);

        public System.Windows.Point boundingRectOrigin = new System.Windows.Point(0,0);
        public System.Windows.Point boundingRectSize = new System.Windows.Point(0,0);


        private System.Windows.Shapes.Rectangle motionBoundingRect = new System.Windows.Shapes.Rectangle();

       

        

        // the MotionDetectionTrigger listens to this event in order to set the motion detection window
        public delegate void SetButtonClickedHandler(object sender, RoutedEventArgs e);
        public event SetButtonClickedHandler OnSetButtonClicked;


        private enum modes
        {
            start,
            selecting, 
            selected
        }

        private modes mode = modes.start;

        public MotionDetectionSetupWindow()
        {
            InitializeComponent();



           // image1 = new System.Windows.Controls.Image();

            imageUpdateHandler = new MayhemImageUpdater.ImageUpdateHandler(i_OnImageUpdated);


            motionBoundingRect.Stroke = System.Windows.Media.Brushes.OrangeRed;
            motionBoundingRect.StrokeThickness = 2;

            //overlay.Children.Add(motionBoundingRect);

            // todo: danger here

            if (i.running == false)
            {
                i.StartFrameGrabbing();
            }

        }


        public void SetupWindowCloseButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                i.OnImageUpdated += imageUpdateHandler;
            }
            else
            {
                i.OnImageUpdated -= imageUpdateHandler;
            }
        }


        private void SetImage1Source_()
        {
            Debug.WriteLine("[SetImage1Source_] ");

            //int stride = 320 * 3;
          
            

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

            IntPtr hBmp;

            //Convert the bitmap to BitmapSource for use with WPF controls
            hBmp = BackBuffer.GetHbitmap();

            image1.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            image1.Source.Freeze();
            

                
            

        }
        
        public void i_OnImageUpdated(object sender, EventArgs e)
        {

            Dispatcher.Invoke(new SetImage1Source(SetImage1Source_), null);

        }


        

        private void btn_selRegion_Click(object sender, RoutedEventArgs e)
        {
            if (mode == modes.start)
            {
                this.Cursor = Cursors.Cross;
                mode = modes.selecting;
            }

        }


        #region window mouse event

        private void window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            startDrag = false;
        }


        #endregion

        #region mouse events on the overlay canvas


        private void overlay_MouseLeave(object sender, MouseEventArgs e)
        {
            startDrag = false;

            mode = modes.start;
        }

        /**<summary>
         * Draw a selection rectangle. 
         * Used upon deserialization.
         * </summary>
         */ 
        internal void SetSelectionDisplayRect(System.Windows.Point bRectOrigin, System.Windows.Point bRectSize)
        {
            //throw new NotImplementedException();
            mode = modes.selected;
            motionBoundingRect.Stroke = System.Windows.Media.Brushes.OrangeRed;
            motionBoundingRect.StrokeThickness = 2;

            boundingRectOrigin = bRectOrigin;
            boundingRectSize = bRectSize;

            Canvas.SetLeft(motionBoundingRect, boundingRectOrigin.X);
            Canvas.SetTop(motionBoundingRect, boundingRectOrigin.Y);

            motionBoundingRect.Width = boundingRectSize.X;
            motionBoundingRect.Height = boundingRectSize.Y;

            overlay.Children.Add(motionBoundingRect);
            
        }



        private void overlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mode == modes.selecting)
            {
                startDrag = true;
                //overlay.Children.Remove(motionBoundingRect);



                motionBoundingRect.Stroke = System.Windows.Media.Brushes.OrangeRed;
                motionBoundingRect.StrokeThickness = 2;



                System.Windows.Point pos = e.GetPosition(overlay);

                Canvas.SetLeft(motionBoundingRect, pos.X);
                Canvas.SetTop(motionBoundingRect, pos.Y);
                // mBoundingRect = new Rect(pos.X, pos.Y, 0, 0);

                boundingRectOrigin = pos;
                boundingRectSize = new System.Windows.Point(0, 0);
                overlay.Children.Add(motionBoundingRect);
            }

        }

        private void overlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            this.Cursor = Cursors.Arrow;
            startDrag = false;
            mode = modes.start;



        }



        private void overlay_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("[overlay_MouseMove]");
            if (startDrag)
            {
                System.Windows.Point position =  e.GetPosition(overlay);
                
                if (prev_position.X < 0 && prev_position.Y < 0)
                {
                    prev_position = position;
                }

            
              //  motionBoundingRect

                 boundingRectSize.X = position.X - boundingRectOrigin.X;
                 boundingRectSize.Y = position.Y - boundingRectOrigin.Y;
                // x,y > 0
                 if (boundingRectSize.X >= 0 && boundingRectSize.Y >= 0)
                {
                    motionBoundingRect.Width =boundingRectSize.X;
                    motionBoundingRect.Height =boundingRectSize.Y;
                    motionBoundingRect.RenderTransform = new RotateTransform(0);
                }
                // x < 0, y> 0
                 else if (boundingRectSize.X < 0 && boundingRectSize.Y >= 0)
                {
                    // rotate


                     TransformGroup g = new TransformGroup();

                     Transform r = new RotateTransform(180);
                     Transform t =  new TranslateTransform(0, motionBoundingRect.Height);

                    
                     g.Children.Add(r);
                     g.Children.Add(t);

                    motionBoundingRect.Width = Math.Abs(boundingRectSize.X);
                    motionBoundingRect.Height = boundingRectSize.Y;

               
                    motionBoundingRect.RenderTransform = g;            
                }
                // x < 0, y < 0
                 else if (boundingRectSize.X < 0 && boundingRectSize.Y < 0)
                 {
                     TransformGroup g = new TransformGroup();

                     Transform r = new RotateTransform(180);
                     //Transform t = new TranslateTransform(0, motionBoundingRect.Height);


                     g.Children.Add(r);
                     //g.Children.Add(t);

                     motionBoundingRect.Width = Math.Abs(boundingRectSize.X);
                     motionBoundingRect.Height = Math.Abs(boundingRectSize.Y);


                     motionBoundingRect.RenderTransform = g;         
                 }
                 // x>0, y < 0
                 else if (boundingRectSize.X >=0  && boundingRectSize.Y < 0)
                 {
                     TransformGroup g = new TransformGroup();

                     //Transform r = new RotateTransform(180);
                     Transform t = new TranslateTransform( 0, -motionBoundingRect.Height);


                     g.Children.Add(t);
                    // g.Children.Add(t);

                     motionBoundingRect.Width = Math.Abs(boundingRectSize.X);
                     motionBoundingRect.Height = Math.Abs(boundingRectSize.Y);


                     motionBoundingRect.RenderTransform = g;   

                 }

                prev_position = position;


            }

        }



        #endregion

        #region reset and set buttons

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            motionBoundingRect.Width = 0;
            motionBoundingRect.Height = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // set button is clicked

            if (this.OnSetButtonClicked != null)
            {
                OnSetButtonClicked(this, e);
            }

        }
        #endregion

        public Rect GetBoundingRect()
        {

            // TODO use this code in the "overlay_MouseMove" function

            Rect bRect = new Rect();

            if (boundingRectSize.X >= 0 && boundingRectSize.Y >= 0)
            {
                bRect.X = boundingRectOrigin.X;
                bRect.Y = boundingRectOrigin.Y;
                bRect.Width = boundingRectSize.X;
                bRect.Height = boundingRectSize.Y;
            }
            else if (boundingRectSize.X < 0 && boundingRectSize.Y >= 0)
            {
                bRect.X = boundingRectOrigin.X + boundingRectSize.X;
                bRect.Y = bRect.Y = boundingRectOrigin.Y;
                bRect.Width = Math.Abs(boundingRectSize.X);
                bRect.Height = boundingRectSize.Y;

            }
            else if (boundingRectSize.X >= 0 && boundingRectSize.Y < 0)
            {
                bRect.X = boundingRectOrigin.X;
                bRect.Y = boundingRectOrigin.Y + boundingRectSize.Y;
                bRect.Width = boundingRectSize.X;
                bRect.Height = Math.Abs(boundingRectSize.Y);
            }
            else if (boundingRectSize.X <0 && boundingRectSize.Y < 0)
            {
                bRect.X = boundingRectOrigin.X + boundingRectSize.X;
                bRect.Y = boundingRectOrigin.Y + boundingRectSize.Y;
                bRect.Width = Math.Abs(boundingRectSize.X);
                bRect.Height = Math.Abs(boundingRectSize.Y);
            }

            return bRect;

        }



      
    }
}
