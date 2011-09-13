using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace MayhemSerial
{
    public interface ISerialPortDataListener
    {
          void port_DataReceived(string portName, byte[] buffer, int nBytes);
    }
}
