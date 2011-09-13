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
                    //textBox1.Visibility = System.Windows.Visibility.Hidden;
                    //this.Width = bi.Width+10;
                    //this.Height = bi.Height;
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
        }

        private void PhoneUIElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsSelected)
            {
                Storyboard storyB = (Storyboard)borderSelected.FindResource("storyboardSelected");
                storyB.Begin();
            }
        }

        void PhoneUIElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Down");
            startPoint = e.GetPosition(Parent as Canvas);
            startCanvasLoc = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            //elementMoved = false;
            isMovingElement = true;
            Mouse.Capture(this);
            PhoneUIElement_LostFocus(null, null);
        }

        private void canvas1_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isMovingElement && e.LeftButton == MouseButtonState.Pressed)
            {
                Debug.WriteLine("Move");
                //elementMoved = true;
                Point point = e.GetPosition(Parent as Canvas);
                double x = startCanvasLoc.X + point.X - startPoint.X;
                double y = startCanvasLoc.Y + point.Y - startPoint.Y;
                x = Math.Min(Math.Max(0, x), 320 - borderSelected.ActualWidth);
                y = Math.Min(Math.Max(0, y), 450 - borderSelected.ActualHeight);
                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);
        //        this.LayoutInfo.X = x;
     //           this.LayoutInfo.Y = y;
            }
        }

        private void canvas1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Up");
            //if (currentlyMovingElement != null)
            //{
            //if (!elementMoved)
            //{
            //if (selectedBorder.Child is PhoneUIElementButton)
            //{
            //currentlySelectedElement = currentlyMovingElement;
            //currentlySelectedElement.BorderBrush = new SolidColorBrush(Colors.White);
            //stackPanelProperties.IsEnabled = true;
            //textBoxText.Text = (currentlySelectedElement.Child as PhoneUIElement).Text;
            //if (currentlySelectedElement.Child is PhoneUIElementButton)
            //{
            //    textBoxImage.Text = (currentlySelectedElement.Child as PhoneUIElementButton).ImageFile;
            //}
            //}
            //}
            //}
            //else
            //{
            //    if (currentlySelectedElement != null)
            //    {
            //        currentlySelectedElement.BorderBrush = new SolidColorBrush(Colors.Transparent);
            //        currentlySelectedElement = null;
            //        stackPanelProperties.IsEnabled = false;
            //    }
            //}
            //CanSave = (currentlySelectedElement != null);
            //currentlyMovingElement = null;
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
            Debug.WriteLine("Lost focus");
            //this.Focus();
            textBox1.CaretBrush = new SolidColorBrush(Colors.Transparent);
            buttonText.Background = defaultButtonBrush;
            buttonText.UpdateLayout();
            
            textBox1.UpdateLayout();
            textBox1.IsHitTestVisible = false;
            textBox1.BorderThickness = new Thickness(0);
            textBox1.SelectionLength = 0;
        }
    }
}
