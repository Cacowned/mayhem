using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Reactions
{
    [Serializable]
    public class PlaySound : ReactionBase, ICli
    {

        protected const string TAG = "[PlaySound]";
        protected string path_to_sound = null;
        
        object locker = new object();

        protected int _media_playing = 0;
        private int MediaPlaying
        {
            get
            {
                lock (locker)
                {
                    return _media_playing;
                }

            }

            set
            {
                lock (locker)
                {
                    _media_playing = value;
                }
            }

        }

        public PlaySound()
            : base("Play Sound", "Plays an audio file when triggered.") {
            Setup();
        }

        public void Setup() {
            hasConfig = true;
        }
        public override void Perform() {
            if (path_to_sound != null)
            {
               // media_playing++;
                MPlayer m = new MPlayer();
                m.PlayFile(path_to_sound);
            }
        }

        public void CliConfig() {
            string path = "";

            do {
                Console.WriteLine(String.Format("{0} Enter the path for the audio file", TAG));
                path = Console.ReadLine();
            }
            while (!File.Exists(path));

            path_to_sound = path;
        }

        #region Serialization
        public PlaySound(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Setup();

            string path = info.GetString("soundFile");
            // test if this is a valid path, else set soundfile to null
            if (File.Exists(path)) {
                path_to_sound = path;
            } else {
                path_to_sound = null;

                // TODO: decide to display warning (or not) 
            }
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("soundFile", path_to_sound);
        }
        #endregion
    }
}
