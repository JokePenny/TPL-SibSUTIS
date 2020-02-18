using System;
using Xunit;

namespace lab1.TestModule
{
    public class UnitTest
    {
        [Fact]
        public void InsertSpaceBetween_Str_returnStrWithspaceFormat()
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

        [Fact]
        public void FindComments_StrWithComm_returnStrWithoutComm()
        {
            //arrange
            string str = "//int[]=i;";
            string expected = "";

            //act
            string actual = StringTreatment.FindComments(str);
            //assert
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void FindComments_StrWithMultyComm_returnStrWithoutComm()
        {
            //arrange
            string str = "/*int[]=i;\n asdasd */";
            string expected = "";

            //act
            string actual = StringTreatment.FindComments(str);
            //assert
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void FindComments_StrWithCommInMultyLine_returnStrWithoutComm()
        {
            //arrange
            string str = "//int[]=i;\n asdasd ";
            string expected = "\n asdasd ";

            //act
            string actual = StringTreatment.FindComments(str);
            //assert
            Assert.Equal(actual, expected);
        }
    }
}
