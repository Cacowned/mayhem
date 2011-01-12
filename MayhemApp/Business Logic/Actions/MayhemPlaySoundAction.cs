using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemApp.Low_Level;
using System.Windows.Forms;
using System.Windows.Media;
using System.Diagnostics;

namespace MayhemApp.Business_Logic.Actions
{

    /**<summary>
     * This action plays a sound when it is triggered
     * </summary>
     * */
    [Serializable]
    class MayhemPlaySoundAction : MayhemAction, ISerializable, IMayhemConnectionItemCommon
    {


        public const string TAG = "[MayhemPlaySoundAction] : ";

        private string path_to_sound = null;

       // MPlayer mediaPlayer = new MPlayer();
        //MediaPlayer mediaPlayer = new MediaPlayer();

      
        private int media_playing_ = 0;

        // this needs some locking, as concurrency comes into play here
        private int media_playing
        {
            get
            {
                lock (locker)
                {
                    return media_playing_;
                }

            }

            set
            {
                lock (locker)
                {
                    media_playing_ = value;
                }
            }

        }


        object locker = new object();

        #region Constructors

         /**<summary>
         * 
         * Makes a base-compatible constructor.
         * Ignores the input string!
         * </summary>
         **/
        public MayhemPlaySoundAction(string s) : this() { }

        public MayhemPlaySoundAction()
            : base("Play Sound",
                   "Plays a sound when triggered",
                   "This Action plays a sound when it is triggered. Double click to select the sound file to play.")
        {
            /*
            mediaPlayer.MediaOpened += new EventHandler(mediaPlayer_MediaOpened);
            mediaPlayer.MediaEnded += new EventHandler(mediaPlayer_MediaEnded);
            mediaPlayer.MediaFailed += new EventHandler<ExceptionEventArgs>(mediaPlayer_MediaFailed);*/
        }

        #endregion

        #region media player events
        //void mediaPlayer_MediaFailed(object sender, ExceptionEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    Debug.WriteLine(TAG + "media playback failed");
        //}

        
        //void mediaPlayer_MediaEnded(object sender, EventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    Debug.WriteLine(TAG + "playback ended " + media_playing +  " " + mediaPlayer.Position );
        //    mediaPlayer.Stop();
        //    media_playing--;
        //    mediaPlayer.Position = TimeSpan.Zero;

        //}

        //void mediaPlayer_MediaOpened(object sender, EventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    media_open = true;
        //}

        #endregion



        #region IMayhemConnectionItemCommon

        /**<summary>
         * Show a file selection dialog, so that the user can choose the sound file for playback.
         * </summary>
         * */
        public override void OnDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //throw new NotImplementedException();

            System.Windows.Forms.OpenFileDialog fDialog = new System.Windows.Forms.OpenFileDialog();
            fDialog.Title = "Please choose the sound file you want the action to play";
            fDialog.Filter = "Sound Files|*.mp3|*.wav|*.wma"; // TODO really add all that the framework can play back
            fDialog.InitialDirectory = Environment.SpecialFolder.Personal.ToString();
            fDialog.CheckFileExists = true;
            fDialog.CheckPathExists = true;


            MPlayer m = new MPlayer();


            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                path_to_sound = fDialog.FileName;
            }
            else
            {
                MessageBox.Show("The file you selected is not valid.", "File Invalid", MessageBoxButtons.OK);
            }

            try
            {
               // mediaPlayer.Open(new Uri(path_to_sound, UriKind.Absolute));
                //m.PlayFile(path_to_sound);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(TAG + "caught exception while opening media file " + ex);
            }

        }

        #endregion

        #region MayhemAction override
        /** <summary>
       *  This is the actual action that gets executed -- plays the sound!
       * </summary>
       * 
       */
        public override void PerformAction(MayhemTrigger sender)
        {
            if (path_to_sound != null)
            {
               // media_playing++;
                MPlayer m = new MPlayer();
                m.PlayFile(path_to_sound);
                Debug.WriteLine(TAG + "media playback started ");// + media_playing + " " + mediaPlayer.Position);
            }
        }
        #endregion



        #region Serialization

        public MayhemPlaySoundAction(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            string path = info.GetString("soundFile");
            // test if this is a valid path, else set soundfile to null
            if (System.IO.File.Exists(path))
            {
                path_to_sound = path;
            }
            else
            {
                path_to_sound = null;

                // TODO: decide to display warning (or not) 
            }

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            base.GetObjectData(info, context);
            info.AddValue("soundFile", path_to_sound);

        }

        #endregion
    }
}
