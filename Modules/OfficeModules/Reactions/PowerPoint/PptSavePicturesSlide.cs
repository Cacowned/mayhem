﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    [DataContract]
    [MayhemModule("PowerPoint: Save pictures", "Saves the pictures from the current slide")]
    public class PptSavePicturesSlide : ReactionBase, IWpfConfigurable
    {
        private OPowerPoint.Application app;

        [DataMember]
        private string FileName
        {
            get;
            set;
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var pptSavePicturesConfig = configurationControl as PowerPointSavePicturesConfig;
            FileName = pptSavePicturesConfig.Filename;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!Directory.Exists(FileName))
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.General_DirectoryNotFound);
                e.Cancel = true;
            }
        }

        public override void Perform()
        {
            if (!Directory.Exists(FileName))
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
                        ErrorLog.AddError(ErrorType.Message, Strings.PowerPoint_NoSlideShowWindow);
                    }
                    else
                    {
                        OPowerPoint.Slide slide = activePresentation.SlideShowWindow.View.Slide;

                        //verify if slide is not null
                        if (slide == null)
                        {
                            ErrorLog.AddError(ErrorType.Message, Strings.PowerPoint_NoSlideSelected);
                        }
                        else
                        {

                            List<OPowerPoint.Shape> shapes = new List<OPowerPoint.Shape>();
                            foreach (OPowerPoint.Shape shape in slide.Shapes)
                            {
                                //we save to all list all shapes that may contain imagies
                                if (shape.HasTextFrame != Microsoft.Office.Core.MsoTriState.msoTrue)
                                    shapes.Add(shape);
                            }

                            if (shapes.Count > 0) //exists images
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
                ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_NoSlideShowWindow);
                Logger.Write(e);
            }
            finally
            {
                app = null;
            }
        }

        public void SavePicture(object p_shapes)
        {
            int count;
            List<OPowerPoint.Shape> shapes;
            count = 0;

            lock (this)
            {
                shapes = p_shapes as List<OPowerPoint.Shape>;

                if (shapes == null)
                    return;

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

                            FileStream stream = new FileStream(FileName + "\\pic" + count + ".jpg", FileMode.Create);
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
                }
            }

        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new PowerPointSavePicturesConfig(FileName); }
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.General_DirectorySaveConfigString, Path.GetFullPath(FileName));
        }

        #endregion
    }
}
