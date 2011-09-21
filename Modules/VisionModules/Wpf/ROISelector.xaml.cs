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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
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

            motionBoundingRect.Stroke = System.Windows.Media.Brushes.OrangeRed;
            motionBoundingRect.StrokeThickness = 2;
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

        private System.Windows.Shapes.Rectangle motionBoundingRect = new System.Windows.Shapes.Rectangle();

        private modes mode = modes.start;



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

            Canvas.SetLeft(motionBoundingRect, pos.X);
            Canvas.SetTop(motionBoundingRect, pos.Y);

            boundingRectSize = new Point(0, 0);
            motionBoundingRect.Width = 0;
            motionBoundingRect.Height = 0;

            if (!overlay.Children.Contains(motionBoundingRect))
            {
                overlay.Children.Add(motionBoundingRect);
            }

           // overlay.Children.Add(rec);
           // motionBoundingRect = rec; 

        }

        public void DisplayBoundingRect()
        {
           
            Canvas.SetLeft(motionBoundingRect, boundingRectOrigin.X);
            Canvas.SetTop(motionBoundingRect, boundingRectOrigin.Y);

            motionBoundingRect.Width = boundingRectSize.X;
            motionBoundingRect.Height = boundingRectSize.Y;

            if (!overlay.Children.Contains(motionBoundingRect))
            {
                overlay.Children.Add(motionBoundingRect);
            }
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

                //  motionBoundingRect

                boundingRectSize.X = position.X - boundingRectOrigin.X;
                boundingRectSize.Y = position.Y - boundingRectOrigin.Y;
                // x,y > 0
                if (boundingRectSize.X >= 0 && boundingRectSize.Y >= 0)
                {
                    motionBoundingRect.Width = boundingRectSize.X;
                    motionBoundingRect.Height = boundingRectSize.Y;
                    motionBoundingRect.RenderTransform = new RotateTransform(0);
                }
                // x < 0, y> 0
                else if (boundingRectSize.X < 0 && boundingRectSize.Y >= 0)
                {
                    // rotate

                    TransformGroup g = new TransformGroup();


                    Transform r = new RotateTransform(180);
                    Transform t = new TranslateTransform(0, motionBoundingRect.Height);

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
                else if (boundingRectSize.X >= 0 && boundingRectSize.Y < 0)
                {
                    TransformGroup g = new TransformGroup();


                    //Transform r = new RotateTransform(180);
                    Transform t = new TranslateTransform(0, -motionBoundingRect.Height);
                    g.Children.Add(t);
                    // g.Children.Add(t);

                    motionBoundingRect.Width = Math.Abs(boundingRectSize.X);
                    motionBoundingRect.Height = Math.Abs(boundingRectSize.Y);

                    motionBoundingRect.RenderTransform = g;
                }


                prev_position = position;
            }

        }

        public  void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Logger.WriteLine("onverlay_MouseLeave");
            if (!(e.LeftButton == MouseButtonState.Pressed))
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

