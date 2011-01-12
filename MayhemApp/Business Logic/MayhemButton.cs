using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MayhemApp.Business_Logic;

namespace MayhemApp
{

    public class MayhemButton
    {

        public enum MayhemButtonType
        {
           PLACEHOLDER,
           TRIGGER,
           ACTION
        }

        #region Fields that get displayed
           public string Text { get; set; }
           public BitmapImage Img { get; set; }
           public BitmapImage glowImg { get; set; }
           public string SubTitle { get; set; }
           public string HelpText { get; set; }
        #endregion

       public delegate void DoubleClickHandler(object sender, MouseEventArgs e);
       public event DoubleClickHandler OnDoubleClick;
       public readonly  RoutedUICommand ClickCommand;


       public UserControl control = null;
       public MayhemConnectionItem connectionItem = null;

       public MayhemButtonType buttonType = MayhemButtonType.PLACEHOLDER;

       public MayhemButton() {}

        public void GotDoubleClicked(object sender, MouseEventArgs e)
        {
            Debug.Write("Just Got DoubleClicked");

            if (this.OnDoubleClick != null)
                OnDoubleClick(this, e);

        }

        #region constructors

        public MayhemButton(string text, string subTitle, string helpText, BitmapImage image, BitmapImage glowImage, MayhemButtonType type)
            : this(text, image, glowImage, type)
        {
            SubTitle = subTitle;
            buttonType = type;
            HelpText = helpText;
        }

        public MayhemButton(string text, BitmapImage image, BitmapImage glowImage, MayhemButtonType type)
            : this(text, image)
        {
            glowImg = glowImage;
            buttonType = type;
        }

        public  MayhemButton(string text, BitmapImage image)
        {
            Text = text;
            Img = image;
        }

        #endregion
    }

    public class MayhemButtonPlaceHolder : MayhemButton {

        public MayhemButtonPlaceHolder(string text, BitmapImage image)
        {
            Text = text;
            Img = image;
        }
    }
}