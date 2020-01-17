using RecursiveGeek.DotNetLib.Network;
using Xunit;

namespace RecursiveGeek.DotNetLib.Core.Tests
{
    public class MimeTypeTests
    {
        #region Fields
        private readonly ContentTypes _contentTypes;
        #endregion

        #region Constructors
        public MimeTypeTests()
        {
            _contentTypes = new ContentTypes();
        }
        #endregion

        #region Tests
        [Fact]
        public void ExtensionLookup()
        {
            var contentTypePdf = _contentTypes.GetMimeTypeFromExtension("pdf");
            Assert.True(!string.IsNullOrEmpty(contentTypePdf) && contentTypePdf != ContentTypes.DefaultMimeType, "PDF MimeType Not Found from Extension");
        }

        [Fact]
        public void FilenameLookup()
        {
            var contentTypePdf = _contentTypes.GetMImeTypeFromFilename(@"c:\hans\MYFILE.PDF");
            Assert.True(!string.IsNullOrEmpty(contentTypePdf) && contentTypePdf != ContentTypes.DefaultMimeType, "PDF MimeType Not Found from Filename");
        }

        [Fact]
        public void MimeTypeLookup()
        {
            var extensionPdf = _contentTypes.GetExtension("application/pdf");
            Assert.True(!string.IsNullOrEmpty(extensionPdf), "PDF Extension Not Found from MimeType");
        }

        [Fact]
        public void MimeTypeUpperCaseLookup()
        {
            var extensionPdf = _contentTypes.GetExtension("APPLICATION/PDF");
            Assert.True(!string.IsNullOrEmpty(extensionPdf), "PDF Extension Not Found from Uppercase MimeType");
        }
        #endregion
    }
}
