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
		private bool isRightLastNode;

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

			if (CheckBoolOp())
				typeExpr = "bool";
			else
				typeExpr = typeLeftNode;

			return typeExpr;
        }

		private static bool isBoolNodeAnd = false;
		private static bool headNested = false;
		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			string registerLeft = "";
			string registerRight = "";
			int startInStack;
			if(!headNested)
			{
				headNested = true;
				FindLastRightNode(rightNode);
			}

			if (typeExpr == "bool")
			{
				if (leftNode is BoolAST)
				{
					isBoolNodeAnd = op == "&&";
					leftNode.PrintASM(levelTabulatiion);
					if(rightNode is BinaryExprAST)
					{
						rightNode.PrintASM(levelTabulatiion);
					}
					else if (rightNode is BoolAST)
					{
						rightNode.PrintASM(levelTabulatiion);
					}
					else
					{
						registerRight = ASMregisters.GetFreeRegisterData();
						if (rightNode is IdentificatorAST)
						{
							IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
							startInStack = identificatorRight.GetAddresInStack();
							ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
						}
						else
						{
							IEject valueRight = (rightNode as IEject);
							ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
						}
					}
				}
				else
				{
					registerLeft = ASMregisters.GetFreeRegisterData();
					if (leftNode is IdentificatorAST)
					{
						IdentificatorAST identificatorLeft = (IdentificatorAST)SymTable.symTabls.FindNode((leftNode as IdentificatorAST).GetName());
						startInStack = identificatorLeft.GetAddresInStack();
						ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerLeft + ", " + ASMregisters.GetNameType(identificatorLeft.GetTypeId()) + " [ebp-" + startInStack + "]");
						if (rightNode is BinaryExprAST)
						{
							rightNode.PrintASM(levelTabulatiion);
						}
						else
						{
							registerRight = ASMregisters.GetFreeRegisterData();
							if (rightNode is IdentificatorAST)
							{
								IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
								startInStack = identificatorRight.GetAddresInStack();
								ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
							}
							else
							{
								IEject valueRight = (rightNode as IEject);
								ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
							}
						}
					}
					else //value
					{
						IEject valueLeft = (leftNode as IEject);
						ConsoleHelper.WriteDefault("\t\tmov\t" + registerLeft + ", " + valueLeft.GetValue());
						if (rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
						{
							rightNode.PrintASM(levelTabulatiion);
						}
						else
						{
							registerRight = ASMregisters.GetFreeRegisterData();
							if (rightNode is IdentificatorAST)
							{
								IdentificatorAST identificator = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
								startInStack = identificator.GetAddresInStack();
								ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificator.GetTypeId()) + " [ebp-" + startInStack + "]");
							}
							else
							{
								IEject valueRight = (rightNode as IEject);
								ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
							}
						}
					}
				}

				if(op != "&&" && op != "||")
				{
					ConsoleHelper.WriteDefault(levelTabulatiion + "cmp\t" + registerLeft + ", " + registerRight);
					string marker = !isBoolNodeAnd && !isRightLastNode ? ASMregisters.GetNewMarkerJumpPrevBody() : ASMregisters.GetNewMarkerJumpAfterBody();
					//string marker = isBoolNodeAnd ? ASMregisters.GetNewMarkerJumpAfterBody() : ASMregisters.GetNewMarkerJumpPrevBody();
					ConsoleHelper.WriteDefault(levelTabulatiion + ASMregisters.GetTypeConditionJump(op, isBoolNodeAnd || isRightLastNode) + "\t" + marker);
				}
			}
			else
			{
				if (leftNode is BinaryExprAST)
				{
					leftNode.PrintASM(levelTabulatiion);
					if (rightNode is BinaryExprAST)
					{
						PrintExprAST(rightNode, levelTabulatiion);
						PrintPop(levelTabulatiion, ref registerRight);
					}
					else
					{
						registerRight = ASMregisters.GetFreeRegisterData();
						if (rightNode is IdentificatorAST)
						{
							IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
							startInStack = identificatorRight.GetAddresInStack();
							ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
						}
						else
						{
							IEject valueRight = (rightNode as IEject);
							ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
						}
					}
					registerLeft = ASMregisters.GetFreeRegisterData();
					ConsoleHelper.WriteDefault(levelTabulatiion + "pop\t" + registerLeft);
				}
				else
				{
					registerLeft = ASMregisters.GetFreeRegisterData();
					if (leftNode is IdentificatorAST)
					{
						IdentificatorAST identificatorLeft = (IdentificatorAST)SymTable.symTabls.FindNode((leftNode as IdentificatorAST).GetName());
						startInStack = identificatorLeft.GetAddresInStack();
						ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerLeft + ", " + ASMregisters.GetNameType(identificatorLeft.GetTypeId()) + " [ebp-" + startInStack + "]");
						if (rightNode is BinaryExprAST)
						{
							PrintExprAST(rightNode, levelTabulatiion);
							PrintPop(levelTabulatiion, ref registerRight);
							PrintPop(levelTabulatiion, ref registerLeft);
						}
						else
						{
							registerRight = ASMregisters.GetFreeRegisterData();
							if (rightNode is IdentificatorAST)
							{
								IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
								startInStack = identificatorRight.GetAddresInStack();
								ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
							}
							else
							{
								IEject valueRight = (rightNode as IEject);
								ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
							}
							PrintPop(levelTabulatiion, ref registerRight);
						}
					}
					else //value
					{
						IEject valueLeft = (leftNode as IEject);
						ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerLeft + ", " + valueLeft.GetValue());
						if (rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
						{
							PrintExprAST(rightNode, levelTabulatiion);
							PrintPop(levelTabulatiion, ref registerRight);
							PrintPop(levelTabulatiion, ref registerLeft);
						}
						else
						{
							registerRight = ASMregisters.GetFreeRegisterData();
							if (rightNode is IdentificatorAST)
							{
								IdentificatorAST identificator = (IdentificatorAST)SymTable.symTabls.FindNode((rightNode as IdentificatorAST).GetName());
								startInStack = identificator.GetAddresInStack();
								ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificator.GetTypeId()) + " [ebp-" + startInStack + "]");
							}
							else
							{
								IEject valueRight = (rightNode as IEject);
								ConsoleHelper.WriteDefault(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
							}
						}
					}
				}
				ConsoleHelper.WriteDefault(levelTabulatiion + ASMregisters.GetOperation(op) + "\t" + registerLeft + ", " + registerRight);
				ConsoleHelper.WriteDefault(levelTabulatiion + "push\t" + registerLeft);
			}
			if (registerLeft != "") ASMregisters.SetStateRegisterData(registerLeft, true);
			if (registerRight != "") ASMregisters.SetStateRegisterData(registerRight, true);
		}

		private void PrintExprAST(ASTNode node, string levelTabulatiion)
		{
			ConsoleHelper.WriteDefault(levelTabulatiion + "push\t" + ASMregisters.GetFirstFillRegister());
			node.PrintASM(levelTabulatiion);
		}

		private void PrintPop(string levelTabulatiion, ref string register)
		{
			register = ASMregisters.GetFreeRegisterData();
			ConsoleHelper.WriteDefault(levelTabulatiion + "pop\t" + register);
		}

		private bool CheckBoolOp()
		{
			switch (op)
			{
				case "<":
				case ">":
				case "!=":
				case "==":
				case ">=":
				case "<=":
					return true;
			}
			return false;
		}

		public void FindLastRightNode(ASTNode node)
		{
			if (node is BinaryExprAST)
			{
				(node as BinaryExprAST).FindLastRightNode(rightNode);
			}
			else if (node is BoolAST)
			{
				((node as BoolAST).GetExpression() as BinaryExprAST).FindLastRightNode(rightNode);
			}
			else
			{
				isRightLastNode = true;
			}
		}
	}
}
