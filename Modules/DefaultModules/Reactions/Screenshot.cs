using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using DefaultModules.Wpf;
using System.IO;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Screenshot", "Take a screenshot")]
    public class Screenshot : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string saveLocation;

        [DataMember]
        private string filenamePrefix;

        int startIndex;

        public Screenshot()
        {
            saveLocation = AppDomain.CurrentDomain.BaseDirectory;
            filenamePrefix = "Mayhem";
        }

        protected override void Initialize()
        {
            startIndex = 1;
        }

        public override void Perform()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = 0;
            int maxY = 0;

            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                int x = screen.Bounds.X + screen.Bounds.Width;
                if (x > maxX)
                    maxX = x;
                if (screen.Bounds.X < minX)
                    minX = screen.Bounds.X;
                int y = screen.Bounds.Y + screen.Bounds.Height;
                if (y > maxY)
                    maxY = y;
                if (screen.Bounds.Y < minY)
                    minY = screen.Bounds.Y;
            }
            int screenWidth = maxX - minX;
            int screenHeight = maxY - minY;
            Bitmap bmpScreenShot = new Bitmap(screenWidth, screenHeight);
            Graphics gfx = Graphics.FromImage((Image)bmpScreenShot);
            gfx.CopyFromScreen(minX, minY, 0, 0, new Size(screenWidth, screenHeight));

            DateTime now = DateTime.Now;
            string filename = filenamePrefix + "_" + 
                                now.Year.ToString("D2") + "-" +
                                now.Month.ToString("D2") + "-" + 
                                now.Day.ToString("D2") + "_" + 
                                now.Hour.ToString("D2") + "-" +
                                now.Minute.ToString("D2") + "-" +
                                now.Second.ToString("D2") + ".jpg";
            
            filename = Path.Combine(saveLocation, filename);
            bmpScreenShot.Save(filename, ImageFormat.Jpeg);
            startIndex++;
        }
        
        #region Configuration Views

        public WpfConfiguration ConfigurationControl
        {
            get { return new ScreenshotConfig(saveLocation, filenamePrefix); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            ScreenshotConfig config = configurationControl as ScreenshotConfig;
            saveLocation = config.SaveLocation;
            filenamePrefix = config.FilenamePrefix;
        }

        #endregion
    }
}
