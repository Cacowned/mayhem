using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MayhemApp.Business_Logic
{
    public class MayhemInterfaceItemListPage
    {
        public string list_title { get; set; }
        public ObservableCollection<object> display_items = new ObservableCollection<object>();
        public MayhemInterfaceItemListPage previousPage { get; set; }
        public Dictionary<object, MayhemInterfaceItemListPage> subPages = new Dictionary<object, MayhemInterfaceItemListPage>();


        public MayhemInterfaceItemListPage()
        {

        }

        public MayhemInterfaceItemListPage(string title)
        {
            list_title = title;
        }

        public MayhemInterfaceItemListPage(MayhemInterfaceItemListPage parent, string title)
        {
            previousPage = parent;
            list_title = title;
        }

        public void Add_Item(object item)
        {
            display_items.Add(item);
            subPages[item] = null;
        }



        public void Add_SubPage_To_Item(object item, MayhemInterfaceItemListPage page)
        {
            page.previousPage = this;
            subPages[item] = page;
        }


        /**<summary>
         * Finds a subpage containing a certain item
         * 
         * Used to support drag and drop
         * </summary>
         * */
        public MayhemInterfaceItemListPage Find_Page_Containing_Item(object item)
        {
            // first look at all the display items
            // if they don't contain the item, look at the subpages

            foreach (object o in display_items)
            {
                if (o == item)
                    return this;
            }

            foreach (MayhemInterfaceItemListPage p in subPages.Values)
            {
                if (p != null)
                {
                    if (p.Find_Page_Containing_Item(item) != null)
                        return p;
                }
                else
                {
                    return null;
                }
            }

            return null;

        }

        /**<summary>
         * Replaces an item on the display
         * 
         * Usually used to save new instances of MayhemConnections
         * </summary>
         * */
        public bool Replace_Display_Item(object original_item, object replacement_item)
        {
            // TODO: sanity checks
            int index = display_items.IndexOf(original_item);

            if (index > -1)
            {
                display_items.RemoveAt(index);
                if (replacement_item is MayhemConnectionItem)
                {
                    display_items.Insert(index, ((MayhemConnectionItem)replacement_item).template_data);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
