using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Outlook = Microsoft.Office.Interop.Outlook;
using System.Runtime.InteropServices;
using MayhemCore;

using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules
{
    public static class OfficeFactory
    {
        private static Outlook.Application outlook;
        private static PowerPoint.Application powerpoint;

        public static Outlook.Application GetOutlook()
        {
            if (outlook == null)
            {
                try {
				    outlook = (Outlook.Application)Marshal.GetActiveObject("Outlook.Application");
			    } catch (Exception e) {
                    ErrorLog.AddError(ErrorType.Failure, "Unable to find the open Outlook application");
                    return null;
			    }
            }

            return outlook;
        }

        public static PowerPoint.Application GetPowerPoint()
        {
            if (powerpoint == null)
            {
                try
                {
                    powerpoint = (PowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                }
                catch (Exception e)
                {
                    ErrorLog.AddError(ErrorType.Message, e.Message);
                    ErrorLog.AddError(ErrorType.Failure, "Unable to find the open PowerPoint application");
                    
                    // something didn't work, make sure powerpoint is null
                    // so that if we run this again, we will try to find it
                    powerpoint = null;
                }
            }

            return powerpoint;
        }
    }
}
