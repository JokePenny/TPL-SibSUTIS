using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class BinaryExprAST : ASTNode, IStorage, ISemantics
    {
        private string typeExpr;
        private string op;
        private ASTNode leftNode;
        private ASTNode rightNode;

        public BinaryExprAST(string op, ASTNode leftNode, ASTNode rightNode, Point point)
        {
            this.op = op;
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            this.point = point;
        }

        public BinaryExprAST(ASTNode leftNode, Point point)
        {
            this.leftNode = leftNode;
            this.point = point;
        }

        public string GetTypeExp()
        {
            return typeExpr;
        }

		public string GetOp()
		{
			return op;
		}

		public override void Print(string level)
        {
            Console.WriteLine(level + "[OP] " + op);
            leftNode.Print(level + "\t");
            rightNode.Print(level + "\t");
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (leftNode is IStorage)
                (leftNode as IStorage).AddAllSymbolIn(symTable);
            if (rightNode is IStorage)
                (rightNode as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            string typeLeftNode = (leftNode as ISemantics).GetTypeMember();
            string typerightNode = (rightNode as ISemantics).GetTypeMember();
            if (typeLeftNode != typerightNode || typeLeftNode == "")
            {
                ConsoleHelper.WriteError("<" + point.y + "," + point.x + ">: different types -> '" + typeLeftNode + "' " + op + " '" + typerightNode + "'");
                return "";
            }
			//new added
			typeExpr = typeLeftNode;
			return typeExpr;
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			string registerLeft = "";
			string registerRight = "";
			int startInStack = 0;
			if (leftNode is BinaryExprAST)
			{
				leftNode.PrintASM(levelTabulatiion);
				if (rightNode is BinaryExprAST)
				{
					ConsoleHelper.WriteDefault("\t\tpush\t" + ASMregisters.GetFirstFillRegister());
					rightNode.PrintASM(levelTabulatiion);
					registerRight = ASMregisters.GetFreeRegisterData();
					ConsoleHelper.WriteDefault("\t\tpop\t" + registerRight);
				}
				else
				{
					registerRight = ASMregisters.GetFreeRegisterData();
					if (rightNode is IdentificatorAST)
					{
						IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
						startInStack = identificatorRight.GetAddresInStack();
						ConsoleHelper.WriteDefault("\t\tmov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
					}
					else
					{
						IEject valueRight = (rightNode as IEject);
						ConsoleHelper.WriteDefault("\t\tmov\t" + registerRight + ", " + valueRight.GetValue());
					}
				}
				registerLeft = ASMregisters.GetFreeRegisterData();
				ConsoleHelper.WriteDefault("\t\tpop\t" + registerLeft);
			}
			else
			{
				registerLeft = ASMregisters.GetFreeRegisterData();
				if (leftNode is IdentificatorAST)
				{
					IdentificatorAST identificatorLeft = (IdentificatorAST)SymTable.symTabls.FindNode((leftNode as IdentificatorAST).GetName());
					startInStack = identificatorLeft.GetAddresInStack();
					ConsoleHelper.WriteDefault("\t\tmov\t" + registerLeft + ", " + ASMregisters.GetNameType(identificatorLeft.GetTypeId()) + " [ebp-" + startInStack + "]");
					if (rightNode is BinaryExprAST)
					{
						ConsoleHelper.WriteDefault("\t\tpush\t" + ASMregisters.GetFirstFillRegister());
						rightNode.PrintASM(levelTabulatiion);
						registerRight = ASMregisters.GetFreeRegisterData();
						ConsoleHelper.WriteDefault("\t\tpop\t" + registerRight);
						registerLeft = ASMregisters.GetFreeRegisterData();
						ConsoleHelper.WriteDefault("\t\tpop\t" + registerLeft);
					}
					else
					{
						string registeRight = ASMregisters.GetFreeRegisterData();
						if (rightNode is IdentificatorAST)
						{
							IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
							startInStack = identificatorRight.GetAddresInStack();
							ConsoleHelper.WriteDefault("\t\tmov\t" + registeRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
						}
						else
						{
							IEject valueRight = (rightNode as IEject);
							ConsoleHelper.WriteDefault("\t\tmov\t" + registeRight + ", " + valueRight.GetValue());
						}
						registerLeft = ASMregisters.GetFreeRegisterData();
						ConsoleHelper.WriteDefault("\t\tpop\t" + registerLeft);
					}
				}
				else //value
				{
					IEject valueLeft = (leftNode as IEject);
					ConsoleHelper.WriteDefault("\t\tmov\t" + registerLeft + ", " + valueLeft.GetValue());
					if (rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
					{
						ConsoleHelper.WriteDefault("\t\tpush\t" + ASMregisters.GetFirstFillRegister());
						rightNode.PrintASM(levelTabulatiion);
						registerRight = ASMregisters.GetFreeRegisterData();
						ConsoleHelper.WriteDefault("\t\tpop\t" + registerRight);
						registerLeft = ASMregisters.GetFreeRegisterData();
						ConsoleHelper.WriteDefault("\t\tpop\t" + registerLeft);
					}
					else
					{
						registerRight = ASMregisters.GetFreeRegisterData();
						if (rightNode is IdentificatorAST)
						{
							IdentificatorAST identificator = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
							startInStack = identificator.GetAddresInStack();
							ConsoleHelper.WriteDefault("\t\tmov\t" + registerRight + ", " + ASMregisters.GetNameType(identificator.GetTypeId()) + " [ebp-" + startInStack + "]");
						}
						else
						{
							IEject valueRight = (rightNode as IEject);
							ConsoleHelper.WriteDefault("\t\tmov\t" + registerRight + ", " + valueRight.GetValue());
						}
					}
				}
			}
			ConsoleHelper.WriteDefault("\t\t" + ASMregisters.GetOperation(op) + "\t" + registerLeft + ", " + registerRight);
			ConsoleHelper.WriteDefault("\t\tpush\t" + registerLeft);

			if (registerLeft != "") ASMregisters.SetStateRegisterData(registerLeft, true);
			if (registerRight != "") ASMregisters.SetStateRegisterData(registerRight, true);
		}
	}
}
