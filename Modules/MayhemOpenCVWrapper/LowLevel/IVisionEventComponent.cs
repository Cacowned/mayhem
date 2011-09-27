﻿/*
 *   IVisionEventComponent
 * 
 *   Abstract base class for Event components that use vision. 
 *   (c) Microsoft Applied Sciences Group, 2011
 *   
 *   Author: Sven Kratz
 *   
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemOpenCVWrapper;
using MayhemCore;

namespace MayhemOpenCVWrapper.LowLevel
{
    public abstract class IVisionEventComponent
    {

        protected Camera.ImageUpdateHandler imageUpdateHandler;

        public IVisionEventComponent()
        {
            imageUpdateHandler = new Camera.ImageUpdateHandler(update_frame);
        }

        /// <summary>
        /// Register and unregister for image callbacks
        /// </summary> 
        public virtual void RegisterForImages(ImagerBase c)
        {
            Logger.WriteLine("");
            if (c != null)
            {
                c.OnImageUpdated -= imageUpdateHandler;
                c.OnImageUpdated += imageUpdateHandler;
            }
        }
        public virtual void UnregisterForImages(ImagerBase c)
        {
            Logger.WriteLine("");
            if (c != null)
                c.OnImageUpdated -= imageUpdateHandler;
        }

        /// <summary>
        ///  Processes a new frame from the camera and decides if event should be triggered
        /// </summary>
        public abstract void update_frame(object sender, EventArgs e);

    }
}
