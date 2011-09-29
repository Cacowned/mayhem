using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using MayhemCore;

namespace PhoneModules
{
    public class PhoneLayoutElement
    {
        public string ID;
        public double X;
        public double Y;
        public double Width;
        public double Height;
    }

    public class PhoneLayoutButton : PhoneLayoutElement
    {
        public string Text = "";
        private string imageFile = "";
        public string ImageFile
        {
            get
            {
                return imageFile;
            }
            set
            {
                imageFile = value;
                if (imageFile != null && imageFile.Length > 0)
                {
                    ImageBytes = FileDictionary.Get(imageFile);
                }
            }
        }
        public byte[] ImageBytes = new byte[0];
        public bool IsEnabled = true;
    }

    public class PhoneLayout
    {
        public List<PhoneLayoutButton> Buttons = new List<PhoneLayoutButton>();

        string htmlTemplate = null;

        #region Singleton
        static readonly PhoneLayout instance = new PhoneLayout();

        PhoneLayout()
        {
        }

        public static PhoneLayout Instance
        {
            get { return instance; }
        }
        #endregion

        public PhoneLayoutButton AddButton(string id)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].ID == id)
                {
                    throw new Exception();
                }
            }
            PhoneLayoutButton button = new PhoneLayoutButton();
            button.Text = "Button";
            button.IsEnabled = true;
            button.ID = id;
            button.X = 110;
            button.Y = 200;
            AddButton(button);
            return button;
        }

        public void AddButton(PhoneLayoutButton button)
        {
            Buttons.Add(button);
        }

        public void EnableButton(string id)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].ID == id)
                {
                    Buttons[i].IsEnabled = true;
                    break;
                }
            }
        }

        public void DisableButton(string id)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].ID == id)
                {
                    Buttons[i].IsEnabled = false;
                    break;
                }
            }
        }

        public void RemoveButton(string id)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].ID == id)
                {
                    Buttons.RemoveAt(i);
                    break;
                }
            }
        }

        public PhoneLayoutButton GetByID(string id)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].ID == id)
                {
                    return Buttons[i];
                }
            }
            return null;
        }

        public string SerializeToHtml(bool includeButtons, out string insideDiv)
        {
            string html = htmlTemplate;

            if (htmlTemplate == null)
            {
                try
                {
                    Assembly assembly = this.GetType().Assembly;
                    using (Stream stream = assembly.GetManifestResourceStream("PhoneModules.HtmlTemplate.html"))
                    {
                        using (StreamReader _textStreamReader = new StreamReader(stream))
                        {
                            htmlTemplate = _textStreamReader.ReadToEnd();
                        }
                    }
                    html = htmlTemplate;
                }
                catch (Exception erf)
                {
                    Logger.WriteLine(erf);
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"buttons\">");
            if (Buttons.Count > 0 && includeButtons)
            {
                foreach (PhoneLayoutButton button in Buttons)
                {
                    if (button.IsEnabled)
                    {
                        int width;
                        int height;
                        if (button.ImageFile == null || button.ImageFile == "")
                        {
                            sb.AppendLine("<input type=\"button\" value=\"" + button.Text + "\" class=\"button\"");
                            width = (int)button.Width;
                            height = (int)button.Height;
                        }
                        else
                        {
                            string imageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                            if (!Directory.Exists(imageDir))
                            {
                                Directory.CreateDirectory(imageDir);
                            }
                            string file = Path.Combine(imageDir, button.ID + ".png");
                            File.Copy(button.ImageFile, file, true);
                            //sb.AppendLine("<input type=\"image\" src=\"Images/" + button.ID + "\"");
                            width = 96;
                            height = 96;
                            sb.AppendLine("<input type=\"image\" src=\"Images/" + button.ID + "\"");
                        }

                        sb.AppendLine("onClick=\"sendEvent('" + button.ID + "')\"");
                        sb.AppendLine("style=\"position: absolute; top: " + button.Y + "px; left: " + button.X + "px; width:" + width + "px; height: " + height + "px;\" />");
                    }
                }
            }
            sb.AppendLine("</div>");
            insideDiv = sb.ToString();
            html = html.Replace("%%INSERTBODYHERE%%", insideDiv);

            return html;
        }
    }
}
