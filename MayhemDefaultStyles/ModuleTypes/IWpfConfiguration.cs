﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MayhemDefaultStyles.UserControls
{
    public class IWpfConfiguration : UserControl
    {
        public virtual string Title { get { return "Title"; } }
        public virtual bool OnSave() { return true; }
        public virtual void OnCancel() { }
        public virtual void OnLoad() { }
        public virtual void OnClosing() { }
        public delegate void ConfigCanSaveHandler(bool canSave);
        public event ConfigCanSaveHandler CanSavedChanged;
        private bool canSave = true;
        public bool CanSave
        {
            get
            {
                return canSave;
            }
            set
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