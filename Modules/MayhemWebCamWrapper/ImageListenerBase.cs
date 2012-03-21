using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace MayhemWebCamWrapper
{
    /// <summary>
    /// A dummy class acting as a camera listener. 
    /// Fake Camera Listener used by Picture and Vidoe to keep subscribed to camera updates
    /// </summary>
    public class ImageListenerBase : System.Windows.Controls.Image, INotifyPropertyChanged
    {
        protected ImagerBase.ImageUpdateHandler ImageUpdateHandler
        {
            get;
            set;
        }

        /// <summary>
        /// The frame update function
        /// Subclasses can override this method to implement their specific functionality when
        /// receiving a frame update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void UpdateFrame(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Register and unregister for image callbacks
        /// </summary> 
        /// <param name="c">ImagerBase to register</param>
        public void RegisterForImages(ImagerBase c)
        {
            if (c != null)
            {
                c.OnImageUpdated -= ImageUpdateHandler;
                c.OnImageUpdated += ImageUpdateHandler;
                ImagerWidth = c.Width;
                ImagerHeight = c.Height;
                HasImageSource = true;
                SubscribedImagers.Add(c);
                c.Subsribers.Add(this);
            }
        }

        /// <summary>
        /// Deregister from image callbacks
        /// </summary>
        /// <param name="c">ImagerBase to unregister</param>
        public void UnregisterForImages(ImagerBase c)
        {
            if (c != null)
            {
                c.OnImageUpdated -= ImageUpdateHandler;
                ImagerWidth = 0;
                ImagerHeight = 0;
                HasImageSource = false;
                SubscribedImagers.Remove(c);
                c.Subsribers.Remove(this);
            }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string property)
        {
        }

        /// <summary>
        /// A flag that helps know whether an imager base has been registered to this listener or not
        /// </summary>
        public bool HasImageSource
        {
            get { return _hasImageSource; }
            set { _hasImageSource = (bool)value;  NotifyPropertyChanged("SourceChanged");}
        }
        public static readonly DependencyProperty HasImageSourceProperty =
        DependencyProperty.Register("Has Source", typeof(bool), typeof(ImageListenerBase), new UIPropertyMetadata(default(bool)));
        
        /// <summary>
        /// Image dimensions of the images returned by the imagerbase to which this listener is registered to...
        /// </summary>
        public double ImagerWidth
        {
            get { return _imagerWidth; }
            set { _imagerWidth = (double)value; /*NotifyPropertyChanged("ImagerWidth");*/ }
        }
        public static readonly DependencyProperty ImagerWidthProperty =
        DependencyProperty.Register("Imager Width", typeof(double), typeof(ImageListenerBase), new UIPropertyMetadata(default(double)));

        public double ImagerHeight
        {
            get { return _imagerHeight; }
            set { _imagerHeight = (double)value; /*NotifyPropertyChanged("ImagerHeight");*/ }
        }
        public static readonly DependencyProperty ImagerHeightProperty =
        DependencyProperty.Register("Imager Height", typeof(double), typeof(ImageListenerBase), new UIPropertyMetadata(default(double)));

        public System.Collections.Generic.List<ImagerBase> SubscribedImagers;

        private bool _hasImageSource;
        private double _imagerWidth;
        private double _imagerHeight;
        
    }
}
