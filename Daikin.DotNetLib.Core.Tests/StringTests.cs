using System;
using Xunit;
using String = Daikin.DotNetLib.Data.String;

namespace Daikin.DotNetLib.Core.Tests
{
    public class StringTests
    {
        #region Constants
        private const string BigString = "TheQuickBrownFoxJumpedOverTheLazyDog";
        private const string SmallString = "1234567890";
        #endregion

        #region Methods
        [Fact]
        public void StringLeftSimple()
        {
            var leftString = String.Left(BigString, 10);
            Assert.Equal("TheQuickBr", leftString);
        }

        [Fact]
        public void StringLeftShort()
        {
            var leftString = String.Left(SmallString, 20);
            Assert.Equal(SmallString, leftString);
        }

        [Fact]
        public void StringLeftNull()
        {
            var leftString = String.Left(null, 10);
            Assert.Null(leftString);
        }

        [Fact]
        public void StringRightSimple()
        {
            var rightString = String.Right(BigString, 10);
            Assert.Equal("TheLazyDog", rightString);
        }

        [Fact]
        public void StringRightNull()
        {
            var rightString = String.Right(null, 10);
            Assert.Null(rightString);
        }


        [Fact]
        public void StringRightShort()
        {
            var leftString = String.Right(SmallString, 20);
            Assert.Equal(SmallString, leftString);
        }

        [Fact]
        public void MassiveReplace()
        {
            var replacementSet = new[,] { {"o", "!"}, {"Lazy", "Smiling"} };
            var resultString = String.MassiveReplace(BigString, replacementSet);
            Assert.Equal("TheQuickBr!wnF!xJumpedOverTheSmilingD!g", resultString);
        }

        [Fact]
        public void GetIniValue()
        {
            const string iniValues = "one=1;two=2;three=3;four=four;;five=;seven=0007";
            Assert.Equal("1",String.GetIniValue(iniValues, "one"));
            Assert.Equal(string.Empty,String.GetIniValue(iniValues, "five"));
            Assert.Equal("0007",String.GetIniValue(iniValues, "Seven"));
            Assert.Null(String.GetIniValue(iniValues, "on"));
            Assert.Null(String.GetIniValue(iniValues, "six"));
            Assert.Null(String.GetIniValue(iniValues, "Seven", caseInsentive: false));
        }

        [Fact]
        public void CharsInString()
        {
            const string s = "OneTwoThreeFourFive";
            Assert.True(String.CharInString(s, "oF"));
            Assert.True(String.CharInString(s, "of"));
            Assert.True(String.CharInString(s, "OT"));
            Assert.False(String.CharInString(s, "1"));
            Assert.False(String.CharInString(s, "WR"));
        }

        [Fact]
        public void ConvertFromBytes()
        {
            Assert.Equal("Ha", String.ConvertFromBytes(new [] {(byte)'H', (byte)'a'}));
        }

        [Fact]
        public void ConvertToBytes()
        {
            Assert.Equal(new [] {(byte)'H', (byte)'a'}, String.ConvertToBytes("Ha"));
        }

        [Fact]
        public void Split()
        {
            const string set = "one two three four five";
            Assert.Equal("one", String.Split(set, ' ', 0));
            Assert.Equal("three", String.Split(set, ' ', 2));
            Assert.Null(String.Split(set, ' ', 5));
        }

        [Fact]
        public void DelimitBuild()
        {
            Assert.Equal("one=1;two=2;three=three", String.DelimiterBuild(new [] {"one=1", "two=2", "three=t;hree"}, ";"));
        }

        [Fact]
        public void ConvertToInt32()
        {
            Assert.Equal(27, String.ConvertToInt32("27", -1));
            Assert.Equal(-1, String.ConvertToInt32("s27", -1));
        }
        #endregion
    }
}
