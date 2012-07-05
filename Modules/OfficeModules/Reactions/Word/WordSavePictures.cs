using System;
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
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Reactions.Word
{
    /// <summary>
    /// A reaction that will save the pictures from the current document.
    /// </summary>
    [DataContract]
    [MayhemModule("Word: Save Pictures", "Saves the pictures from the current document")]
    public class WordSavePictures : ReactionBase, IWpfConfigurable
    {
        /// <summary>
        /// The path of the folder where the pictures will be saved.
        /// </summary>
        [DataMember]
        private string fileName;

        private OWord.Application app;
        private string documentName;

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
        /// This method will get the instance of the Word application and save the pictures from the current document.
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
                app = (OWord.Application)Marshal.GetActiveObject("Word.Application");
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_ApplicationNotFound);
                Logger.Write(ex);

                return;
            }

            try
            {
                OWord.Document activeDocument = app.ActiveDocument;

                if (activeDocument == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Word_NoActiveDocument);
                }
                else
                {
                    List<OWord.InlineShape> inlineShapes = new List<OWord.InlineShape>();

                    documentName = activeDocument.Name;

                    if (documentName.Contains(".docx"))
                        documentName = documentName.Remove(documentName.LastIndexOf(".docx"));

                    if (documentName.Contains(".doc"))
                        documentName = documentName.Remove(documentName.LastIndexOf(".doc"));

                    foreach (OWord.InlineShape inlineShape in activeDocument.InlineShapes)
                    {
                        if (inlineShape.Type == Microsoft.Office.Interop.Word.WdInlineShapeType.wdInlineShapePicture)
                        {
                            inlineShapes.Add(inlineShape);
                        }
                    }

                    if (inlineShapes.Count > 0)
                    {
                        Thread thread = new Thread(this.SavePictures);
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start(inlineShapes);

                        thread.Join();
                    }
                    else
                    {
                        ErrorLog.AddError(ErrorType.Warning, Strings.Word_NoPicturesInDocument);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_NoSlideShowWindow);
                Logger.Write(ex);
            }
            finally
            {
                app = null;
            }
        }

        /// <summary>
        /// This method will save the pictures received as a parameter to the predefined path.
        /// </summary>
        /// <param name="p_inlineShapes">A list of pictures that need to be saved</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SavePictures(object p_inlineShapes)
        {
            int count;
            List<OWord.InlineShape> inlineShapes;

            count = 0;
            inlineShapes = p_inlineShapes as List<OWord.InlineShape>;

            if (inlineShapes == null)
                return;

            FileStream streamWriter = null;

            try
            {
                foreach (OWord.InlineShape inlineShape in inlineShapes)
                {
                    inlineShape.Select();

                    if (app == null)
                        return;

                    app.Selection.CopyAsPicture();

                    bool containsImage = Clipboard.ContainsImage();

                    if (containsImage)
                    {
                        count++;

                        BitmapSource image = Clipboard.GetImage();

                        streamWriter = new FileStream(fileName + "\\" + documentName + "_pic" + count + ".jpg", FileMode.Create);
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                        encoder.Frames.Add(BitmapFrame.Create(image));
                        encoder.Save(streamWriter);

                        streamWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_CantSavePictures);
                Logger.WriteLine(ex);

                try
                {
                    if (streamWriter != null)
                        streamWriter.Close();
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
            get { return new WordSavePicturesConfig(fileName); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var wordSavePicturesConfig = configurationControl as WordSavePicturesConfig;
            fileName = wordSavePicturesConfig.FileName;
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
