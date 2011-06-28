using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Xml;

// Copied this class from MayhemCli
namespace MayhemWpf
{
    public class Base64Serialize<T>
    {
        public static string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serial.settings");

        public static T Deserialize() {

            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

            T obj = (T)dcs.ReadObject(reader);

            fs.Close();
            return obj;
            
        }

        public static void SerializeObject(T objectToSerialize) {

            FileStream stream = File.Open(filename, FileMode.Create);
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8);
            dcs.WriteObject(xdw, objectToSerialize);

            stream.Close();
 
        }
    }
}
