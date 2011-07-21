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
        public static string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
        
        public static T Deserialize(List<Type> types) 
        {
            T obj = default(T);
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream,XmlDictionaryReaderQuotas.Max))
                {
                    obj = (T)dcs.ReadObject(reader, false, new ModuleTypeResolver(types));
                }
            }
            
            return obj;
        }

        public static void SerializeObject(T objectToSerialize) 
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream))
                {
                    dcs.WriteObject(writer, objectToSerialize, new ModuleTypeResolver());
                }
            }
        }
    }
}
