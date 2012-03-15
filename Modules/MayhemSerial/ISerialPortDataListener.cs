namespace MayhemSerial
{
    public interface ISerialPortDataListener
    {
          void DataReceived(string portName, byte[] buffer, int nBytes);
    }
}
