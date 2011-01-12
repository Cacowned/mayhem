using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

namespace MayhemApp.Business_Logic
{
    /**
     * <summary>
     * This class is used to serialize/desirialize more complex settings strings in the app settings
     * </summary>
     * 
     */
    public class MayhemSettingsDictionaryLoader
    {

        public static string TAG = "[MayhemSettingsDictionaryLoader] : ";

        public static Dictionary<string, string> LoadDictionaryWithKey(Func<string> PROPERTY_GET)
        {
            StringReader sr = new StringReader(PROPERTY_GET());

            Dictionary<string, string> dict = new Dictionary<string, string>();

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
            PROPERTY_MODIFY(tempString);



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
