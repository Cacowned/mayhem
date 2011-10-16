﻿using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class OpenUrlConfig : WpfConfiguration
    {
        public string Url
        {
            get;
            private set;
        }

        public OpenUrlConfig(string url)
        {
            this.Url = url;

            InitializeComponent();
        }

        public override string Title
        {
            get { return "Open Website"; }
        }

        public override void OnLoad()
        {
            UrlBox.Text = Url;
        }

        public override void OnSave()
        {
            Url = UrlBox.Text;
        }

        private void CheckValidity()
        {
            CanSave = UrlBox.Text.Length > 0;
        }
        
        private void UrlBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
