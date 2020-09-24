using System;
using lab1.Asm;
using lab1.Helpers;

namespace lab1
{
    public sealed class Command
    {
        private enum TypeCommand
		{
            DUMP_TOKENS,
            DUMP_AST,
            DUMP_ASM,
        }

        public static void RunCommand(string[] command)
        {
            if(!CheckCorrectFilename(command))
			{
                Console.WriteLine("ERROR: what could be\n-Wrong path to file\n-Not correct extension (should be text.cs)");
			}

            switch (command[0])
            {
                case "--dump-asm":
                    ReadFile(command[1], TypeCommand.DUMP_ASM, command.Length > 2 ? command[2] : null);
                    break;
                case "--dump-ast":
                    ReadFile(command[1], TypeCommand.DUMP_AST);
                    break;
                case "--dump-tokens":
                    ReadFile(command[1], TypeCommand.DUMP_TOKENS);
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

        private static void ReadFile(string path, TypeCommand command, string pathToFasm = null)
		{
            if (pathToFasm != null) ReadSource.SetPathToFasm(pathToFasm);

            string source = ReadSource.ReadFile(path);
            if (source != "")
            {
                SwitcherCommand(source, command);
            }
            else
            {
                ConsoleHelper.WriteError("Исходник пустой");
            }
        }

        private static void SwitcherCommand(string source, TypeCommand command)
		{
            switch(command)
			{
                case TypeCommand.DUMP_TOKENS:
                    Lexer.StartLexer(source);
                    Lexer.ParseLexem();
                    Lexer.ViewTokens();
                    break;
                case TypeCommand.DUMP_AST:
                    Lexer.StartLexer(source);
                    AbstractSyntaxTree.CreateAST(true, true);
                    break;
                case TypeCommand.DUMP_ASM:
                    Lexer.StartLexer(source);
                    AbstractSyntaxTree.CreateAST(true, true);
                    ASM.CreateASM();
                    ASM.RunCompileProgramm();
                    break;
            }
		}
    }
}
