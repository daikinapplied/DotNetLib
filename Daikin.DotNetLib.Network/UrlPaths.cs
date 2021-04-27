using System.Collections.Generic;
using System.Linq;

namespace Daikin.DotNetLib.Network
{
    public class UrlPaths
    {
        #region Fields
        private readonly List<string> _urlPaths;
        #endregion

        #region Constructors

        public UrlPaths()
        {
            _urlPaths = new List<string>();
        }

        public UrlPaths(params string[] paths)
        {
            _urlPaths = new List<string>();
            Add(paths);
        }
        #endregion

        #region Methods

        public void Clear()
        {
            _urlPaths.Clear();
        }

        public void Add(params string[] paths)
        {
            foreach (var path in paths)
            {
                _urlPaths.Add(path);
            }
        }

        public new string ToString()
        {
            var url = _urlPaths.Aggregate(string.Empty, Url.Concatenate);
            if (!url.EndsWith("/")) url += "/"; // always end with a slash - really important with Authenticaed API calls 
            return url;
        }
        #endregion

    }
}
