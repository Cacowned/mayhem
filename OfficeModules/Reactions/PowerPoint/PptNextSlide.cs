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
		protected OPowerPoint.Application oApp;

		public PptNextSlide()
			: base("PowerPoint: Next Slide", "Navigates to the next slide.") {
		}

		public override void Enable() {
			base.Enable();

			try {
				oApp = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
			} catch (Exception e) {
				Debug.Write(e);
			}
		}

		public override void Disable() {
			base.Disable();

			oApp = null;
		}

		public override void Perform() {
			try {
				// If we have a presentation window, go to the next slide
				if (oApp.SlideShowWindows.Count >= 1) {
					oApp.SlideShowWindows[1].View.Next();
				}
			} catch (Exception e) {
				Debug.Write(e);
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
