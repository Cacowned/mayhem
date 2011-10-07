/*
 *  ROISelector.xaml.cs
 * 
 *  Region of interest selector widget for Mayhem
 *  vision modules. 
 * 
 *  (c) 2011, Microsoft Applied Sciences Group
 *  Author: Sven Kratz
 */ 


using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MayhemCore;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for ROISelector.xaml
    /// </summary>
    public partial class ROISelector : UserControl
    {
        public ROISelector()
        {
            InitializeComponent();
        }

        //------   ROI Selection with Mouse

        private bool startDrag = false;
        private System.Windows.Point prev_position = new System.Windows.Point(-1.0, -1.0);


        public System.Windows.Point boundingRectOrigin = new System.Windows.Point(0, 0);
        public System.Windows.Point boundingRectSize = new System.Windows.Point(0, 0);

        private enum modes
        {
            start,
            selecting,
            selected
        }

        private modes mode = modes.start;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Loaded += new RoutedEventHandler(ROISelector_Loaded);
        }

        void ROISelector_Loaded(object sender, RoutedEventArgs e)
        {
            OutterRect.Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
        }

        #region ROI Selection with Mouse
        public  void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mode !=  modes.start)
            {
            Point mouseUpPos = e.GetPosition(this);

            boundingRectSize.X = mouseUpPos.X - boundingRectOrigin.X;
            boundingRectSize.Y = mouseUpPos.Y - boundingRectOrigin.Y; 



            Logger.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> overlay_MouseLeftButtonUp" + boundingRectOrigin + " -- " + boundingRectSize);
            this.Cursor = Cursors.Arrow;
            startDrag = false;
            mode = modes.start;
            }

        }

        public  void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Logger.WriteLine("onverlay_MouseLeftButtonDown");

            mode = modes.selecting;
            this.Cursor = Cursors.Cross;

            startDrag = true;

            System.Windows.Point pos = e.GetPosition(this);
            boundingRectOrigin = pos;

            var rect = InnerRect.Rect;
            rect.X = pos.X;
            rect.Y = pos.Y;
            boundingRectSize = new Point(0, 0);
            rect.Width = 0;
            rect.Height = 0;

            InnerRect.Rect = rect;

           // overlay.Children.Add(rec);
           // InnerRect = rec; 

        }

        public void DisplayBoundingRect()
        {

            var rect = InnerRect.Rect;
            rect.X = boundingRectOrigin.X;
            rect.Y = boundingRectOrigin.Y;

            rect.Width = boundingRectSize.X;
            rect.Height = boundingRectSize.Y;

            InnerRect.Rect = rect;           
        }

        /**<summary>
         * Displays a given bounding rectangle.
         * This function will normally be used on deserialization with a previously set bounding rect. 
         * </summary> */
        public void DisplayBoundingRect(Rect bRect)
        {
            boundingRectOrigin = new Point(bRect.X, bRect.Y);
            boundingRectSize = new Point(bRect.Width, bRect.Height);

            DisplayBoundingRect();
        }

        public  void OnMouseMove(object sender, MouseEventArgs e)
        {
            Logger.WriteLine("overlay_MouseMove");
            if (startDrag)
            {
                System.Windows.Point position = e.GetPosition(this);

                if (prev_position.X < 0 && prev_position.Y < 0)
                {
                    prev_position = position;
                }

                //  InnerRect

                boundingRectSize.X = position.X - boundingRectOrigin.X;
                boundingRectSize.Y = position.Y - boundingRectOrigin.Y;
                // x,y > 0
                if (boundingRectSize.X >= 0 && boundingRectSize.Y >= 0)
                {
                    var rect = InnerRect.Rect;
                    rect.Width = boundingRectSize.X;
                    rect.Height = boundingRectSize.Y;
                    //InnerRect.RenderTransform = new RotateTransform(0);

                    InnerRect.Rect = rect;
                }
                // x < 0, y> 0
                else if (boundingRectSize.X < 0 && boundingRectSize.Y >= 0)
                {
                    // rotate

                    TransformGroup g = new TransformGroup();


                    Transform r = new RotateTransform(180);
                    Transform t = new TranslateTransform(0, InnerRect.Rect.Height);

                    g.Children.Add(r);
                    g.Children.Add(t);

                    var rect = InnerRect.Rect;
                    rect.Width = Math.Abs(boundingRectSize.X);
                    rect.Height = boundingRectSize.Y;
                    InnerRect.Rect = rect;
                    //InnerRect.RenderTransform = g;
                }
                // x < 0, y < 0
                else if (boundingRectSize.X < 0 && boundingRectSize.Y < 0)
                {
                    TransformGroup g = new TransformGroup();


                    Transform r = new RotateTransform(180);
                    //Transform t = new TranslateTransform(0, InnerRect.Height);

                    g.Children.Add(r);
                    //g.Children.Add(t);

                    var rect = InnerRect.Rect;
                    rect.Width = Math.Abs(boundingRectSize.X);
                    rect.Height = Math.Abs(boundingRectSize.Y);
                    InnerRect.Rect = rect;
                    //InnerRect.RenderTransform = g;
                }
                // x>0, y < 0
                else if (boundingRectSize.X >= 0 && boundingRectSize.Y < 0)
                {
                    TransformGroup g = new TransformGroup();


                    //Transform r = new RotateTransform(180);
                    Transform t = new TranslateTransform(0, -InnerRect.Rect.Height);
                    g.Children.Add(t);
                    // g.Children.Add(t);

                    var rect = InnerRect.Rect;
                    rect.Width = Math.Abs(boundingRectSize.X);
                    rect.Height = Math.Abs(boundingRectSize.Y);

                    InnerRect.Rect = rect;

                    //InnerRect.RenderTransform = g;
                }


                prev_position = position;
            }

        }

        public  void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Logger.WriteLine("onverlay_MouseLeave");
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                startDrag = false;
            }
        }



        public Rect GetBoundingRect()
        {


            // TODO use this code in the "overlay_MouseMove" function (????) 


            Point origin = new Point();
            Size size = new Size();

            // adapt to negative width/height values
            // origin must always be in the upper right corner
            
            if (boundingRectSize.X >= 0 && boundingRectSize.Y >= 0)
            {
                origin.X = boundingRectOrigin.X;
                origin.Y = boundingRectOrigin.Y;
                size.Width = boundingRectSize.X;
                size.Height = boundingRectSize.Y;
            }
            else if (boundingRectSize.X < 0 && boundingRectSize.Y >= 0)
            {
                origin.X = boundingRectOrigin.X + boundingRectSize.X;
                origin.Y =  boundingRectOrigin.Y;
                size.Width = Math.Abs(boundingRectSize.X);
                size.Height = boundingRectSize.Y;


            }
            else if (boundingRectSize.X >= 0 && boundingRectSize.Y < 0)
            {
                origin.X = boundingRectOrigin.X;
                origin.Y = boundingRectOrigin.Y + boundingRectSize.Y;
                size.Width = boundingRectSize.X;
                size.Height = Math.Abs(boundingRectSize.Y);
            }
            else if (boundingRectSize.X < 0 && boundingRectSize.Y < 0)
            {
                origin.X = boundingRectOrigin.X + boundingRectSize.X;
                origin.Y = boundingRectOrigin.Y + boundingRectSize.Y;
                size.Width = Math.Abs(boundingRectSize.X);
                size.Height = Math.Abs(boundingRectSize.Y);
            }  
       
           
            return new Rect(origin, size);


        }

        public  void OnMouseEnter(object sender, MouseEventArgs e)
        {
            // todo
        }

        #endregion

    }
}

