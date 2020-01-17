using System;

namespace RecursiveGeek.DotNetLib.Data
{
    /// <summary>
    /// ObjectSorter class used to sort data on a item by item basis.
    /// For example, this can be used via a call from the ListView or Grid Control,
    /// to sort columns.
    /// </summary>
    public class ObjectSorter : System.Collections.IComparer
    {
        #region Fields
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private readonly bool _ascending;
        private readonly string _fieldName;
        #endregion

        #region Constructors
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="ascending">Whether ascending order</param>
        public ObjectSorter(string fieldName, bool ascending)
        {
            _ascending = ascending;
            _fieldName = fieldName;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Compares two objects.  Null values are considered the lowest possible value when comparing.
        /// </summary>
        /// <param name="pObject1">Object 1 to compare</param>
        /// <param name="pObject2">Object 2 to compare</param>
        /// <returns>0, -1, 1, 2: Equality of values</returns>
        /// <remarks>
        /// Null values are considered the lowest possible value when comparing
        /// </remarks>
        public int Compare(object pObject1, object pObject2)
        {
            if (pObject1 == null || pObject2 == null) throw new Exception("Missing an object as a parameter");

            var object1Info = pObject1.GetType().GetProperty(_fieldName);
            var object2Info = pObject2.GetType().GetProperty(_fieldName);

            if (object1Info == null || object2Info == null) throw new Exception("Unable to get properties of parameter");

            // get object values and cast as Icomparable.
            // if object does not implement Icomparable it will be null
            var comp1 = (IComparable)object1Info.GetValue(pObject1, null);
            var comp2 = (IComparable)object2Info.GetValue(pObject2, null);

            int nCompareValue;
            if (comp1 == null && comp2 == null)
            {
                nCompareValue = 0; // both values are null, therefore equal
            }
            else if (comp1 == null)
            {
                nCompareValue = -1;  // first object value is null
            }
            else if (comp2 == null)
            {
                nCompareValue = 1; // second object value is null
            }
            else
            {
                // compare the two objects using the objects compare method
                // if the object does not implement IComparable this will throw an exception
                nCompareValue = comp1.CompareTo(comp2);
            }

            if (_ascending) // if sort order is descending comparison values are floped
            {
                return nCompareValue;
            }
            else
            {
                return -nCompareValue;
            }
        }
        #endregion
    }
}
