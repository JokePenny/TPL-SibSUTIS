using System;
using lab1.Helpers;

namespace lab1
{
    public sealed class Command
    {
        public static void RunCommand(string[] command)
        {
            if(!CheckCorrectFilename(command))
			{
                Console.WriteLine("ERROR: what could be\n-Wrong path to file\n-Not correct extension (should be text.cs)");
			}

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

        private static bool CheckCorrectFilename(string[] command)
		{
            if (command.Length <= 1) return false;

            string pathToFile = command[1];
            string[] allDiectory = pathToFile.Split('\\');
            string nameFile = allDiectory[allDiectory.Length - 1];
            string[] partsNameFile = nameFile.Split('.');

            if (partsNameFile.Length <= 1) return false;
            if (partsNameFile[partsNameFile.Length - 1] != "cs") return false;

            return true;
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
