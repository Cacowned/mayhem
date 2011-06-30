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

            FileStream stream = new FileStream(filename, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
            stream.Position = 0;

            T obj = (T)dcs.ReadObject(reader, false, new ModuleTypeResolver());

            reader.Close();
            
            return obj;
        }

        public static void SerializeObject(T objectToSerialize) {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            FileStream stream = new FileStream(filename, FileMode.Create);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream);

            dcs.WriteObject(writer, objectToSerialize, new ModuleTypeResolver());

            writer.Close();
 
        }
    }
}
