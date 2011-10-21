using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Point = System.Drawing.Point;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class DetectionEventArgs : EventArgs
    {
        public List<Point> Points
        {
            get;
            private set;
        }
        
        public DetectionEventArgs(List<Point> points)
        {
            Points = points;
        }

        public DetectionEventArgs(Point[] points)
        {
            Points = points.ToList();
        }
    }
}
