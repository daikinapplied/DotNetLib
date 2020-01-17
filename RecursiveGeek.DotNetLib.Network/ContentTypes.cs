using System.Collections;

namespace RecursiveGeek.DotNetLib.Network
{
    public class ContentTypes
    {
        #region Fields
        private readonly SortedList _sortedList;
        #endregion

        #region Constants
        public const string DefaultMimeType = "application/octet-stream";
        #endregion

        #region Constructors
        public ContentTypes()
        {
            _sortedList = new SortedList
            {
                {"png", "image/png"},
                {"gif", "image/gif"},
                {"jpg", "image/jpeg"},
                {"jpeg", "image/jpeg"},
                {"tif", "image/tiff"},
                {"tiff", "image/tiff"},
                {"ico", "image/x-icon"},
                {"ai", "application/postscript"},
                {"eps", "application/postscript"},

                {"htm", "text/html"},
                {"html", "text/html"},
                {"css", "text/css"},
                {"less", "text/css"},
                {"json", "application/json"},

                {"wma", "video/x-ms-wma"},
                {"wmv", "video/x-ms-wmv"},
                {"mp4", "video/mp4"},
                {"mpg", "video/mpeg"},
                {"mpeg", "video/mpeg"},
                {"mov", "video/quicktime"},
                {"movie", "video/x-sgi-movie"},
                {"m1v", "video/mpeg"},
                {"m4v", "video/mp4"},
                {"m4a", "audio/mp4"},
                {"flv", "video/x-flv"},

                {"pdf", "application/pdf"},

                {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {"doc", "application/msword"},
                {"dot", "application/msword"},
                {"docm", "application/vnd.ms-word.document.macroEnabled.12"},

                {"xls", "application/vnd.ms-excel"},
                {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {"xlm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {"xlt", "application/vnd.ms-excel"},
                {"xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
                {"xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {"xlw", "application/vnd.ms-excel"},
                {"xla", "application/vnd.ms-excel"},

                {"ppt", "application/vnd.ms-powerpoint"},
                {"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {"pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {"pot", "application/vnd.ms-powerpoint"},
                {"potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
                {"ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
                {"pps", "application/vnd.ms-powerpoint"},

                {"mpp", "application/vnd.ms-powerpoint"},
                {"lit", "application/x-ms-reader"},
                {"exe", "application/octet-stream"},
                {"tar", "application/x-tar"},
                {"z", "application/x-compress"},
                {"zip", "application/x-zip-compressed"}
            };
        }
        #endregion

        #region Methods
        public string GetMimeTypeFromExtension(string extension)
        {
            if (extension.StartsWith(".")) { extension = extension.Substring(1); }
            var mimeTypeObj = _sortedList[extension.ToLower()];
            return mimeTypeObj == null ? DefaultMimeType : mimeTypeObj.ToString();
        }

        public string GetMImeTypeFromFilename(string filename)
        {
            var extensionStart = filename.LastIndexOf('.');
            var extension = string.Empty;
            if (extensionStart > -1)
            {
                extension = filename.Substring(extensionStart + 1);
            }
            return GetMimeTypeFromExtension(extension);
        }

        public string GetExtension(string mimeType)
        {
            var extensionObj = _sortedList.GetKey(_sortedList.IndexOfValue(mimeType.ToLower()));
            return extensionObj == null ? string.Empty : extensionObj.ToString();
        }
        #endregion
    }
}
