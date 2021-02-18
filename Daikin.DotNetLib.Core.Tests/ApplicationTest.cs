using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class ApplicationTest
    {
        [Fact]
        public void AssemblyTypeOf()
        {
            var assembly = Application.Information.GetAssembly(typeof(ApplicationTest));
            Assert.NotNull(assembly);
        }

        [Fact]
        public void VersionSimpleMessage()
        {
            var version = Application.Information.GetVersionMessage();
            Assert.NotNull(version);
            Assert.NotEmpty(version);
        }

        [Fact]
        public void VersionMessage()
        {
            var assembly = Application.Information.GetAssembly();
            Assert.NotNull(assembly);
            var version = Application.Information.GetVersionMessage(assembly);
            Assert.NotNull(version);
            Assert.NotEmpty(version);
        }

        [Fact]
        public void VersionSimple()
        {
            var version = Application.Information.GetVersion();
            Assert.NotNull(version);
            Assert.NotEmpty(version);
        }

        [Fact]
        public void NameSimple()
        {
            var name = Application.Information.GetName();
            Assert.NotNull(name);
            Assert.NotEmpty(name);
        }
    }
}
