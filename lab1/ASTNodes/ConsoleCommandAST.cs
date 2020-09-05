using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
	class ConsoleCommandAST : ASTNode
	{
		private ASTNode storage;
		private Tokens.Token command;

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{

		}

		public ConsoleCommandAST(Tokens.Token command, ASTNode storage = null)
		{
			this.command = command;
			this.storage = storage;
		}
	}
}
