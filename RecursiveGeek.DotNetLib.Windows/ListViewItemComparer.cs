using System;
using System.Windows.Forms;

namespace RecursiveGeek.DotNetLib.Windows
{
    /// <summary>
    /// Sort items in a ListView by its columns
    /// </summary>
    public class ListViewItemComparer : System.Collections.IComparer
    {
        #region Enumerators
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public enum SortType
        {
            String,
            Numeric,
            Date
        }
        #endregion


        #region Fields
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private readonly int _col;
        private readonly SortOrder _order;
        private readonly SortType _sortStyle;
        #endregion

        #region Constructors
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ListViewItemComparer()
        {
            _col = 0;
            _order = SortOrder.Ascending;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="column">Column to sort</param>
        public ListViewItemComparer(int column)
        {
            _col = column;
            _order = SortOrder.Ascending;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="column">Column to sort</param>
        /// <param name="sortStyle">Method of sorting the string data</param>
        public ListViewItemComparer(int column, SortType sortStyle)
        {
            _col = column;
            _order = SortOrder.Ascending;
            _sortStyle = sortStyle;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="column">Column to sort</param>
        /// <param name="order">Sort order type (ascending, descending)</param>
        public ListViewItemComparer(int column, SortOrder order)
        {
            _col = column;
            _order = order;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="column">Column to sort</param>
        /// <param name="order">Sort order type (ascending, descending)</param>
        /// <param name="sortStyle">Method of sorting the string data</param>
        public ListViewItemComparer(int column, SortOrder order, SortType sortStyle)
        {
            _col = column;
            _order = order;
            _sortStyle = sortStyle;
        }
        #endregion

        #region Methods
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Method (override) implementation to compare to objects in a ListView control (by column)
        /// </summary>
        /// <param name="x">First item to compare</param>
        /// <param name="y">Second item to compare</param>
        /// <returns>Which it is greater for sorting purposes</returns>
        public int Compare(object x, object y)
        {
            if (x == null || y == null) throw new Exception("Mising object to compare");

            var returnVal = -1; // default result of sorting

            string value1;
            try
            {
                value1 = ((ListViewItem)x).SubItems[_col].Text;
            }
            catch
            {
                value1 = null;
            }

            string value2;
            try
            {
                value2 = ((ListViewItem)y).SubItems[_col].Text;
            }
            catch
            {
                value2 = null;
            }

            // Determine how to sort based on the values extracted
            if (value1 == null && value2 == null)
            {
                returnVal = 0; // both equal
            }
            else if (value1 == null)
            {
                returnVal = -1; // first item is null
            }
            else if (value2 == null)
            {
                returnVal = 1; // second item is null
            }
            else
            {
                switch (_sortStyle)
                {
                    case SortType.String:
                        returnVal = string.CompareOrdinal(value1, value2);
                        break;
                    case SortType.Numeric:
                        if (Convert.ToInt64(value1) > Convert.ToInt64(value2))
                            returnVal = 1;
                        else
                            returnVal = -1;
                        break;
                    case SortType.Date:
                        returnVal = DateTime.Compare(DateTime.Parse(value1), DateTime.Parse(value2));
                        break;
                }
            }

            // Determine whether sorting is descending
            if (_order == SortOrder.Descending)
            {
                // Invert the value returned from String.Compare
                returnVal *= -1;
            }

            return returnVal;
        }

        /// <summary>
        /// Event to setup the sorting of columns in the ListView control
        /// </summary>
        /// <param name="sender">Caller to this event</param>
        /// <param name="e">Event Arguments from the caller</param>
        /// <param name="sortColumns">How to sort the string column (what type of data to represent it as)</param>
        public static void SortListViewColumn(object sender, ColumnClickEventArgs e, SortType[] sortColumns)
        {
            var list = (ListView)sender;
            var newSortColumn = e.Column;
            int previousSortColumn;
            SortType sortStyle;

            if (list.Tag != null) // See if the last sorted column information is hidden in the control
            {
                previousSortColumn = (int)list.Tag;
            }
            else
            {
                previousSortColumn = 0; // default if not previously defined
            }

            // Determine if ascending or descending - if new column, ascending.  If repeat
            // click, then toggle
            if (previousSortColumn != newSortColumn)
            {
                // Set new column as ascending
                list.Sorting = SortOrder.Ascending;
            }
            else
            {
                // If the existing column is selected again, toggle the sort order
                list.Sorting = list.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }

            // Create a pointing to the class that has the Compare method to do the comparison as items are sorted
            if (sortColumns != null && newSortColumn < sortColumns.Length)
            {
                // A column sort style has been specified for the column selected
                sortStyle = sortColumns[newSortColumn];
            }
            else
            {
                sortStyle = SortType.String;
            }
            list.ListViewItemSorter = new ListViewItemComparer(newSortColumn, list.Sorting, sortStyle);
            list.Sort(); // Sort the list

            list.Tag = newSortColumn; // Store in the control
        }

        /// <summary>
        /// Event that can be used with many ClickColumn event handlers (character string sorting)
        /// </summary>
        /// <param name="sender">Caller of this event</param>
        /// <param name="e">Event Arguments from Caller</param>
        public static void SortListViewColumn(object sender, ColumnClickEventArgs e)
        {
            SortListViewColumn(sender, e, null);
        }
        #endregion
    }
}
