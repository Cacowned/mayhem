using System;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;

namespace VisionModules.Components
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

        public DetectionEventArgs(IEnumerable<Point> points)
        {
            Points = points.ToList();
        }
    }
}
