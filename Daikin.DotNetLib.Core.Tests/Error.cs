using System;
using Daikin.DotNetLib.Data;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class Error
    {
        #region Constants
        const string Code = "Ref A17";
        const string Message = "Unable to access the whatchamacallit";

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
        #endregion

    }
}
