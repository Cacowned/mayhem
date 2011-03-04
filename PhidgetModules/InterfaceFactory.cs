using Phidgets;

namespace PhidgetModules
{
	public static class InterfaceFactory
	{
		private static InterfaceKit ifKit;
		private static RFID rfid;

		public static InterfaceKit GetInterface() {
			if (ifKit == null) {
				ifKit = new InterfaceKit();
				ifKit.open();
			}

			return ifKit;
		}

		public static RFID GetRFID() {
			if (rfid == null) {
				rfid = new RFID();
				rfid.open();
			}

			return rfid;
		}
	}
}
