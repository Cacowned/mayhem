using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MayhemApp
{
    public class Base64Serialize<T>
    {
        public static string filename = "serial.settings";

        public static T Deserialize() {
            /*
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();

            T obj = (T)bFormatter.Deserialize(stream);
            stream.Close();

            return obj;
             */

            
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            T objectToSerialize = (T)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
 
        }

        public static void SerializeObject(T objectToSerialize) {
            /*
            XmlSerializer x = new XmlSerializer(objectToSerialize.GetType());

            Stream stream = File.Open("serial.settings", FileMode.Create);

            x.Serialize(stream, objectToSerialize);
             */

            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
 
        }
    }
}
