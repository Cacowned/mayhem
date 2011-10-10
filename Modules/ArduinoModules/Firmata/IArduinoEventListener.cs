/*
 * IArduinoEventListener
 * 
 * Interface for Arduino Events
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */ 
using System;

namespace ArduinoModules.Firmata
{
    public interface IArduinoEventListener
    {
        void Arduino_OnInitialized(object sender, EventArgs e);
        void Arduino_OnAnalogPinChanged(Pin p);
        void Arduino_OnDigitalPinChanged(Pin p);
        void Arduino_OnPinAdded(Pin p);
    }
}
