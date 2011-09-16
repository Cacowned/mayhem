/*
 * 
 * VideoConfig.cs
 * 
 * Config for the Video Reaction
 * 
 * Derives from PictureConfig.cs, so check out the xaml there 
 * 
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemOpenCVWrapper;

namespace VisionModules.Wpf
{
    public class VideoConfig : PictureConfig
    {

        public  VideoConfig(string location, double capture_offset_time) : base(location, capture_offset_time)
        {
            Init();
        }


        /// <summary>
        /// Hide the old Init() method. It should get called during construction, though. 
        /// </summary>
        public new void Init()
        {
            // changes to the duration selection
            slider_capture_offset.SelectionStart = 0;
            slider_capture_offset.SelectionEnd = Camera.LOOP_DURATION;

            lbl_slider_title.Content = "Select the time offset (max) " + Camera.LOOP_DURATION + "s in the future for the video to be recorded.";
            lbl_img_save.Content = "Click the button below to choose the location of saved videos:"; 

        }

        public new string Title()
        {
            return "Video Reaction";
        }

    }
}
