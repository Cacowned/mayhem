using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Controls;

namespace MayhemApp
{
    /* controls pagination in the mayhem UI */
    [Serializable]
    class MayhemInterfacePaginationController
    {

        // references to the getter/setter functions of the properties list
        // this allows me to keep the dictionary loader unspecific to the property in question
        private Action<String> RUNLIST_PROPERTY_SET = (value => Properties.Settings.Default.RunlistSettings = value);
        private Func<String> RUNLIST_PROPERTY_GET = (() => Properties.Settings.Default.RunlistSettings);


        public const string TAG = "[MayhemInterfacePaginationController] : ";
        public int MAX_ITEMS_PER_PAGE = 8;
        public int currentPage = 0;
        public List<object> allItems = new List<object>();
        public ObservableCollection<object> displayItems = new ObservableCollection<object>();
        public Label pageLabel = null;
        public ItemsControl userControl { get; set; }



        public void AddItem(object b)
        {

            // if object is a runcontrol add the Trashbutton clicked callback

            if (b is RunControl)
            {
                ((RunControl)b).OnTrashButtonClicked += new RunControl.TrashButtonClickHandler(MayhemInterfacePaginationController_OnTrashButtonClicked);
            }



            // add item to global page
            allItems.Insert(0, b);

            // repaginate

            PaginateItems(currentPage);

            UpdatePageLabel();
        }

        void MayhemInterfacePaginationController_OnTrashButtonClicked(object sender, EventArgs e)
        {
            //  throw new NotImplementedException();
            RunControl s = sender as RunControl;

            if (s != null)
            {
                Debug.WriteLine(TAG + "MayhemInterfacePaginationController_OnTrashButtonClicked --> removing item " + s.ToString());
                this.RemoveItem(s);
            }


        }


        private void UpdatePageLabel()
        {
            if (pageLabel != null)
            {
                pageLabel.Content = GetPagePositionString();
            }
        }

        public String GetPagePositionString()
        {
            return "Page " + (currentPage + 1) + " of " + MaxPages();
        }

        public bool RemoveItem(object o)
        {
            bool success = allItems.Remove(o);

            if (success)
            {
                PaginateItems(currentPage);
                UpdatePageLabel();

            }
            return success;
        }

        public void ShowNextPage()
        {

            PaginateItems(currentPage + 1);
            UpdatePageLabel();
        }

        public void ShowPrevPage()
        {
            PaginateItems(currentPage - 1);
            UpdatePageLabel();
        }

        private int MaxPages()
        {
            return (int)Math.Ceiling((double)allItems.Count / MAX_ITEMS_PER_PAGE);
        }

        public void PaginateItems(int page)
        {

            /* sets up the item pagination */
            int maxPages = MaxPages();

            if (page >= 0 && page <= maxPages)
            {
                displayItems = new ObservableCollection<object>();

                int startindex = MAX_ITEMS_PER_PAGE * page;

                int endIndex = 0;
                if (allItems.Count < startindex + MAX_ITEMS_PER_PAGE)
                {
                    endIndex = allItems.Count;
                }
                else
                {
                    endIndex = startindex + MAX_ITEMS_PER_PAGE;
                }


                int maxItems = allItems.Count;


                for (int i = startindex; i < endIndex; i++)
                {
                    Trace.WriteLine("Adding Item...");
                    if (displayItems.Count < MAX_ITEMS_PER_PAGE)
                    {

                        if (i < maxItems)
                            displayItems.Add(allItems[i]);

                    }

                }

                currentPage = page;

            }
            else if (page < 0)
            {
                PaginateItems(0);

                return;
            }
            else if (page > maxPages)
            {
                PaginateItems(maxPages);
            }


            userControl.ItemsSource = displayItems;

        }

        /**<summary>
         * Serializes the entire pagination controller, hopefully with all members, to the settings file
         * </summary>
         */
        public bool SaveToSettingsFile()
        {


            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                bf.Serialize(ms, this);
            }
            catch (SerializationException ex)
            {
                Debug.WriteLine(TAG + "SerializationException");
                Debug.WriteLine(ex);
            }

            string saved_str = System.Convert.ToBase64String(ms.ToArray());

            Debug.WriteLine(saved_str);

            // save myself in the settings file
            Properties.Settings.Default.RunlistSettings = saved_str;

            return true;
        }

        /**<summary>
         * Restores a previous pagination controller from the settings file.
         * </summary>
         * */
        public static MayhemInterfacePaginationController LoadFromSettingsFile()
        {
            byte[] serialized_data = System.Convert.FromBase64String(Properties.Settings.Default.RunlistSettings);


            MemoryStream ms = new MemoryStream(serialized_data);
            BinaryFormatter bf = new BinaryFormatter();
            MayhemInterfacePaginationController theController = null;

            try
            {
                theController = (MayhemInterfacePaginationController)bf.Deserialize(ms);
            }
            catch (SerializationException ex)
            {
                Debug.WriteLine(TAG + "(De-)SerializationException");
                Debug.WriteLine(ex);
            }


            return theController;
        }

    }
}
