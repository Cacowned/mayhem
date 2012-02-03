using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Play Sound", "Plays an audio file when triggered")]
    public class PlaySound : ReactionBase, ICli, IWpfConfigurable
    {
        [DataMember]
        private string SoundPath
        {
            get;
            set;
        }

        private MPlayer m;

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

        protected override void OnEnabling(EnablingEventArgs e)
        {
            m = new MPlayer();
            if (!File.Exists(SoundPath))
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.PlaySound_FileNotFound);
                e.Cancel = true;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            m.Stop();
        }

        #region Configuration Views

        public void CliConfig()
        {
            const string Tag = "[Play Sound]";

            string path;

            do
            {
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Strings.PlaySound_CliConfig_AudioPath, Tag));
                path = Console.ReadLine();
            }
            while (!File.Exists(path));

            SoundPath = path;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new PlaySoundConfig(SoundPath); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var rpc = configurationControl as PlaySoundConfig;
            SoundPath = rpc.FileName;
        }

        #endregion

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}", SoundPath);
        }
    }
}
