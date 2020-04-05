using System;
using lab1.Helpers;

namespace lab1
{
    public sealed class Command
    {
        public static void RunCommand(string[] command)
        {
            switch (command[0])
            {
                case "--dump-asm":
                    break;
                case "--dump-ast":
                    DumpAST(command[1]);
                    break;
                case "--dump-tokens":
                    DumpTokens(command[1]);
                    break;
                case "--test":
                    Test(command[1]);
                    break;
            }
        }

        private static void Test(string path)
        {
            string source = ReadSource.ReadFile(path);
            if (source != "")
            {
                Lexer.StartLexer(source);
            }
        }

        private static void DumpASM(string path)
        {

        }

        private static void DumpAST(string path)
        {
            string source = ReadSource.ReadFile(path);
            if (source != "")
            {
                Lexer.StartLexer(source);
                AbstractSyntaxTree.CreateAST();
            }
            else
            {
                ConsoleHelper.WriteError("Исходник пустой");
            }
            Lexer.ViewTokens();
        }

        private static void DumpTokens(string path)
        {
            string source = ReadSource.ReadFile(path);
            if (source != "")
            {
                Lexer.StartLexer(source);
                Lexer.ParseLexem();
                Lexer.ViewTokens();
            }
            else
            {
                ConsoleHelper.WriteError("Исходник пустой");
            }
        }
    }
}
