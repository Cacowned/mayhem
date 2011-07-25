
using System;
namespace MayhemCore.ModuleTypes
{
    /// <summary>
    /// Every module that has configuration
    /// that wants to be accessible from a Windows Phone 7
    /// application needs to implement this interface
    /// </summary>
    public interface IWp7
    {
        Uri Wp7Config();
    }
}
