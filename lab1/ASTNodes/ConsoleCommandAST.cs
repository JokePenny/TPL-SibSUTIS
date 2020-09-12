using lab1.Asm;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
	class ConsoleCommandAST : ASTNode
	{
		private readonly ASTNode storage;
		private readonly Tokens.Token command;

		public override void Print(string level)
		{
			Console.WriteLine(level + "[CONSOLE COMMAND]: " + command.ToString());
			if (storage != null)
			{
				Console.WriteLine(level + "[STORAGE] =");
				storage.Print(level + "\t");
			}
		}

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			//string code;
			switch(command)
			{
				case Tokens.Token.CONSOLE_READLINE:
					//code = levelTabulatiion + 
					break;
				case Tokens.Token.CONSOLE_READ_KEY:
					//code = 
					break;
				case Tokens.Token.CONSOLE_WRITE:
					break;
				case Tokens.Token.CONSOLE_WRITELINE:
					break;
			}
			//ASM.WriteASMCode(code);
		}

		public ConsoleCommandAST(Tokens.Token command, ASTNode storage = null)
		{
			this.command = command;
			this.storage = storage;
		}
	}
}
