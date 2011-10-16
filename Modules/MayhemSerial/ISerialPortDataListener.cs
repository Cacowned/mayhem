namespace MayhemSerial
{
    public interface ISerialPortDataListener
    {
          void port_DataReceived(string portName, byte[] buffer, int nBytes);
    }
}
