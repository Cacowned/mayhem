/*
 * ArduinoDigitalWrite.cs
 * 
 * Writes logic values to a set of digital pins on the Arduino.
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using System.Runtime.Serialization;

namespace ArduinoModules.Reactions
{
    [DataContract]
    [MayhemModule("Arduino Digital Write", "Writes logic values to a set of digital pins on the Arduino.")]
    public class ArduinoDigitalWrite : ReactionBase, IWpfConfigurable
    {

        public override void Perform()
        {
            throw new NotImplementedException();
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { throw new NotImplementedException(); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            throw new NotImplementedException();
        }
    }
}
