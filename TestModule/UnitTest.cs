using System;
using Xunit;
using lab1.ASTNodes;

namespace lab1.TestModule
{
    public class UnitTest
    {
        //-------
        // Тесты Lexer
        //-------

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

        //-------
        // Тесты AST
        //-------


        [Fact]
        public void AST_ParseNamespace_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test1.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseMainArea(AbstractSyntaxTree.Area.NAMESPACE);
            //assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void AST_ParseClass_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test2.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseMainArea(AbstractSyntaxTree.Area.CLASS);
            //assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void AST_ParseMethod_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test3.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseMethod("", "", false);
            //assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void AST_ParseIfBrace_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test4.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseIf();
            //assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void AST_ParseIfNotBrace_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test5.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseIf();
            //assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void AST_ParseForBrace_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test6.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseFor();
            //assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void AST_ParseForNotBrace_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test7.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseFor();
            //assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void AST_ParseWhileBrace_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test8.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseWhile();
            //assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void AST_ParseWhileNotBrace_returnNotNull()
        {
            string[] path = { "--test", "\\tests\\test9.txt" };
            Command.RunCommand(path);
            //act
            ASTNode actual = AbstractSyntaxTree.ParseWhile();
            //assert
            Assert.NotNull(actual);
        }
    }
}
