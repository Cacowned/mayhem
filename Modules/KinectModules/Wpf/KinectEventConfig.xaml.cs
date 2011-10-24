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
using MayhemWpf.UserControls;
using Microsoft.Research.Kinect.Nui;
using KinectModules.Helpers;
using System.Collections;
using DTWGestureRecognition;
using MayhemCore;
using System.Threading;
using System.Collections.ObjectModel;

namespace KinectModules.Wpf
{
    class GestureListItem
    {
        public string gestureName;
        public bool selected; 

        public GestureListItem(string gestureName, bool selected)
        {
            this.gestureName = gestureName;
            this.selected = selected;
        }       
    }
    /// <summary>
    /// Interaction logic for KinectEventConfig.xaml
    /// </summary>
    public partial class KinectEventConfig : WpfConfiguration
    {

        private ObservableCollection<ListViewItem> listboxItems; 

        /// <summary>
        /// The minumum number of frames in the _video buffer before we attempt to start matching gestures
        /// </summary>
        private const int MinimumFrames = 6;

        /// <summary>
        /// How many skeleton frames to store in the _video buffer
        /// </summary>
        private const int BufferSize = 32;

        /// <summary>
        /// How many skeleton frames to ignore (_flipFlop)
        /// 1 = capture every frame, 2 = capture every second frame etc.
        /// </summary>
        private const int Ignore = 2;

        /// <summary>
        /// Switch used to ignore certain skeleton frames
        /// </summary>
        private int _flipFlop;
        private ArrayList _video;
        private DtwGestureRecognizer _dtw;

        // private Runtime nui;
        private MayhemKinect kinect;
        // handler for rendering of skeleton frames
        private EventHandler<SkeletonFrameReadyEventArgs> skeletonRenderFrameHandler;
        // handler for recognition of skeleton gestures
        private EventHandler<SkeletonFrameReadyEventArgs> skeletonRecogFrameHandler; 

        Dictionary<JointID, Brush> jointColors = new Dictionary<JointID, Brush>() { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };

        private List<string> _selectedGestures;
        public List<string> SelectedGestures
        {
            get
            {
                List<string> listOut = new List<string>();
                foreach (ListViewItem s in lbGestures.SelectedItems)
                {
                    listOut.Add(s.Content as string);
                }
                return listOut;
            }
        }

        public KinectEventConfig(List<string> selectedGestures)
        {
            InitializeComponent();
            this._selectedGestures = selectedGestures;
        }

        public override string Title
        {
            get
            {
               return "Kinect Gesture";
            }
        }

        public override void OnLoad()
        {
            kinect = MayhemKinect.Instance;
            skeletonRenderFrameHandler = new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            kinect.AttachSkeletonEventHandler(skeletonRenderFrameHandler);

            skeletonRecogFrameHandler = new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonExtractSkeletonFrameReady);
            kinect.AttachSkeletonEventHandler(skeletonRecogFrameHandler);

            _video = new ArrayList();
            _dtw = new DtwGestureRecognizer(12, 0.6, 2, 2, 10);
            _flipFlop = 0;

            kinect = MayhemKinect.Instance;
            skeletonRenderFrameHandler = new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonExtractSkeletonFrameReady);

            // skeleton data extractor
            Skeleton2DDataExtract.Skeleton2DdataCoordReady += NuiSkeleton2DdataCoordReady;

