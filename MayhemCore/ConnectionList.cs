using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace MayhemCore
{
	/// <summary>
	/// Maintains a set of all the current connections
	/// </summary>
	public class ConnectionList : ObservableCollection<Connection>
	{
        public static ConnectionList Deserialize(Stream stream, ICollection<Type> types)
        {
            ConnectionList obj = null;
            DataContractSerializer dcs = new DataContractSerializer(typeof(ConnectionList));

            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max))
            {
                obj = dcs.ReadObject(reader, false, new ModuleTypeResolver(types)) as ConnectionList;
            }

            return obj;
        }

        public void Serialize(Stream stream)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(ConnectionList));

            using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream))
            {
                dcs.WriteObject(writer, this, new ModuleTypeResolver());
            }
        }
	}
}
