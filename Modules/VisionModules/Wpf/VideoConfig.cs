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
using System.Windows.Controls;

namespace VisionModules.Wpf
{
    public class VideoConfig : PictureConfig
    {

        private CheckBox compress; 
        public bool compress_video
        {
            get 
            {
                    if (compress.IsChecked != null)
                    {
                        return (bool)compress.IsChecked;
                    }
                    else
                    {
                        return false; 
                    }
            }
        }

        public  VideoConfig(string location, double capture_offset_time) : base(location, capture_offset_time)
        {
            Init();
        }


        /// <summary>
        /// Hide the old Init() method. It should get called during construction, though. 
        /// </summary>
        public new void Init()
        {
            int max_duration = Camera.LOOP_DURATION / 1000; 
            slider_capture_offset.Minimum = 0;
            slider_capture_offset.Maximum = max_duration;
            // changes to the duration selection
            slider_capture_offset.SelectionStart = 0;
            slider_capture_offset.SelectionEnd = max_duration;

            lbl_slider_title.Content = "Select the time offset (max) " + max_duration + "s in the future for the video to be recorded.";
            lbl_img_save.Content = "Click the button below to choose the location of saved videos:";
            img_save_button.Content = "Video Save Location";

            compress = new CheckBox();
            compress.Content = "Compress Video";

            slider_panel.Children.Add(compress);

            

        }

        public override string Title
        {
            get { return "Video"; }
        }

    }
}
