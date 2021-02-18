using Daikin.DotNetLib.Serilog;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class SerilogTests
    {
        [Fact]
        public void TestTruncate()
        {
            string s = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Null(s.Truncate(5));
            s = "1234567890";
            Assert.Equal("12345", s.Truncate(5));
            s = "12";
            Assert.Equal("12", s.Truncate(10));
        }
    }
}
