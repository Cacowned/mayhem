using System.Collections.Generic;
using System.IO;

namespace PhoneModules
{
    public static class FileDictionary
    {
        private static readonly Dictionary<string, byte[]> dict;

        static FileDictionary()
        {
            dict = new Dictionary<string, byte[]>();
        }

        public static void Add(string str)
        {
            if (!dict.ContainsKey(str))
            {
                byte[] bytes;
                using (FileStream fs = new FileStream(str, FileMode.Open))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                }

                dict[str] = bytes;
            }
        }

        public static byte[] Get(string str)
        {
            if (!dict.ContainsKey(str))
            {
                Add(str);
            }

            return dict[str];
        }
    }
}
