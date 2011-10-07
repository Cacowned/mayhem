/*
 *  DummyCameraImageListener.cs 
 * 
 *  Fake Camera Listener used by Picture and Vidoe to keep subscribed to camera updates
 * 
 * (c) 2011 Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class DummyCameraImageListener : CameraImageListener
    {
        public override void UpdateFrame(object sender, EventArgs e)
        {
           // Do Nothing
        }
    }
}
