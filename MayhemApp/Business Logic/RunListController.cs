using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MayhemApp.Business_Logic
{
    [Serializable]
    class RunListController : MayhemInterfacePaginationController
    {

        private Dictionary<string, string> runListSettings = null;


        // references to the getter/setter functions of the properties list
        // this allows me to keep the dictionary loader unspecific to the property in question
        // TODO: clarify the above comment LOL
        private Action<String> RUNLIST_PROPERTY_SET = (value => Properties.Settings.Default.RunlistSettings = value);
        private Func<String> RUNLIST_PROPERTY_GET = (() => Properties.Settings.Default.RunlistSettings);




        /**<summary>
         * Saves items in the runlist to the settings file entry.
         * </summary>
         * */
        private bool SaveItemsToSettings()
        {
            // TODO

            if (runListSettings != null)
            {

                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(this.allItems.GetType());

                StringBuilder sb = new StringBuilder(null);
                StringWriter sw = new StringWriter(sb);
                try
                {
                    xs.Serialize(sw, allItems);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(TAG + "Serialize Exception");
                    Debug.WriteLine(TAG + ex);
                    return false;
                }
                string tempString = sb.ToString();
                Debug.WriteLine(TAG + "Serialized Entries: " + tempString);


                // finish the dictionary
            }
            return false;
        }

        /**<summary>
         * Builds runlist from settings file entry.
         * </summary>
         * */
        private bool LoadItemsFromSettings()
        {
            return false;
        }

        public void PopulateRunList()
        {
            if (!LoadItemsFromSettings())
            {
                runListSettings = new Dictionary<string, string>();
            }
        }
    }
}
