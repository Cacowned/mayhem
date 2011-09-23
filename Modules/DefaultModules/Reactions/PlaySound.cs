using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Play Sound", "Plays an audio file when triggered")]
    public class PlaySound : ReactionBase, ICli, IWpfConfigurable
    {
        MPlayer m;

        #region Configuration Properties
        [DataMember]
        private string SoundPath
        {
            get;
            set;
        }
        #endregion

        public override void Perform()
        {
            if (SoundPath != null && File.Exists(SoundPath))
            {
                m.PlayFile(SoundPath);
            }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PlaySound_FileNotFound);
            }
        }

        public override bool Enable()
        {
            m = new MPlayer();
            if (File.Exists(SoundPath))
            {
                return true;
            }
            else
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.PlaySound_FileNotFound);
            }

            return false;
        }

        public override void Disable()
        {
            m.Stop();
            base.Disable();
        }

        #region Configuration Views

        public void CliConfig()
        {
            string TAG = "[Play Sound]";

            string path = "";

            do
            {
                Console.WriteLine(String.Format(CultureInfo.CurrentCulture, Strings.PlaySound_CliConfig_AudioPath, TAG));
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

        public override void SetConfigString()
        {
            ConfigString = String.Format(CultureInfo.CurrentCulture, "{0}", SoundPath);
        }
    }
}
