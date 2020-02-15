using System;

namespace lab1
{
    sealed class Command
    {
        public static void RunCommand(string[] command)
        {
            switch (command[3])
            {
                case "--dump-asm":
                    break;
                case "--dump-ast":
                    break;
                case "--dump-tokens":
                    DumpTokens(command[4]);
                    break;
            }
        }

        private static void DumpASM(string path)
        {

        }

        private static void DumpAST(string path)
        {

        }

        private static void DumpTokens(string path)
        {
            string source = ReadSource.ReadFile(path);
            if (source != "")
            {
                Lexer.StartLexer(source);
                Lexer.ViewTokens();
            }
            else
            {
                ViewWarning("Исходник пустой");
            }
        }

        private static void ViewWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
