using System;
using Daikin.DotNetLib.Data;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class Error
    {
        #region Constants
        private const string Code = "Ref A17";
        private const string Message = "Unable to access the whatchamacallit";
        #endregion

        #region Methods
        [Fact]
        public void BuildDateId()
        {
            var when = new DateTime(2019,3,1,8,46,57);
            var dateId = Data.Error.BuildDateId(when);
            Assert.Equal(190301084657, Convert.ToInt64(dateId, 16));
        }

        [Fact]
        public void BuildStringCounter()
        {
            var errorMessage = Data.Error.BuildString(Code, Message, trackingType: TrackingType.Counter, data: 27);
            Assert.Equal($"Error Encountered: {Message} ({Code} #27)", errorMessage);
        }

        [Fact]
        public void BuildStringGuid()
        {
            var guid = new Guid();
            var errorMessage = Data.Error.BuildString(Code, Message, trackingType: TrackingType.Guid, data: guid.ToString());
            Assert.Equal($"Error Encountered: {Message} ({Code} G{guid.ToString()})", errorMessage);
        }

        [Fact]
        public void EmptyCode()
        {
            var errorMessage = Data.Error.BuildString("", "This is a message");
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void NullCode()
        {
            var errorMessage = Data.Error.BuildString(null, "This is a message");
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void EmptyMessage()
        {
            var errorMessage = Data.Error.BuildString("MyCode", "");
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void NullMessage()
        {
            var errorMessage = Data.Error.BuildString("MyCode", null);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void EmptyCodeAndMessage()
        {
            var errorMessage = Data.Error.BuildString(string.Empty, string.Empty);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void NullCodeAndMessage()
        {
            var errorMessage = Data.Error.BuildString(null, null);
            Assert.NotEmpty(errorMessage);
        }
        #endregion

    }
}
