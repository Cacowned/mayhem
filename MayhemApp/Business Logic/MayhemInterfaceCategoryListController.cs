using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows;

namespace MayhemApp.Business_Logic
{

    public class MayhemInterfaceItemListPage
    {
        public string list_title {get;set;}
        public ObservableCollection<object> display_items = new ObservableCollection<object>();
        public MayhemInterfaceItemListPage previousPage { get; set; }
        public Dictionary<object, MayhemInterfaceItemListPage> subPages = new  Dictionary<object, MayhemInterfaceItemListPage>();


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
                    display_items.Insert(index, ((MayhemConnectionItem) replacement_item).template_data);
                }
                return true;
            }
            else
            {
                return false;
            }
          
           
        }
    }



    /**<summary>
     * Controller for the Trigger / Action Category Lists
     * </summary>
     */ 
    class MayhemInterfaceCategoryListController
    {

        public static readonly string TAG = "[MayhemInterfaceCategoryListController] : ";

        public string list_title { get; set; }

        // the categories go into here
        public ObservableCollection<object> current_displayItems = null; 
        public MayhemInterfaceItemListPage  current_page = null;

     
        public MayhemInterfaceItemListPage top_page; 

        public ListBox observeListbox { get; set; }
        public ListNavigationControl observe_navcontrol { get; set; }

       

        public MayhemInterfaceCategoryListController(ListBox observe, ListNavigationControl navControl, string title)
        {
            top_page = new MayhemInterfaceItemListPage(null, title);
            observeListbox = observe;
            observe_navcontrol = navControl;

            observeListbox.SelectionChanged += new SelectionChangedEventHandler(l_SelectionChanged);
            current_page = top_page;
            current_page.previousPage = null;
            current_displayItems = current_page.display_items;


            observeListbox.DataContext = current_page;
            observe_navcontrol.DataContext = current_page;
            observe_navcontrol.back_button.Click += new System.Windows.RoutedEventHandler(back_button_Click);

        }

        void back_button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (current_page.previousPage != null)
            {
                GoToPreviousPage();
            }

        }

        void l_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(TAG + "selection just changed");

            object item = observeListbox.SelectedItem;
            if (item as LibraryListItem != null)
            {
                LibraryListItem l = item as LibraryListItem;
                Debug.WriteLine(TAG + " LibraryListItem: " + l.Label);
               
                // try to switch selection
                // TODO: Trigger some animation at this point!
                if (current_page.subPages[l] != null)
                {
                    GoToNextPage(current_page.subPages[l]);
                }
                
            }
        }

        private void GoToPreviousPage()
        {
            current_page = current_page.previousPage;
            current_displayItems = current_page.display_items;
            observeListbox.ItemsSource = current_displayItems;
            observeListbox.DataContext = current_page;
            observe_navcontrol.DataContext = current_page;

            // not at top page, unhide the back button
            if (current_page.previousPage != null)
            {
                observe_navcontrol.back_button.Visibility = Visibility.Visible;
            }
            else
            {
                observe_navcontrol.back_button.Visibility = Visibility.Hidden;
            }

        }

        private void GoToNextPage(MayhemInterfaceItemListPage next_page)
        {
            next_page.previousPage = current_page;
            current_page = next_page;
            current_displayItems = current_page.display_items;
            observeListbox.ItemsSource = current_displayItems;
            observeListbox.DataContext = current_page;
            observe_navcontrol.DataContext = current_page;

            // not at top page, unhide the back button
            if (current_page.previousPage != null)
            {
                observe_navcontrol.back_button.Visibility = Visibility.Visible;
            }
            else
            {
                observe_navcontrol.back_button.Visibility = Visibility.Hidden;
            }
        }
        
        public void AddDisplayItem(MayhemConnectionItem item)
        {
            Debug.WriteLine(TAG + " Adding ConnectionItem " + item.description + " " + item.ToString());
            current_page.Add_Item(item);
           
        }

        public void AddDisplayItem(LibraryListItem category)
        {
            Debug.WriteLine(TAG + " Adding Item " + category.Label + " " +  category.ToString());
            current_page.Add_Item(category);
        }

        public void AddSubPageToItem(object item, string subPageTitle)
        {
            Debug.WriteLine(TAG + " Addint Subpage to(2) " + item.ToString());

            current_page.Add_SubPage_To_Item(item, new MayhemInterfaceItemListPage(subPageTitle));

        }

        public void AddSubPageToItem(object item, MayhemInterfaceItemListPage page)
        {
            Debug.WriteLine(TAG + " Addint Subpage to " + item.ToString());
            current_page.Add_SubPage_To_Item(item, page);
        }

        /**<summary>
         * Check if top page contains the item we are looking for
         * Used to support drag and drop operations
         * </summary>
         * */
        public bool ContainsItem(object item)
        {
            if (top_page.Find_Page_Containing_Item(item) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /** <summary>
         * Replaces an  item on one of the subpages
         * </summary>
         */
        public bool ReplaceItem(object original_item, object new_item)
        {
            MayhemInterfaceItemListPage p = top_page.Find_Page_Containing_Item(original_item);
            if (p != null)
            {
                return p.Replace_Display_Item(original_item, new_item); 
            }
            else
            {
                return false;
            }
        }
    }
}