            // read gesture file
            if (_dtw.LoadGesturesFromFile(KinectSkeletalGesture.GestureFileLocation)) 
            {
                CanSave = true;
                listboxItems = new ObservableCollection<ListViewItem>();
                //listboxItems.Add("All");
                // get gesture labels from dtw

                lbGestures.ItemsSource = listboxItems;

                foreach (string s in _dtw.Labels)
                {
                    ListViewItem l = new ListViewItem();
                    l.Content = s;
                    if (_selectedGestures.Contains(s))
                    {
                        l.IsSelected = true;
                    }                 
                    listboxItems.Add(l);
                }
                lbGestures.Focus();
            }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, "Could not read gesture definition file!");
            }
        }

      

        private Point getDisplayPosition(Joint joint)
        {
            float depthX, depthY;
            kinect.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = depthX * 320; //convert to 320, 240 space
            depthY = depthY * 240; //convert to 320, 240 space
            int colorX, colorY;
            ImageViewArea iv = new ImageViewArea();
            // only ImageResolution.Resolution640x480 is supported at this point
            kinect.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

            // map back to skeleton.Width & skeleton.Height
            return new Point((int)(skeleton.Width * colorX / 640.0), (int)(skeleton.Height * colorY / 480));
        }

        Polyline getBodySegment(Microsoft.Research.Kinect.Nui.JointsCollection joints, Brush brush, params JointID[] ids)
        {
            PointCollection points = new PointCollection(ids.Length);
            for (int i = 0; i < ids.Length; ++i)
            {
                points.Add(getDisplayPosition(joints[ids[i]]));
            }

            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            int iSkeleton = 0;
            Brush[] brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            skeleton.Children.Clear();
            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    // Draw bones
                    Brush brush = brushes[iSkeleton % brushes.Length];
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

                    // Draw joints
                    foreach (Joint joint in data.Joints)
                    {
                        Point jointPos = getDisplayPosition(joint);
                        Line jointLine = new Line();
                        jointLine.X1 = jointPos.X - 3;
                        jointLine.X2 = jointLine.X1 + 6;
                        jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                        jointLine.Stroke = jointColors[joint.ID];
                        jointLine.StrokeThickness = 6;
                        skeleton.Children.Add(jointLine);
                    }
                }
                iSkeleton++;
            } // for each skeleton
        }

        /// <summary>
        /// The only thing that needs to be done here is to detach the skeleton event handlers
        /// </summary>
        public override void  OnClosing()
        {
            kinect.DetachSkeletonEventHandler(skeletonRenderFrameHandler);
            kinect.DetachSkeletonEventHandler(skeletonRecogFrameHandler);
        }

        /// <summary>
        /// Called each time a skeleton frame is ready. Passes skeletal data to the DTW processor
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Skeleton Frame Ready Event Args</param>
        private static void SkeletonExtractSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                Skeleton2DDataExtract.ProcessData(data);
            }
        }

        /// <summary>
        /// Runs every time our 2D coordinates are ready.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="a">Skeleton 2Ddata Coord Event Args</param>
        private void NuiSkeleton2DdataCoordReady(object sender, Skeleton2DdataCoordEventArgs a)
        {


            // We need a sensible number of frames before we start attempting to match gestures against remembered sequences
            if (_video.Count > MinimumFrames)
            {
                ////Debug.WriteLine("Reading and video.Count=" + video.Count);
                string s = _dtw.Recognize(_video);
                //results.Text = "Recognised as: " + s;

                if (!s.Contains("__UNKNOWN"))
                {
                    Logger.WriteLine("Recognized as: " + s);

                    // There was no match so reset the buffer
                    _video = new ArrayList();

                    lblGestureDetected.Content = s;

                    //Label recognized = new Label();
                    //recognized.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    //recognized.Content = s;

                    //Canvas.SetLeft(recognized, 100);
                    //Canvas.SetTop(recognized, 200);
                    //Canvas.SetZIndex(recognized, 99);


                    //skeleton.Children.Add(recognized);

                     //Timer tm = new Timer((o) => { Dispatcher.BeginInvoke(new Action(() => { lblGestureDetected.Content = "none"; }));},
                     //                       null,
                     //                       3000,
                     //                       Timeout.Infinite);
                }
          
            }

            // Ensures that we remember only the last x frames
            if (_video.Count > BufferSize)
            {

                // Remove the first frame in the buffer
                _video.RemoveAt(0);

            }

            // Decide which skeleton frames to capture. Only do so if the frames actually returned a number. 
            // For some reason my Kinect/PC setup didn't always return a double in range (i.e. infinity) even when standing completely within the frame.
            // TODO Weird. Need to investigate this
            if (!double.IsNaN(a.GetPoint(0).X))
            {
                // Optionally register only 1 frame out of every n
                _flipFlop = (_flipFlop + 1) % Ignore;
                if (_flipFlop == 0)
                {
                    _video.Add(a.GetCoords());
                }
            }

            // Update the debug window with Sequences information
            //dtwTextOutput.Text = _dtw.RetrieveText();
        }
        
    }
}
