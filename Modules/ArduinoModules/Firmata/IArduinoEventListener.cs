using System;

namespace ArduinoModules.Firmata
{
    /// <summary>
    /// Interface for Arduino Events
    /// </summary>
    public interface IArduinoEventListener
    {
        void Arduino_OnInitialized(object sender, EventArgs e);

        void Arduino_OnAnalogPinChanged(Pin p);

        void Arduino_OnDigitalPinChanged(Pin p);

        void Arduino_OnPinAdded(Pin p);
    }
}
