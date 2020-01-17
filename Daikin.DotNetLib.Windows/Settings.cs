using System;
using System.Text;
using System.Xml;

namespace Daikin.DotNetLib.Windows
{
    public class Settings : System.Collections.Hashtable
    {
        #region Enumerators
        /// <summary>
        /// Method to load/save the table
        /// </summary>
        public enum StoreStyle
        {
            None,
            Registry,
            Xml
        }

        /// <summary>
        /// Method to truncate the table for a given key (if multiple values)
        /// </summary>
        public enum TruncateStyle
        {
            Oldest,
            Newest
        }
        #endregion

        #region Constants
        private const string Version = "1.0.2"; // version (especially for XML document)
        #endregion

        #region Fields
        private StoreStyle _storeStyle = StoreStyle.None;
        private string _storeLoc = ""; // Location to use when _storeStyle!=None
        private System.Collections.ArrayList _currentHashValue; // Current value of the hashtable that has "focus"
        private int _currentValueIndex; // Current Array list entry that has "focus" within the hashtable entry

        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Settings()
        {
            Reset();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="style">Style to save/load data</param>
        /// <param name="location">Path for the given style specified</param>
        public Settings(StoreStyle style, string location)
        {
            Reset();
            SetStoreStyle(style, location);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Reset the current "pointer" to Key and Value
        /// </summary>
        private void Reset()
        {
            CurrentKey = null;
            _currentHashValue = null;
            _currentValueIndex = 0;
        }

        /// <summary>
        /// Set the particular store defaults for reading/writing
        /// </summary>
        /// <param name="style">Storage Style</param>
        /// <param name="location">Path based on the storage style selected</param>
        public void SetStoreStyle(StoreStyle style, string location)
        {
            _storeStyle = style;
            _storeLoc = location;
        }

        /// <summary>
        /// Find the first element based on the key specified
        /// </summary>
        /// <param name="key">Key to locate</param>
        /// <returns>Whether there are values available for the associated key</returns>
        /// <remarks>
        /// There are three outcomes as a result to calling this method:
        /// (1) Desired key found and one or more values are available: this will yield success and an update of the current "pointers"
        /// (2) Desired key found and no values are available: this will yield failure but the current points will be correct
        /// (3) Desired key not found: this will yield failure and resetting of the pointers to "null"
        /// </remarks>
        public bool FindFirst(object key)
        {
            CurrentKey = key;
            _currentHashValue = (this[key] as System.Collections.ArrayList);

            if (_currentHashValue != null) // if found the key and associated values
            {
                _currentValueIndex = 0; // reset to the top
                return _currentHashValue.Count > 0;

                // A Key as found, but no value -- this is a special case that keeps
                // current pointer to the key and empty value list.  It is up to the calling
                // code to understand the scenario of FindFirst yielding false but still
                // accessing its current reference points (really used internally in this class)
            }

            // The Key was not Found so reset all
            Reset();
            return false; // Not found
        }

        /// <summary>
        /// Find the first element based on the key and value specified 
        /// </summary>
        /// <param name="key">Key to locate</param>
        /// <param name="value">Value to locate</param>
        /// <returns>Whether a value was found</returns>
        public bool FindFirst(object key, object value)
        {
            if (!FindFirst(key)) return false; // Not found
            var index = 0;
            while (index < _currentHashValue.Count)
            {
                if (_currentHashValue[index].Equals(value))
                {
                    _currentValueIndex = index;
                    return true; // Found
                }
                index++;
            }
            return false; // Not found
        }

        /// <summary>
        /// Find the first element based on the key specified
        /// </summary>
        /// <param name="key">Key to locate</param>
        /// <param name="index">Value index to locate</param>
        /// <returns>Whether a value was found</returns>
        public bool FindFirst(object key, int index)
        {
            if (!FindFirst(key)) return false; // No find
            if (index < 0 || index >= _currentHashValue.Count) return false; // No find
            _currentValueIndex = index;
            return true; // found
        }

        /// <summary>
        /// Find the next available value for the previously specified key
        /// </summary>
        /// <returns>Whether another value as found</returns>
        public bool FindNext()
        {
            if (_currentHashValue == null) return false; // Not found
            _currentValueIndex++; // move to the next item
            return _currentValueIndex < _currentHashValue.Count;
        }

        /// <summary>
        /// Remove an existing key
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <remarks>
        /// This is done because it needs to reset the pointers
        /// </remarks>
        public override void Remove(object key)
        {
            base.Remove(key);
            Reset();
        }

        /// <summary>
        /// Add another instance of the same key base
        /// </summary>
        /// <param name="key">Key to add</param>
        /// <param name="value">Associated value to the key</param>
        /// <returns>The index of the value to the particular key</returns>
        public int AddNext(object key, object value)
        {
            // If the key doesn't already exist, then need to create an instance
            if (!FindFirst(key))
            {
                // Check to see if there is a key without any associated values
                // (special outcome case from the FindFirst call)
                if (_currentHashValue != null)
                {
                    Remove(key);
                }

                Add(key, new System.Collections.ArrayList());
                FindFirst(key); // this will still fail, but the pointers must be set
            }

            // Now the key and value (even if it is just shell without entries)
            // must exist, so can now add and set the current array entry "pointer"
            _currentValueIndex = _currentHashValue.Add(value);

            return _currentValueIndex;
        }

        /// <summary>
        /// Add another instance of the same key base
        /// </summary>
        /// <param name="key">Key to add</param>
        /// <param name="value">Associated value to the key</param>
        /// <param name="duplicateValuesOkay">Whether duplicate values are okay</param>
        /// <returns>The index of the value to the particular key</returns>
        /// <remarks>
        /// If duplicates are not allowed, then the existing entry is removed to preserve
        /// the order in the list (so newest are at the end).
        /// </remarks>
        public int AddNext(object key, object value, bool duplicateValuesOkay)
        {
            while (!duplicateValuesOkay && FindFirst(key, value))
            {
                // Duplicates not allowed and an existing entry was found, so remove all occurrences (in case there are multiples)
                _currentHashValue.Remove(value);
            }
            return AddNext(key, value); // now add
        }


        /// <summary>
        /// Allows only a single value for a particular Key - if there are are multiple values, they are removed.
        /// </summary>
        /// <param name="key">Key to add</param>
        /// <param name="value">Associated value to the key</param>
        /// <returns></returns>
        public int AddOverwrite(object key, object value)
        {
            Remove(key);
            return AddNext(key, value);
        }

        /// <summary>
        /// Remove excess number of values for a given key
        /// </summary>
        /// <param name="count">Number of values to keep in the particular key list</param>
        /// <param name="ts">How to truncate</param>
        /// <param name="key">Key to truncate</param>
        public void Truncate(int count, TruncateStyle ts, object key)
        {
            if (count < 1)
            {
                throw new IndexOutOfRangeException("Invalid number of items to truncate - must be positive but was given: " + count);
            }

            if (FindFirst(key)) // Find the key and verify it has at least one value
            {
                var deleteCount = _currentHashValue.Count - count; // Number of values to delete
                if (deleteCount > 0)
                {
                    switch (ts)
                    {
                        case (TruncateStyle.Newest): // Truncate from the end
                            _currentHashValue.RemoveRange((_currentHashValue.Count - 1) - deleteCount, deleteCount);
                            break;
                        case (TruncateStyle.Oldest): // Truncate from the beginning
                            _currentHashValue.RemoveRange(0, deleteCount);
                            break;
                        default:
                            throw new IndexOutOfRangeException("Unknown Truncate Style Encountered");
                    } // switch
                }
            }
            Reset();
        }

        /// <summary>
        /// Remove excess number of values on a per key basis (but for all keys)
        /// </summary>
        /// <param name="count">Number of values to keep per-key</param>
        /// <param name="ts">How to truncate</param>
        public void Truncate(int count, TruncateStyle ts)
        {
            // Work through the hash table
            var idicte = GetEnumerator();
            while (idicte.MoveNext())
            {
                Truncate(count, ts, idicte.Key);
            }
        }

        /// <summary>
        /// Save items in the hash table to an XML file
        /// </summary>
        /// <param name="file">XML File to save (overwrite) to</param>
        /// <param name="excludeEmptyEntries">Whether to ignore empty entries</param>
        private void SaveXml(string file, bool excludeEmptyEntries = true)
        {
            var xtw = new XmlTextWriter(file, Encoding.Default) {Formatting = Formatting.Indented};
            xtw.WriteStartDocument(true);
            xtw.WriteStartElement("settings"); // Root element <settings>
            xtw.WriteAttributeString("version", Version);

            // Work through the hash table
            var idicte = GetEnumerator();
            while (idicte.MoveNext()) // While there is data to review
            {
                xtw.WriteStartElement("entry");
                xtw.WriteAttributeString("key", System.Web.HttpUtility.HtmlEncode(idicte.Key!=null ? idicte.Key.ToString() : string.Empty));

                var valueList = (idicte.Value as System.Collections.ArrayList);
                if (valueList != null)
                {
                    for (var indexValue = 0; indexValue < valueList.Count; indexValue++)
                    {
                        // Either there is data to write or empty entries are desired
                        if (!excludeEmptyEntries || indexValue.ToString().Trim().Length > 0)
                        {
                            xtw.WriteElementString("value", System.Web.HttpUtility.HtmlEncode(indexValue.ToString()));
                        }
                    }
                }

                xtw.WriteEndElement(); // </entry>
            }

            xtw.WriteEndElement(); // </settings>
            xtw.WriteEndDocument();
            xtw.Close();
        }

        /// <summary>
        /// Read in values from an XML file (replace existing)
        /// </summary>
        /// <param name="file">XML File to Load from</param>
        private void LoadXml(string file)
        {
            Clear(); // Remove all existing keys from the HashTable
            if (!System.IO.File.Exists(file)) return; // make sure there is a file to load
            var xDoc = new XmlDocument();

            try
            {
                xDoc.Load(file);

                var settingNode = xDoc.SelectSingleNode("settings");
                if (settingNode?.Attributes == null) return;
                var version = settingNode.Attributes["version"].Value; // Has the file version
                if (version != Version) return; // mismatch on version (TODO: Add logic for backward compatibility, if supported)

                var keyList = xDoc.SelectNodes("settings/entry");
                if (keyList == null) return;
                foreach (XmlNode keyNode in keyList) // Read in keys
                {
                    if (keyNode?.Attributes == null) return;
                    var key = keyNode.Attributes["key"].Value;
                    var arrayValues = new System.Collections.ArrayList();

                    var valueList = keyNode.SelectNodes("value");
                    if (valueList == null) return;
                    foreach (XmlNode valueNode in valueList)
                    {
                        var value = valueNode.InnerText;
                        arrayValues.Add(value); // add to the list
                    }

                    if (arrayValues.Count > 0) // if there are values to be had
                    {
                        Add(key, arrayValues); // put the key and associated values into the HashTable
                    }
                }
            }
            catch (XmlException ex)
            {
                throw new FormatException("Invalid XML Format Encountered", ex.InnerException);
            }
            catch (NullReferenceException ex)
            {
                throw new NullReferenceException("Null Reference Exception Encountered with Node", ex.InnerException);
            }
        }

        /// <summary>
        /// Saves values in list to the HKEY_CURRENT_USER registry area.  This removes any existing Registry values within a Registry Key.
        /// </summary>
        /// <param name="path">Location in Registry to save</param>
        /// <param name="excludeEmptyEntries">Whether to ignore empty entries</param>
        private void SaveRegistry(string path, bool excludeEmptyEntries = true)
        {
            Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(path, false); // Remove existing keys (to avoid them reappearing), if a key exists (dont' throw exception if it doesn't)
            Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path); // create key

            var settingReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path, true);
            if (settingReg == null) return;

            // Work through the hash table
            var idicte = GetEnumerator();
            while (idicte.MoveNext()) // while there is data to review
            {
                var valueList = (idicte.Value as System.Collections.ArrayList);
                if (valueList == null) continue;
                for (var indexValue = 0; indexValue < valueList.Count; indexValue++)
                {
                    // Either there is data to write or empty entries are desired
                    if (!excludeEmptyEntries || valueList[indexValue].ToString().Trim().Length > 0)
                    {
                        // Save as <Key>-(n)=<Value>
                        // The hyphen is used so it can be searched from the right/end to find
                        // the core key name when it is loaded back into memory.
                        settingReg.SetValue(idicte.Key + "-" + indexValue, valueList[indexValue].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Load in values from the HKEY_CURRENT_USER registry area into the list (Replace existing)
        /// </summary>
        /// <param name="path">Location</param>
        private void LoadRegistry(string path)
        {
            Reset();

            try
            {
                var settingReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);
                if (settingReg == null) return;

                var settingValues = settingReg.GetValueNames();
                foreach (var settingValue in settingValues)
                {
                    var key = settingValue;
                    if (key.LastIndexOf("-", StringComparison.Ordinal) <= 0) continue;
                    key = key.Substring(0, key.LastIndexOf("-", StringComparison.Ordinal)); // Generic key name
                    AddNext(key, settingReg.GetValue(settingValue)); // add Key & Value to the list
                }

            }
            catch (NullReferenceException)
            {
                throw new InvalidRegistryPathException("Path Attempted: " + path);
            }
        }

        /// <summary>
        /// Load the store into memory based in the preset style and location
        /// </summary>
        public void LoadStore()
        {
            switch (_storeStyle)
            {
                case StoreStyle.Registry:
                    LoadRegistry(_storeLoc);
                    break;
                case StoreStyle.Xml:
                    LoadXml(_storeLoc);
                    break;
                case StoreStyle.None:
                    break;
                default:
                    throw new IndexOutOfRangeException("Must set the Store Style before Loading");
            }
        }

        /// <summary>
        /// Load the store into memory based in the specified style and location
        /// </summary>
        /// <param name="style">Store Style to use</param>
        /// <param name="location">Location based on the Store Style selected</param>
        public void LoadStore(StoreStyle style, string location)
        {
            SetStoreStyle(style, location);
            LoadStore();
        }

        /// <summary>
        /// Save the store from memory based on the preset style and location
        /// </summary>
        public void SaveStore()
        {
            switch (_storeStyle)
            {
                case StoreStyle.Registry:
                    SaveRegistry(_storeLoc);
                    break;
                case StoreStyle.Xml:
                    SaveXml(_storeLoc);
                    break;
                case StoreStyle.None:
                    break;
                default:
                    throw new IndexOutOfRangeException("Must set the Store Style before Saving");
            }

        }

        /// <summary>
        /// Save the store from memory based on the specified style and location
        /// </summary>
        /// <param name="style">Store Style to use</param>
        /// <param name="location">Location based on the Store Style selected</param>
        public void SaveStore(StoreStyle style, string location)
        {
            SetStoreStyle(style, location);
            SaveStore();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the current key that has focus
        /// </summary>
        public object CurrentKey { get; private set; }


        /// <summary>
        /// Get/Set the current value position in the list for a given key that has focus
        /// </summary>
        public int CurrentValueIndex
        {
            get => _currentValueIndex;

            set
            {
                if (_currentHashValue != null)
                {
                    if (value >= 0 && value < _currentHashValue.Count)
                    {
                        _currentValueIndex = value;
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("Invalid index specified.  Received: " + value + " by expected 0.." + _currentHashValue.Count);
                    }
                }
                else
                {
                    throw new NullReferenceException("Key has not been previously set prior to calling property");
                }
            }
        }

        /// <summary>
        /// Get/Set the Current Value that has focus
        /// </summary>
        public object CurrentValue
        {
            get
            {
                if (_currentHashValue != null && _currentValueIndex < _currentHashValue.Count)
                {
                    return (_currentHashValue[_currentValueIndex]); // current value
                }
                return null; // No current value
            }
        }
        #endregion
    }
}
