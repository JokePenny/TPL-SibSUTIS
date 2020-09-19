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
		public string TypeExpr => typeExpr;

		private string typeExpr;
        private readonly string op;
        private readonly ASTNode leftNode;
        private readonly ASTNode rightNode;
		private bool isRightLastNode;

		private static bool isBoolNodeAnd = false;
		private static bool headNested = false;

		public BinaryExprAST(string op, ASTNode leftNode, ASTNode rightNode, Point point)
        {
            this.op = op;
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            this.point = point;
        }

		public override void Print(string level)
        {
            Console.WriteLine(level + "[OP] " + op);
            leftNode.Print(level + "\t");
            rightNode.Print(level + "\t");
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (leftNode is IStorage leftNodeStorage)
				leftNodeStorage.AddAllSymbolIn(symTable);
            if (rightNode is IStorage rightNodeStorage)
				rightNodeStorage.AddAllSymbolIn(symTable);
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

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			string registerLeft = "";
			string registerRight = "";
			int offsetInStack;
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
							offsetInStack = identificatorRight.GetOffseIfTtArray();
							startInStack = identificatorRight.GetAddresInStack() + offsetInStack;
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
						}
						else
						{
							IEject valueRight = (rightNode as IEject);
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
						}
					}
				}
				else
				{
					if (leftNode is IdentificatorAST leftNodeIdentificatorAST)
					{
						IdentificatorAST identificatorLeft = (IdentificatorAST)SymTable.symTabls.FindNode(leftNodeIdentificatorAST.GetName());
						
						if (leftNodeIdentificatorAST.IsArray)
						{
							leftNodeIdentificatorAST.PrintArrayIterationASM(levelTabulatiion, isPushResult: true);
						}
						else
						{
							registerLeft = ASMregisters.GetFreeRegisterData();
							offsetInStack = leftNodeIdentificatorAST.GetOffseIfTtArray();
							startInStack = identificatorLeft.GetAddresInStack() + offsetInStack;
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerLeft + ", " + ASMregisters.GetNameType(identificatorLeft.GetTypeId()) + " [ebp-" + startInStack + "]");
						}

						if (rightNode is BinaryExprAST)
						{
							rightNode.PrintASM(levelTabulatiion);
						}
						else
						{
							if (rightNode is IdentificatorAST rightNodeIdentificatorAST)
							{
								IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode(rightNodeIdentificatorAST.GetName());
								if (rightNodeIdentificatorAST.IsArray)
								{
									rightNodeIdentificatorAST.PrintArrayIterationASM(levelTabulatiion, isPushResult: true);
								}
								else
								{
									registerRight = ASMregisters.GetFreeRegisterData();
									offsetInStack = rightNodeIdentificatorAST.GetOffseIfTtArray();
									startInStack = identificatorRight.GetAddresInStack() + offsetInStack;
									ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
								}
							}
							else
							{
								registerRight = ASMregisters.GetFreeRegisterData();
								IEject valueRight = (rightNode as IEject);
								ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
							}
						}
					}
					else //value
					{
						IEject valueLeft = (leftNode as IEject);
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerLeft + ", " + valueLeft.GetValue());
						if (rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
						{
							rightNode.PrintASM(levelTabulatiion);
						}
						else
						{
							registerRight = ASMregisters.GetFreeRegisterData();
							if (rightNode is IdentificatorAST rightNodeIdentificatorAST)
							{
								IdentificatorAST identificator = (IdentificatorAST)SymTable.symTabls.FindNode(rightNodeIdentificatorAST.GetName());
								offsetInStack = rightNodeIdentificatorAST.GetOffseIfTtArray();
								startInStack = identificator.GetAddresInStack() + offsetInStack;
								ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificator.GetTypeId()) + " [ebp-" + startInStack + "]");
							}
							else
							{
								IEject valueRight = (rightNode as IEject);
								ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
							}
						}
					}
				}

				if (registerLeft == "") PrintPop(levelTabulatiion, ref registerLeft);
				if (registerRight == "") PrintPop(levelTabulatiion, ref registerRight);

				if (op != "&&" && op != "||")
				{
					ASM.WriteASMCode(levelTabulatiion + "cmp\t" + registerLeft + ", " + registerRight);
					string marker = !isBoolNodeAnd && !isRightLastNode ? ASMregisters.GetNewMarkerJumpPrevBody() : ASMregisters.GetNewMarkerJumpAfterBody();
					ASM.WriteASMCode(levelTabulatiion + ASMregisters.GetTypeConditionJump(op, isBoolNodeAnd || isRightLastNode) + "\t" + marker);
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
						if (rightNode is IdentificatorAST rightNodeIdentificatorAST)
						{
							IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode(rightNodeIdentificatorAST.GetName());
							offsetInStack = rightNodeIdentificatorAST.GetOffseIfTtArray();
							startInStack = identificatorRight.GetAddresInStack() + offsetInStack;
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
						}
						else
						{
							IEject valueRight = (rightNode as IEject);
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
						}
					}
					registerLeft = ASMregisters.GetFreeRegisterData();
					ASM.WriteASMCode(levelTabulatiion + "pop\t" + registerLeft);
				}
				else
				{
					registerLeft = ASMregisters.GetFreeRegisterData();
					if (leftNode is IdentificatorAST leftNodeIdentificator)
					{
						IdentificatorAST identificatorLeft = (IdentificatorAST)SymTable.symTabls.FindNode(leftNodeIdentificator.GetName());
						offsetInStack = leftNodeIdentificator.GetOffseIfTtArray();
						startInStack = identificatorLeft.GetAddresInStack() + offsetInStack;
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerLeft + ", " + ASMregisters.GetNameType(identificatorLeft.GetTypeId()) + " [ebp-" + startInStack + "]");
						if (rightNode is BinaryExprAST)
						{
							PrintExprAST(rightNode, levelTabulatiion);
							PrintPop(levelTabulatiion, ref registerRight);
							PrintPop(levelTabulatiion, ref registerLeft);
						}
						else
						{
							registerRight = ASMregisters.GetFreeRegisterData();
							if (rightNode is IdentificatorAST rightNodeIdentificatorAST)
							{
								IdentificatorAST identificatorRight = (IdentificatorAST)SymTable.symTabls.FindNode(rightNodeIdentificatorAST.GetName());
								offsetInStack = rightNodeIdentificatorAST.GetOffseIfTtArray();
								startInStack = identificatorRight.GetAddresInStack() + offsetInStack;
								ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificatorRight.GetTypeId()) + " [ebp-" + startInStack + "]");
							}
							else
							{
								IEject valueRight = (rightNode as IEject);
								ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
							}
							PrintPop(levelTabulatiion, ref registerRight);
						}
					}
					else //value
					{
						IEject valueLeft = (leftNode as IEject);
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerLeft + ", " + valueLeft.GetValue());
						if (rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
						{
							PrintExprAST(rightNode, levelTabulatiion);
							PrintPop(levelTabulatiion, ref registerRight);
							PrintPop(levelTabulatiion, ref registerLeft);
						}
						else
						{
							registerRight = ASMregisters.GetFreeRegisterData();
							if (rightNode is IdentificatorAST rightNodeIdentificatorAST)
							{
								IdentificatorAST identificator = (IdentificatorAST)SymTable.symTabls.FindNode(rightNodeIdentificatorAST.GetName());
								offsetInStack = rightNodeIdentificatorAST.GetOffseIfTtArray();
								startInStack = identificator.GetAddresInStack() + offsetInStack;
								ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + ASMregisters.GetNameType(identificator.GetTypeId()) + " [ebp-" + startInStack + "]");
							}
							else
							{
								IEject valueRight = (rightNode as IEject);
								ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerRight + ", " + valueRight.GetValue());
							}
						}
					}
				}
				ASM.WriteASMCode(levelTabulatiion + ASMregisters.GetOperation(op) + "\t" + registerLeft + ", " + registerRight);
				ASM.WriteASMCode(levelTabulatiion + "push\t" + registerLeft);
			}
			if (registerLeft != "") ASMregisters.SetStateRegisterData(registerLeft, true);
			if (registerRight != "") ASMregisters.SetStateRegisterData(registerRight, true);
		}

		private void PrintExprAST(ASTNode node, string levelTabulatiion)
		{
			ASM.WriteASMCode(levelTabulatiion + "push\t" + ASMregisters.GetFirstFillRegisterData());
			node.PrintASM(levelTabulatiion);
		}

		private void PrintPop(string levelTabulatiion, ref string register)
		{
			register = ASMregisters.GetFreeRegisterData();
			ASM.WriteASMCode(levelTabulatiion + "pop\t" + register);
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
			if (node is BinaryExprAST binaryExprNode)
			{
				binaryExprNode.FindLastRightNode(rightNode);
			}
			else if (node is BoolAST boolNode)
			{
				(boolNode.GetExpression() as BinaryExprAST).FindLastRightNode(rightNode);
			}
			else
			{
				isRightLastNode = true;
			}
		}
	}
}
