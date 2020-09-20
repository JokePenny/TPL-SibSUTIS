using lab1.Asm;
using lab1.SymbolTable;
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
			if(storage is IdentificatorAST identificatorStorage)
			{
				IdentificatorAST identificatorAST = (IdentificatorAST)SymTable.symTabls.FindNode(identificatorStorage.GetName());
				if(identificatorAST.IsArray)
				{
					identificatorAST.PrintArrayASM(levelTabulatiion);
				}
				else
				{
					identificatorAST.PrintIdentificatorASM(levelTabulatiion);
				}
			} 
			else if (storage is StringAST stringAST)
			{
				//Вывод строек не работает
				//stringAST.PrintSringArray(levelTabulatiion, stringAST.GetString(), startInStack);
			}
		}

		public ConsoleCommandAST(Tokens.Token command, ASTNode storage = null)
		{
			this.command = command;
			this.storage = storage;
		}
	}
}
