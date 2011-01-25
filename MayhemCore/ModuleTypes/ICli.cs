using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemCore.ModuleTypes
{
    /// <summary>
    /// Every module that has configuration
    /// that wants to be accessible from a CLI
    /// application needs to implement this interface
    /// </summary>
    public interface ICli
    {
        void CliConfig();
    }
}
