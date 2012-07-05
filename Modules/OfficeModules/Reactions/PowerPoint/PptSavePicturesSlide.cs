﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using OfficeModules.Resources;
using OfficeModules.Wpf;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions.PowerPoint
{
    /// <summary>
    /// A reaction that will save the pictures from the current slide of the active slideshow.
    /// </summary>
    [DataContract]
    [MayhemModule("PowerPoint: Save Pictures", "Saves the pictures from the current slide")]
    public class PptSavePicturesSlide : ReactionBase, IWpfConfigurable
    {
        /// <summary>
        /// The path of the folder where the pictures will be saved.
        /// </summary>
        [DataMember]
        private string fileName;

        private OPowerPoint.Application app;
        private string presentationName;

        /// <summary>
        /// This method will check if the folder exists.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!Directory.Exists(fileName))
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.General_DirectoryNotFound);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method will get the instance of the PowerPoint application and save the pictures from the current slide of the active slideshow.
        /// </summary>
        public override void Perform()
        {
            if (!Directory.Exists(fileName))
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.General_DirectoryNotFound);
                return;
            }

            try
            {
                app = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
                Logger.Write(ex);

                return;
            }

            try
            {
                OPowerPoint.Presentation activePresentation = app.ActivePresentation;

                if (activePresentation == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_NoActivePresentation);
                }
                else
                    if (activePresentation.SlideShowWindow == null)
                    {
                        ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_NoSlideShowWindow);
                    }
                    else
                    {
                        OPowerPoint.Slide slide = activePresentation.SlideShowWindow.View.Slide;

                        presentationName = activePresentation.Name;

                        if (presentationName.Contains(".pptx"))
                            presentationName = presentationName.Remove(presentationName.LastIndexOf(".pptx"));

                        if (presentationName.Contains(".ppt"))
                            presentationName = presentationName.Remove(presentationName.LastIndexOf(".ppt"));

                        if (slide == null)
                        {
                            ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_NoSlideSelected);
                        }
                        else
                        {
                            List<OPowerPoint.Shape> shapes = new List<OPowerPoint.Shape>();

                            foreach (OPowerPoint.Shape shape in slide.Shapes)
                            {
                                // We add to list all shapes that may contain imagies
                                if (shape.HasTextFrame != Microsoft.Office.Core.MsoTriState.msoTrue)
                                    shapes.Add(shape);
                            }

                            // We have images for saving
                            if (shapes.Count > 0)
                            {
                                Thread thread = new Thread(this.SavePicture);
                                thread.SetApartmentState(ApartmentState.STA);
                                thread.Start(shapes);

                                thread.Join();
                            }
                        }
                    }
            }
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_NoSlideShowWindow);
                Logger.Write(e);
            }
            finally
            {
                app = null;
            }
        }

        /// <summary>
        /// This method will save the pictures received as a parameter to the predefined path.
        /// </summary>
        /// <param name="pShapes">A list of pictures that need to be saved</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SavePicture(object pShapes)
        {
            int count = 0;
            List<OPowerPoint.Shape> shapes;

            shapes = pShapes as List<OPowerPoint.Shape>;

            if (shapes == null)
                return;

            FileStream stream = null;

            try
            {
                foreach (OPowerPoint.Shape shape in shapes)
                {
                    shape.Copy();

                    bool containsImage = Clipboard.ContainsImage();

                    if (containsImage)
                    {
                        count++;

                        BitmapSource image = Clipboard.GetImage();

                        stream = new FileStream(fileName + "\\" + presentationName + "_pic" + count + ".jpg", FileMode.Create);

                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                        encoder.Frames.Add(BitmapFrame.Create(image));
                        encoder.Save(stream);

                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_CantSavePictures);
                Logger.WriteLine(ex.Message);

                try
                {
                    if (stream != null)
                        stream.Close();
                }
                catch (IOException e)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.CantCloseFileStream);
                    Logger.WriteLine(e);
                }
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new PowerPointSavePicturesConfig(fileName); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var pptSavePicturesConfig = configurationControl as PowerPointSavePicturesConfig;

            fileName = pptSavePicturesConfig.FileName;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.General_DirectorySaveConfigString, Path.GetFullPath(fileName));
        }

        #endregion
    }
}
