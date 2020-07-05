using lab1.ASTNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.Asm
{
	class ASM
	{
		public static void CreateASM(ASTNode head)
		{
			if (head is NamespaceAST)
				(head as NamespaceAST).PrintASM(true);
		}
	}
}
