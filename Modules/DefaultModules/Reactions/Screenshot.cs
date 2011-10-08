using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

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

        protected override void OnLoadDefaults()
        {
            saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            filenamePrefix = "Mayhem";
        }

        public override void Perform()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = 0;
            int maxY = 0;

            foreach (Screen screen in Screen.AllScreens)
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
            bmpScreenShot.Dispose();
        }

        public string GetConfigString()
        {
            return Path.Combine(saveLocation, filenamePrefix + "*.jpg");
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
