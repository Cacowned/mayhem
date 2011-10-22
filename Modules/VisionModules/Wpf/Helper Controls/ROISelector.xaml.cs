using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MayhemCore;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Region of interest selector widget for Mayhem
    /// vision modules.
    /// </summary>
    public partial class ROISelector : UserControl
    {
        //------   ROI Selection with Mouse
        private bool startDrag;
        private Point prevPosition;

        private Point boundingRectOrigin;
        private Point boundingRectSize;

        private enum Modes
        {
            Start,
            Selecting,
            Selected
        }

        private Modes mode;

        public ROISelector()
        {
            mode = Modes.Start;
            boundingRectSize = new Point(0, 0);
            boundingRectOrigin = new Point(0, 0);
            prevPosition = new Point(-1.0, -1.0);

            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Loaded += ROISelector_Loaded;
        }

        private void ROISelector_Loaded(object sender, RoutedEventArgs e)
        {
            OutterRect.Rect = new Rect(0, 0, ActualWidth, ActualHeight);
        }

        #region ROI Selection with Mouse
        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mode != Modes.Start)
            {
                Point mouseUpPos = e.GetPosition(this);

                boundingRectSize.X = mouseUpPos.X - boundingRectOrigin.X;
                boundingRectSize.Y = mouseUpPos.Y - boundingRectOrigin.Y;

                Logger.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> overlay_MouseLeftButtonUp" + boundingRectOrigin + " -- " + boundingRectSize);
                Cursor = Cursors.Arrow;
                startDrag = false;
                mode = Modes.Start;
            }
        }

        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Logger.WriteLine("onverlay_MouseLeftButtonDown");

            mode = Modes.Selecting;
            Cursor = Cursors.Cross;

            startDrag = true;

            Point pos = e.GetPosition(this);
            boundingRectOrigin = pos;

            var rect = InnerRect.Rect;
            rect.X = pos.X;
            rect.Y = pos.Y;
            boundingRectSize = new Point(0, 0);
            rect.Width = 0;
            rect.Height = 0;

            InnerRect.Rect = rect;
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

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            Logger.WriteLine("overlay_MouseMove");
            if (startDrag)
            {
                Point position = e.GetPosition(this);

                if (prevPosition.X < 0 && prevPosition.Y < 0)
                {
                    prevPosition = position;
                }

                // InnerRect
                boundingRectSize.X = position.X - boundingRectOrigin.X;
                boundingRectSize.Y = position.Y - boundingRectOrigin.Y;

                // x,y > 0
                if (boundingRectSize.X >= 0 && boundingRectSize.Y >= 0)
                {
                    var rect = InnerRect.Rect;
                    rect.Width = boundingRectSize.X;
                    rect.Height = boundingRectSize.Y;

                    InnerRect.Rect = rect;
                }
                else if (boundingRectSize.X < 0 && boundingRectSize.Y >= 0)
                {
                    // x < 0, y> 0
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
                }
                else if (boundingRectSize.X < 0 && boundingRectSize.Y < 0)
                {
                    // x < 0, y < 0
                    TransformGroup g = new TransformGroup();

                    Transform r = new RotateTransform(180);

                    g.Children.Add(r);

                    var rect = InnerRect.Rect;
                    rect.Width = Math.Abs(boundingRectSize.X);
                    rect.Height = Math.Abs(boundingRectSize.Y);
                    InnerRect.Rect = rect;
                }
                else if (boundingRectSize.X >= 0 && boundingRectSize.Y < 0)
                {
                    // x>0, y < 0
                    TransformGroup g = new TransformGroup();

                    Transform t = new TranslateTransform(0, -InnerRect.Rect.Height);
                    g.Children.Add(t);

                    var rect = InnerRect.Rect;
                    rect.Width = Math.Abs(boundingRectSize.X);
                    rect.Height = Math.Abs(boundingRectSize.Y);

                    InnerRect.Rect = rect;
                }

                prevPosition = position;
            }
        }

        public void OnMouseLeave(object sender, MouseEventArgs e)
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
                origin.Y = boundingRectOrigin.Y;
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

        public void OnMouseEnter(object sender, MouseEventArgs e)
        {
            // todo
        }

        #endregion
    }
}