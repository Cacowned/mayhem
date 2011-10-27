using System.Windows.Controls;

namespace MayhemWpf.UserControls
{
    /// <summary>
    /// The parent type for all configuration controls for the Mayhem Wpf Application
    /// </summary>
    public class WpfConfiguration : UserControl
    {
        /// <summary>
        /// The title of the configuration window
        /// </summary>
        public virtual string Title
        {
            get
            {
                return "Title";
            }
        }

        /// <summary>
        /// Called when the configuration save button is clicked
        /// </summary>
        public virtual void OnSave()
        {
        }

        /// <summary>
        /// Called when the configuration cancel button is clicked
        /// </summary>
        public virtual void OnCancel()
        {
        }

        /// <summary>
        /// This is called when the configuration window is loaded.
        /// </summary>
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// This is called when the configuration window is closing down
        /// </summary>
        public virtual void OnClosing()
        {
        }

        public delegate void ConfigCanSaveHandler(bool canSave);

        public event ConfigCanSaveHandler CanSavedChanged;

        // When the configuration windows open, you can't save without changing fields.
        private bool canSave = false;

        /// <summary>
        /// Flag for whether the configuration window is in a state that can be saved.
        /// If this is true then save button is enabled, if false then the save button is disabled.
        /// </summary>
        public bool CanSave
        {
            get
            {
                return canSave;
            }
            set
            {
                // Only do this if the flag is being switched
                if (value != canSave)
                {
                    canSave = value;

                    if (CanSavedChanged != null)
                    {
                        CanSavedChanged(canSave);
                    }
                }
            }
        }
    }
}
