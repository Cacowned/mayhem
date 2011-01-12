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
        public static  T DeserializeFromString(string s)
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









    /**
     * <summary>
     * This class is used to serializ/desirialize more complex settings strings in the app settings
     * </summary>
     * 
     */ 
    public class MayhemSettingsDictionaryLoader
    {

        public static string TAG="[MayhemSettingsDictionaryLoader] : ";

        public static Dictionary<string, string> LoadDictionaryWithKey(Func<string> PROPERTY_GET)
        {
            StringReader sr = new StringReader( PROPERTY_GET() );

            Dictionary<string, string> dict = new Dictionary<string,string>();

            try
            {
                Deserialize(sr, dict);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(TAG + "Deserialize Exception");
                Debug.WriteLine(TAG + ex);
                return null;
            }
            
            return dict;
        }

        public static bool SaveDictionaryWithKey(Dictionary<string, string> dict, Action<string> PROPERTY_MODIFY)
        {
            StringBuilder sb = new StringBuilder(null);
            StringWriter sw = new StringWriter(sb);
            try
            {
                Serialize(sw, dict);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(TAG + "Serialize Exception");
                Debug.WriteLine(TAG + ex);
                return false;
            }
            string tempString = sb.ToString();

            // set the property
            PROPERTY_MODIFY( tempString );

            

            Properties.Settings.Default.Save();
            Debug.WriteLine("New Twitter Settings: ");
            Debug.WriteLine(Properties.Settings.Default.TwitterSettings);
            return true;
        }


        public static void Serialize(TextWriter writer, IDictionary dictionary)
        {

            List<Entry> entries = new List<Entry>(dictionary.Count);
            foreach (object key in dictionary.Keys)
            {
                entries.Add(new Entry(key, dictionary[key]));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            serializer.Serialize(writer, entries);

        }



        public static void Deserialize(TextReader reader, IDictionary dictionary)
        {

            dictionary.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            List<Entry> list = (List<Entry>)serializer.Deserialize(reader);
            foreach (Entry entry in list)
            {

                dictionary[entry.Key] = entry.Value;

            }

        }



        public class Entry
        {
            public object Key;
            public object Value;
            public Entry() { }
            public Entry(object key, object value)
            {
               Key = key; Value = value;
            }

        }

    }
}
