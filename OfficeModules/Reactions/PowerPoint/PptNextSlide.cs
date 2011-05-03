using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Diagnostics;

namespace OfficeModules.Reactions.PowerPoint
{
	[Serializable]
	public class PptNextSlide : ReactionBase
	{
		protected OPowerPoint.Application powerpoint;

		public PptNextSlide()
			: base("PowerPoint: Next Slide", "Navigates to the next slide.") {
		}

		public override void Perform() {
            ErrorLog.AddError(ErrorType.Message, "Trying to go to next slide");
            OPowerPoint.Application powerpoint = OfficeFactory.GetPowerPoint();

            if (powerpoint != null)
            {
                if (powerpoint.SlideShowWindows.Count >= 1)
                {
                    Debug.WriteLine("Whee");
                    powerpoint.SlideShowWindows[1].View.Next();
                }
            }
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
