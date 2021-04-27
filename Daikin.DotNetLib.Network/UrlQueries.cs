using System.Collections.Generic;
using System.Web;

namespace Daikin.DotNetLib.Network
{
    public class UrlQueries
    {
        #region Fields
        private readonly bool _encodeVars;
        private readonly bool _encodeValues;
        private readonly List<KeyValuePair<string,string>> _queries;
        #endregion

        #region Constructors

        public UrlQueries(bool encodeVars = false, bool encodeValues = false)
        {
            _queries = new List<KeyValuePair<string, string>>();
            _encodeVars = encodeVars;
            _encodeValues = encodeValues;
        }

        public UrlQueries(string queryString, bool encodeVars = false, bool encodeValues = false)
        {
            _queries = new List<KeyValuePair<string, string>>();
            _encodeVars = encodeVars;
            _encodeValues = encodeValues;
            Add(queryString);
        }
        #endregion

        #region Methods
        public void Set(string queryString)
        {
            _queries.Clear();
            Add(queryString);
        }

        public void Set(string var, string value)
        {
            _queries.Clear();
            Add(var, value);
        }

        public void Add(string queryString)
        {
            if (queryString.StartsWith("?")) queryString = queryString.Substring(1); // eliminate ?
            foreach (var query in queryString.Split('&'))
            {
                var split = query.Split('=');
                if (split.Length == 2)
                {
                    _queries.Add(new KeyValuePair<string, string>(split[0].Trim(), split[1].Trim()));
                }
            }
        }

        public void Add(string var, string value)
        {
            _queries.Add(new KeyValuePair<string, string>(var.Trim(), value.Trim()));
        }

        public new string ToString()
        {
            var queryString = string.Empty;
            foreach (var query in _queries)
            {
                var var = query.Key;
                if (_encodeVars) var = HttpUtility.UrlEncode(var);
                var value = query.Value;
                if (_encodeValues) value = HttpUtility.UrlEncode(value);
                if (!string.IsNullOrEmpty(queryString)) queryString += "&";
                queryString += $"{var}={value}";
            }
            return queryString;
        }

        public bool HasData()
        {
            return _queries != null && _queries.Count > 0;
        }
        #endregion
    }
}
