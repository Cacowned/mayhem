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
            if (current_page.previousPage != null)
            {
                GoToPreviousPage();
            }

        }

        void l_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            Debug.WriteLine(TAG + " Adding Subpage to(2) " + item.ToString());

            current_page.Add_SubPage_To_Item(item, new MayhemInterfaceItemListPage(subPageTitle));

        }

        public void AddSubPageToItem(object item, MayhemInterfaceItemListPage page)
        {
            Debug.WriteLine(TAG + " Adding Subpage to " + item.ToString());
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
