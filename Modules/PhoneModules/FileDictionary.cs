using System.Collections.Generic;
using System.IO;

namespace PhoneModules
{
    public static class FileDictionary
    {
        private static readonly Dictionary<string, byte[]> Dictionary;

        static FileDictionary()
        {
            Dictionary = new Dictionary<string, byte[]>();
        }

        public static void Add(string str)
        {
            if (!Dictionary.ContainsKey(str))
            {
                byte[] bytes;
                using (FileStream fs = new FileStream(str, FileMode.Open))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                }

                Dictionary[str] = bytes;
            }
        }

        public static byte[] Get(string str)
        {
            if (!Dictionary.ContainsKey(str))
            {
                Add(str);
            }

            return Dictionary[str];
        }
    }
}
