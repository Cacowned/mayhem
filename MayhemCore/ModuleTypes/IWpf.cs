using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;

namespace MayhemCore.ModuleTypes
{
    /// <summary>
    /// Every module that has configuration
    /// that wants to be accessible from a WPF
    /// application needs to implement this interface
    /// </summary>
    public interface IWpf : ISerializable
    {
        void WpfConfig();

        //void WpfConfig(Window window);
    }
}
