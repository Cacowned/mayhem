using System;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Reactions
{
    [MayhemModule("Play Sound", "Plays an audio file when triggered")]
    public class PlaySound : ReactionBase, ICli // TODO: Make WPF Compatible
    {
        protected const string TAG = "[PlaySound]";

        #region Configuration Properties
        private string _soundPath;
        [DataMember]
        private string SoundPath
        {
            get
            {
                return _soundPath;
            }
            set
            {
                if (File.Exists(value))
                {
                    _soundPath = value;
                }
                else
                {
                    _soundPath = null;
                    ErrorLog.AddError(ErrorType.Warning, "The sound file does not exist");
                }
            }
        }
        #endregion

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
        { }

        protected override void Initialize()
        {
            base.Initialize();
        }


        public override void Perform()
        {
            if (SoundPath != null)
            {
                // media_playing++;
                MPlayer m = new MPlayer();
                m.PlayFile(SoundPath);
            }
        }

        public void CliConfig()
        {
            string path = "";

            do
            {
                Console.WriteLine(String.Format("{0} Enter the path for the audio file", TAG));
                path = Console.ReadLine();
            }
            while (!File.Exists(path));

            SoundPath = path;
        }
    }
}
