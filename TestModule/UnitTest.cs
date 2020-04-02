using System;
using Xunit;

namespace lab1.TestModule
{
    public class UnitTest
    {
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
