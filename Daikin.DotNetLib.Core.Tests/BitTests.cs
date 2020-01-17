using System;
using Daikin.DotNetLib.Data;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class BitTests
    {
        [Fact]
        public void BitInt()
        {
            // 1 (binary) == 1 decimal
            var test = Bit.IsBitSet(1, 1);
            if (!test) throw new Exception("For number 1, expected 1 in first position");

            // 10 (binary) == 2 decimal
            if (Bit.IsBitSet(2, 1)) throw new Exception("For number2, expected 0 in first position");

            // 1010 (binary) == 10 decimal
            if (!Bit.IsBitSet(10, 4)) throw new Exception("For number 10, expected 1 in fourth position");
            if (Bit.IsBitSet(10, 3)) throw new Exception("For number 10, expected 0 in the third position");
            if (!Bit.IsBitSet(10, 2)) throw new Exception("For number 10, expected 1 in the second position");
            if (Bit.IsBitSet(10, 1)) throw new Exception("For number 10, expected 0 in the first position");
        }
    }
}
