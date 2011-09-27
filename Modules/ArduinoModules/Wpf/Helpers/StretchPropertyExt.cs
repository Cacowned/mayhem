using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// ListViewColumnStretch
    /// </summary>
    public class DataGridColumns : DependencyObject
    {

        #region Fields

        /// <summary>
        /// Holds a reference to our parent ListView control
        /// </summary>
        private DataGrid _parentListView = null;

        #endregion

        #region Dependancy Property Infrastructure

        /// <summary>
        /// IsStretched Dependancy property which can be attached to gridview columns.
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.RegisterAttached("Stretch",
            typeof(bool),
            typeof(DataGridColumns),
            new UIPropertyMetadata(true, null, OnCoerceStretch));


        /// <summary>
        /// Gets the stretch.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetStretch(DependencyObject obj)
        {
            return (bool)obj.GetValue(StretchProperty);
        }

        /// <summary>
        /// Sets the stretch.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetStretch(DependencyObject obj, bool value)
        {
            obj.SetValue(StretchProperty, value);
        }

        /// <summary>
        /// Called when [coerce stretch].
        /// </summary>
        /// <remarks>If this callback seems unfamilar then please read
        /// the great blog post by Paul jackson found here. 
        /// http://compilewith.net/2007/08/wpf-dependency-properties.html</remarks>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object OnCoerceStretch(DependencyObject source, object value)
        {
            DataGrid lv = (source as DataGrid);

            //Ensure we dont have an invalid dependancy object of type ListView.
            if (lv == null)
                throw new ArgumentException("This property may only be used on ListViews");

            //Setup our event handlers for this list view.
            lv.Loaded += new RoutedEventHandler(lv_Loaded);
            lv.SizeChanged += new SizeChangedEventHandler(lv_SizeChanged);
            return value;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the SizeChanged event of the lv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private static void lv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DataGrid lv = (sender as DataGrid);
            if (lv.IsLoaded)
            {
                //Set our initial widths.
                SetColumnWidths(lv);
            }
        }

        /// <summary>
        /// Handles the Loaded event of the lv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private static void lv_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid lv = (sender as DataGrid);
            //Set our initial widths.
            SetColumnWidths(lv);
        }
        #endregion

        #region Private Members

        /// <summary>
        /// Sets the column widths.
        /// </summary>
        private static void SetColumnWidths(DataGrid gridView)
        {
            //Pull the stretch columns fromt the tag property.
            ObservableCollection<DataGridColumn> columns = gridView.Columns as ObservableCollection<DataGridColumn>;
            double specifiedWidth = 0;
           
            if (gridView != null)
            {
                if (columns == null)
                {
                    //Instance if its our first run.
                    columns = new ObservableCollection<DataGridColumn>();
                    // Get all columns with no width having been set.
                    foreach (DataGridColumn column in gridView.Columns)
                    {
                       
                        if (!(column.Width.Value >= 0))
                            columns.Add(column);
                        else specifiedWidth += column.ActualWidth;
                    }
                }
                else
                {
                    // Get all columns with no width having been set.
                    foreach (DataGridColumn column in gridView.Columns)
                        if (!columns.Contains(column))
                            specifiedWidth += column.ActualWidth;
                }

                // Allocate remaining space equally.
                foreach (DataGridColumn column in columns)
                {
                    double newWidth = (gridView.ActualWidth - specifiedWidth) / columns.Count;
                    if (newWidth >= 0) column.Width = newWidth - 10;
                }

                //Store the columns in the TAG property for later use. 
                gridView.Tag = columns;
            }
        }


        #endregion

    }
}

