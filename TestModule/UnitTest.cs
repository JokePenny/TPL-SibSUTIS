using System;
using Xunit;

namespace lab1.TestModule
{
    public class UnitTest
    {
        [Fact]
        public void InsertSpaceBetween_Str_returnStrWithSpaceFormat()
        {
            //arrange
            string str = "int[]=i;";
            string expected = "int [ ] = i ;";

            //act
            string actual = StringTreatment.InsertSpaceBetween(str);
            //assert
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void SplitString_Str_returnStrArray()
        {
            //arrange
            string str = "int [ ] = i ; \"das  dasd\"";
            string[] expected = { "int", "[", "]", "=", "i", ";", "\"das  dasd\"" };

            //act
            string[] actual = StringTreatment.SplitString(str);
            //assert
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void FormatString_Str_returnStrFromat()
        {
            //arrange
            string str = "int[]=i;";
            string[] expected = { "int", "[", "]", "=", "i", ";" };

            //act
            string[] actual = StringTreatment.FormatStroke(str);
            //assert
            Assert.Equal(actual, expected);
        }

        [Theory]
        [InlineData ("//int[]=i;\n asdasd ", "\n asdasd ")]
        [InlineData("//int[]=i;\n// asdasd ", "\n")]
        [InlineData("/*int[]=i;\n asdasd */", "")] // удаление мульти комментариев
        [InlineData("a//int[]=i;", "a")]
        public void FindComments_StrWithCommInMultyLine_returnStrWithoutComms(string str, string expected)
        {
            //act
            string actual = StringTreatment.FindComments(str);
            //assert
            Assert.Equal(actual, expected);
        }
    }
}
