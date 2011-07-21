using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace MayhemCore.ModuleTypes
{
    /// <summary>
    /// Every module that has configuration
    /// that wants to be accessible from a Windows Phone 7
    /// application needs to implement this interface
    /// </summary>
    public interface IWp7
    {
        /// <summary>
        /// Called when a module should be configured
        /// </summary>
        /// <returns>A URI path to the page that should be displayed</returns>
        Uri Wp7Config();
    }
}
