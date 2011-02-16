﻿using System;
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
using Microsoft.Win32;

namespace DefaultModules.Wpf
{
    public partial class RunProgramConfig : Window
    {
        public string filename;
        public string arguments;

        public RunProgramConfig(string filename, string arguments) {
            InitializeComponent();
            LocationBox.Text = filename;
            ArgumentsBox.Text = arguments;
        }
        
        // Browse for file
        private void Button_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = filename;
            dlg.DefaultExt = ".exe";

            if ((bool)dlg.ShowDialog()) {
                filename = dlg.FileName;
                LocationBox.Text = filename;
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
