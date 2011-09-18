using Phidgets;

namespace PhidgetModules
{
    public static class InterfaceFactory
    {
        private static InterfaceKit ifKit;
        private static RFID rfid;
        private static IR ir;

        public static InterfaceKit Interface
        {
            get
            {
                if (ifKit == null)
                {
                    ifKit = new InterfaceKit();
                    ifKit.open();
                }

                return ifKit;
            }
        }

        public static RFID Rfid
        {
            get
            {
                if (rfid == null)
                {
                    rfid = new RFID();
                    rfid.open();
                }

                return rfid;
            }
        }

        public static IR IR
        {
            get
            {
                if (ir == null)
                {
                    ir = new IR();
                    ir.open();
                }

                return ir;
            }
        }
    }
}
