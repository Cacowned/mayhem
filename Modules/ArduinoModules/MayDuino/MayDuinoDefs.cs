using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.MayDuino
{
    /// <summary>
    /// Event Types  -- keep in sync with code running on Arduino
    /// </summary>
    public enum Event_t
    {
        EVENT_DISABLED,
        DIGITALEVENT,
        ANALOGEVENT
    } ; 

    /// <summary>
    /// Reaction Types   -- keep in sync with code running on Arduino
    /// </summary>
    public enum Reaction_t
    {
     REACTION_DISABLED,
     DIGITALREACTION,
     ANALOGREACTION,
     SERVOREACTION
    } ;

    /// <summary>
    /// Mayduino Commands -- keep in sync with code running on Arduino
    /// </summary>
    public struct MayduinoCommands
    {
        public static string NEW_CONNECTION = "$c,";
        public static string RESET_CONNECTIONS = "$r";
        public static string WRITE_CONNECTIONS = "$w";
    }
}
