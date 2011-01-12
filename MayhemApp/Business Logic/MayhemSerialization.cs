using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;


// TODO: Move to Low-Level Namespace

namespace MayhemApp.Business_Logic
{

    public class Base64Serialize<T>
    {
        public static T DeserializeFromString(string s)
        {

            byte[] bytes = Convert.FromBase64String(s);
            MemoryStream memStr = new MemoryStream(bytes);

            T result = default(T);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                result = (T)bf.Deserialize(memStr);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Deserialize Exception " + e);
            }
            finally
            {
                memStr.Close();
            }

            return result;

        }

        public static string SerializeToString(T objectToSerialize)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memStr = new MemoryStream();

            try
            {
                bf.Serialize(memStr, objectToSerialize);
                memStr.Position = 0;

                return Convert.ToBase64String(memStr.ToArray());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Serialize Exception " + e);
                return null;
            }
            finally
            {
                memStr.Close();
            }
        }
    }
}
