﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace MayhemCore
{
    /// <summary>
    /// Maintains a set of all the current connections
    /// </summary>
    internal class ConnectionList : ObservableCollection<Connection>
    {
        internal static ConnectionList Deserialize(Stream stream, ICollection<Type> types)
        {
            ConnectionList obj;
            DataContractSerializer dcs = new DataContractSerializer(typeof(ConnectionList));

            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max))
            {
                obj = dcs.ReadObject(reader, false, new ModuleTypeResolver(types)) as ConnectionList;
            }

            return obj;
        }

        internal void Serialize(Stream stream)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(ConnectionList));

            using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(stream))
            {
                dcs.WriteObject(writer, this, new ModuleTypeResolver());
            }
        }
    }
}
