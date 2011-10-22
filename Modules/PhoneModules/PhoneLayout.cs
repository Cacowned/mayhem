using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MayhemCore;

namespace PhoneModules
{
    public class PhoneLayout
    {
        public List<PhoneLayoutButton> Buttons
        {
            get;
            set;
        }

        private string htmlTemplate;

        #region Singleton

        private static PhoneLayout instance;

        private PhoneLayout()
        {
            Buttons = new List<PhoneLayoutButton>();
        }

        public static PhoneLayout Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PhoneLayout();
                }

                return instance;
            }
        }
        #endregion

        public PhoneLayoutButton AddButton(string id)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].Id == id)
                {
                    throw new Exception();
                }
            }

            PhoneLayoutButton button = new PhoneLayoutButton();
            button.Text = "Button";
            button.IsEnabled = true;
            button.Id = id;
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
            foreach (PhoneLayoutButton t in Buttons)
            {
                if (t.Id == id)
                {
                    t.IsEnabled = true;
                    break;
                }
            }
        }

        public void DisableButton(string id)
        {
            foreach (PhoneLayoutButton t in Buttons)
            {
                if (t.Id == id)
                {
                    t.IsEnabled = false;
                    break;
                }
            }
        }

        public void RemoveButton(string id)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].Id == id)
                {
                    Buttons.RemoveAt(i);
                    break;
                }
            }
        }

        public PhoneLayoutButton GetById(string id)
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].Id == id)
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
                    Assembly assembly = GetType().Assembly;
                    using (Stream stream = assembly.GetManifestResourceStream("PhoneModules.HtmlTemplate.html"))
                    {
                        using (StreamReader textStreamReader = new StreamReader(stream))
                        {
                            htmlTemplate = textStreamReader.ReadToEnd();
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
                        if (string.IsNullOrEmpty(button.ImageFile))
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

                            string file = Path.Combine(imageDir, button.Id + ".png");
                            File.Copy(button.ImageFile, file, true);

                            width = 96;
                            height = 96;
                            sb.AppendLine("<input type=\"image\" src=\"Images/" + button.Id + "\"");
                        }

                        sb.AppendLine("onClick=\"sendEvent('" + button.Id + "')\"");
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
