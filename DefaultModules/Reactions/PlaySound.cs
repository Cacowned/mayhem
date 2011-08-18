using System;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Play Sound", "Plays an audio file when triggered")]
    public class PlaySound : ReactionBase, ICli, IWpfConfigurable
    {
        protected const string TAG = "[PlaySound]";

        MPlayer m;

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

        protected override void Initialize()
        {
            base.Initialize();
        }


        public override void Perform()
        {
            if (SoundPath != null)
            {
                
                m.PlayFile(SoundPath);
            }
        }

        public override void Enable()
        {
            m = new MPlayer();
            base.Enable();
        }

        public override void Disable()
        {
            m.Stop();
            base.Disable();
        }

        #region Configuration Views

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

        public IWpfConfiguration ConfigurationControl
        {
            get { return new PlaySoundConfig(SoundPath); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            PlaySoundConfig rpc = configurationControl as PlaySoundConfig;
            SoundPath = rpc.FileName;
        }

        #endregion
    }
}
