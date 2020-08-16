using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ParenthesisExprAST : ASTNode, IStorage, ISemantics
    {
        private ASTNode node;
        private string type;

        public ParenthesisExprAST(ASTNode node)
        {
            this.node = node;
        }

        public ASTNode GetNode()
        {
            return node;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[PARENTHESIS_L] (");
            node.Print(level + "\t");
            Console.WriteLine(level + "[PARENTHESIS_R] )");
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (node is IStorage)
                (node as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            type = (node as ISemantics).GetTypeMember();
            return type;
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			if (node == null) return;

			if (node is BinaryExprAST)
			{
				node.PrintASM(levelTabulatiion);
			}
			else if (node is ParenthesisExprAST)
			{
				node.PrintASM(levelTabulatiion);
			}
			else
			{
				string elementStorage;
				if (node is IEject)
				{
					elementStorage = (node as IEject).GetValue();
					string register = ASMregisters.GetFreeRegisterData();
					ConsoleHelper.WriteDefault("\t\tmov\t" + register + ", " + elementStorage);
					ConsoleHelper.WriteDefault("\t\tpush\t" + register);
				}
				else
				{
					node.PrintASM(levelTabulatiion);
				}
			}
		}
	}
}
