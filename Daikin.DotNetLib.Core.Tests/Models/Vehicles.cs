using System.Collections.Generic;

namespace Daikin.DotNetLib.Core.Tests.Models
{
    public class Vehicles : List<Vehicle>
    {
        public static Vehicles CreateSamples()
        {
            return new Vehicles
            {
                new Vehicle {Make = "Ford", Model = "Escort", Trim = "LX", Year = 1991},
                new Vehicle {Make = "Honda", Model = "Civic", Trim = "LX", Year = 1994},
                new Vehicle {Make = "Audi", Model = "A4", Trim = "Avant", Year = 1998},
                new Vehicle {Make = "Audi", Model = "S4", Trim = "Avant", Year = 1999},
            };
        }

        public static string CreateJson()
        {
            return "["
                   + "{\"Make\":\"Ford\",\"Model\":\"Escort\",\"Trim\":\"LX\",\"Year\":1991},"
                   + "{\"Make\":\"Honda\",\"Model\":\"Civic\",\"Trim\":\"LX\",\"Year\":1994},"
                   + "{\"Make\":\"Audi\",\"Model\":\"A4\",\"Trim\":\"Avant\",\"Year\":1998},"
                   + "{\"Make\":\"Audi\",\"Model\":\"S4\",\"Trim\":\"Avant\",\"Year\":1999}"
                   + "]";
        }
    }
}
