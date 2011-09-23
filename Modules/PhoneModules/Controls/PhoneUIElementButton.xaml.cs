using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Diagnostics;
using Microsoft.Win32;
using MayhemCore;
using System.Globalization;

namespace PhoneModules.Controls
{
    /// <summary>
    /// Interaction logic for UIElementButton.xaml
    /// </summary>
    public partial class PhoneUIElementButton : PhoneUIElement
    {
        Point startPoint;
        Point startCanvasLoc;
        bool isMovingElement = false;
        bool isGridOnRight = true;

        public bool IsGridOnRight
        {
            get { return isGridOnRight; }
            set { isGridOnRight = value; }
        }

        public new PhoneLayoutButton LayoutInfo
        {
            get
            {
                return base.LayoutInfo as PhoneLayoutButton;
            }
            set
            {
                base.LayoutInfo = value;
            }
        }

        public override string Text
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        string imageFile = null;
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
                    FileDictionary.Add(imageFile);
                    
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(value);
                    bi.EndInit();
                    image1.Stretch = Stretch.Fill;
                    image1.Source = bi;
                    image1.Width = bi.Width;
                    image1.Height = bi.Height;
                    image1.Visibility = System.Windows.Visibility.Visible;
                    border1.Visibility = Visibility.Hidden;
                }
            }
        }

        public bool IsSelected
        {
            get;
            set;
        }

        Brush defaultButtonBrush;

        public PhoneUIElementButton()
        {
            InitializeComponent();
            defaultButtonBrush = buttonText.Background;
            stackPanel1.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
        }

        private void PhoneUIElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsSelected)
            {
                Storyboard storyB = (Storyboard)borderSelected.FindResource("storyboardSelected");
                storyB.Begin();
            }
            ResizeTextBox(false);
        }

        void PhoneUIElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(Parent as Canvas);
            if (!isGridOnRight)
                startPoint.X -= gridEdit.ActualWidth;
            startCanvasLoc = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            isMovingElement = true;
            Mouse.Capture(this);
            PhoneUIElement_LostFocus(null, null);
        }

        private void canvas1_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isMovingElement && e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(Parent as Canvas);
                double x = startCanvasLoc.X + point.X - startPoint.X;
                double y = startCanvasLoc.Y + point.Y - startPoint.Y;
                x = Math.Min(Math.Max(0, x), 320 - borderSelected.ActualWidth);
                y = Math.Min(Math.Max(0, y), 450 - borderSelected.ActualHeight);
                
                if (x + ActualWidth > 320)
                {
                    if (isGridOnRight)
                    {
                        isGridOnRight = false;
                        stackPanel1.Children.Remove(gridEdit);
                        stackPanel1.Children.Insert(0, gridEdit);
                    }
                }
                else if (!isGridOnRight)
                {
                    isGridOnRight = true;
                    //x += gridEdit.ActualWidth;
                    stackPanel1.Children.Remove(gridEdit);
                    stackPanel1.Children.Insert(1, gridEdit);
                }
                if (!isGridOnRight)
                    x -= gridEdit.ActualWidth;

                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);
            }
        }

        private void canvas1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            isMovingElement = false;
        }

        public void CanvasClicked()
        {
            PhoneUIElement_LostFocus(null, null);
        }

        private void buttonText_Click(object sender, RoutedEventArgs e)
        {
            ImageFile = null;
            image1.Visibility = Visibility.Hidden;
            image1.Height = 10;
            border1.Visibility = Visibility.Visible;

            textBox1.CaretBrush = new SolidColorBrush(Colors.Black);
            textBox1.IsHitTestVisible = true;
            textBox1.BorderThickness = new Thickness(1);
            textBox1.Focus();
            buttonText.Background = new SolidColorBrush(Colors.LightSkyBlue);
            buttonText.UpdateLayout();
            ResizeTextBox(true);
        }

        private void buttonImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { CheckPathExists = true, RestoreDirectory = true };
            ofd.Filter = "Images|*.jpg;*.jpeg;*.png;*.gif|All Files|*.*";
            if (ofd.ShowDialog() == true)
            {
                ImageFile = ofd.FileName;
                LayoutInfo.ImageFile = ofd.FileName;
            }
        }

        private void PhoneUIElement_LostFocus(object sender, RoutedEventArgs e)
        {
            //this.Focus();
            textBox1.CaretBrush = new SolidColorBrush(Colors.Transparent);
            buttonText.Background = defaultButtonBrush;
            buttonText.UpdateLayout();
            
            textBox1.UpdateLayout();
            textBox1.IsHitTestVisible = false;
            textBox1.BorderThickness = new Thickness(0);
            textBox1.SelectionLength = 0;
            ResizeTextBox(false);
        }

        void ResizeTextBox(bool editing)
        {
            Typeface myTypeface = new Typeface(textBox1.FontFamily, textBox1.FontStyle, textBox1.FontWeight, textBox1.FontStretch);
            FormattedText ft = new FormattedText(textBox1.Text, CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, myTypeface, textBox1.FontSize, Brushes.Black);
            textBox1.Width = ft.WidthIncludingTrailingWhitespace + (editing ? 10 : 5);
            border1.Width = textBox1.Width + 15;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResizeTextBox(true);
        }
    }
}
