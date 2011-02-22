using System;
using System.Runtime.Serialization;
using MayhemCore;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace DefaultModules.Reactions.Office.PowerPoint
{
    [Serializable]
    public class PptPrevSlide : ReactionBase
    {
        public PptPrevSlide()
            : base("PowerPoint: Last Slide", "Navigates to the previous slide.") {
        }
        public override void Perform() {
            OPowerPoint.Application oApp;

            oApp = (OPowerPoint.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("PowerPoint.Application");

            // If we have a presentation window, go to the prev slide
            if (oApp.SlideShowWindows.Count >= 1) {
                oApp.SlideShowWindows[1].View.Last();
            }

            oApp = null;
        }

        #region Serialization
        public PptPrevSlide(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
