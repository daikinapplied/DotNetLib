namespace Daikin.DotNetLib.Network
{
    public class UrlPage
    {
        #region Fields
        private string _pageName;
        #endregion

        #region Constructors

        public UrlPage()
        {
            _pageName = string.Empty;
        }

        public UrlPage(string pageName)
        {
            _pageName = pageName;
        }
        #endregion

        #region Methods
        public void Set(string pageName)
        {
            _pageName = pageName;
        }

        public new string ToString()
        {
            return _pageName;
        }
        #endregion

    }
}
