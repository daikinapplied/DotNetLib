using System;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class Error
    {
        [Fact]
        public void BuildDate()
        {
            var when = new DateTime(2019,3,1,8,46,57);
            var errorMessage = Data.Error.BuildDate(when);
            Assert.Equal("1903010846", errorMessage);
        }

        [Fact]
        public void BuildString()
        {
            var errorMessage = Data.Error.BuildString("Ref A17", "Unable to access the whatchamacallit", errorNumber: 27);
            Assert.Equal("Error Encountered: Unable to access the whatchamacallit (Ref A17 #27)", errorMessage);
        }

    }
}
