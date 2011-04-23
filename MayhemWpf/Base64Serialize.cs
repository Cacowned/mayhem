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

// Copied this class from MayhemCli
namespace MayhemWpf
{
    public class Base64Serialize<T>
    {
        public static string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serial.settings");

        public static T Deserialize() {

            byte[] file = File.ReadAllBytes(filename);
            MemoryStream stream = new MemoryStream(file);
            BinaryFormatter formatter = new BinaryFormatter();


            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            formatter.Binder = new VersionConfigToNamespaceAssemblyObjectBinder();
            T obj = (T)formatter.Deserialize(stream);
            return obj;
            
        }

        public static void SerializeObject(T objectToSerialize) {

            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            formatter.Serialize(stream, objectToSerialize);
            stream.Close();
 
        }
    }
    internal sealed class VersionConfigToNamespaceAssemblyObjectBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                try
                {

                    string ToAssemblyName = assemblyName.Split(',')[0];
                    Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly ass in Assemblies)
                    {
                        if (ass.FullName.Split(',')[0] == ToAssemblyName)
                        {
                            typeToDeserialize = ass.GetType(typeName);
                            break;
                        }
                    }

                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
                finally
                {
                }
                return typeToDeserialize;
            }
    }




}
