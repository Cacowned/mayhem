using System;
using System.Runtime.Serialization;
using MayhemCore;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace DefaultModules.Reactions.Office.PowerPoint
{
    [Serializable]
    public class PptNextSlide : ReactionBase
    {
        public PptNextSlide ()
            : base("PowerPoint: Next Slide", "Navigates to the next slide.") {
        }
        public override void Perform() {
            OPowerPoint.Application oApp;

            oApp = (OPowerPoint.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("PowerPoint.Application");

            // If we have a presentation window, go to the next slide
            if (oApp.SlideShowWindows.Count >= 1) {
                oApp.SlideShowWindows[1].View.Next();
            }

            oApp = null;
        }

        #region Serialization
        public PptNextSlide(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
