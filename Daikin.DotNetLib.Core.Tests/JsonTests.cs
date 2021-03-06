using Daikin.DotNetLib.Core.Tests.Models;
using Daikin.DotNetLib.Network;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class JsonTests
    {
        #region Tests
        [Fact]
        public void ObjectToJson()
        {
            var objVehicles = Vehicles.CreateSamples();
            var jsonVehicles = Json.ObjectToString(objVehicles);
            Assert.Equal(Vehicles.CreateJson(), jsonVehicles);
        }

        [Fact]
        public void JsonToObject()
        {
            var jsonVehicles = Vehicles.CreateJson();
            var objVehicles = Json.ObjectFromString<Vehicles>(jsonVehicles);
            var jsonSample = Vehicles.CreateJson();
            Assert.Equal(jsonSample, Json.ObjectToString(objVehicles));
        }
        #endregion
    }
}
