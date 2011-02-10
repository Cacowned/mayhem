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
using System.Windows.Shapes;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace MayhemWpf
{
    /// <summary>
    /// Interaction logic for ActionList.xaml
    /// </summary>
    public partial class ActionList : Window
    {
        Mayhem<ICli> mayhem;

        public ActionList(Mayhem<ICli> mayhem)
        {
            InitializeComponent();
            this.mayhem = mayhem;

            ActionsList.ItemsSource = mayhem.ActionList;
        }

        private void ChooseButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
