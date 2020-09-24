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

		public static void ClearStaicFlags()
		{
			isBoolNodeAnd = false;
			headNested = false;
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
					if(rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
					{
						rightNode.PrintASM(levelTabulatiion);
						PrintPop(levelTabulatiion, ref registerLeft);
						PrintPop(levelTabulatiion, ref registerRight);
					}
					else if (rightNode is BoolAST)
					{
						rightNode.PrintASM(levelTabulatiion);
						PrintPop(levelTabulatiion, ref registerLeft);
						PrintPop(levelTabulatiion, ref registerRight);
					}
					else
					{
						registerRight = IdentificatorOrValuePrint(levelTabulatiion, rightNode);
					}
				}
				else
				{
					if (leftNode is IdentificatorAST leftNodeIdentificatorAST)
					{
						IdentificatorMainPrint(levelTabulatiion, rightNode, leftNodeIdentificatorAST, ref registerLeft, ref registerRight);
					}
					else //value
					{
						ValuePrint(levelTabulatiion, leftNode, rightNode, ref registerLeft, ref registerRight);
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
				if (leftNode is BinaryExprAST || leftNode is ParenthesisExprAST)
				{
					leftNode.PrintASM(levelTabulatiion);
					if (rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
					{
						PrintExprAST(rightNode, levelTabulatiion);
						PrintPop(levelTabulatiion, ref registerLeft);
						PrintPop(levelTabulatiion, ref registerRight);
					}
					else
					{
						registerRight = IdentificatorOrValuePrint(levelTabulatiion, rightNode);
					}
				}
				else
				{
					if (leftNode is IdentificatorAST leftNodeIdentificator)
					{
						IdentificatorMainPrint(levelTabulatiion, rightNode, leftNodeIdentificator, ref registerLeft, ref registerRight);
					}
					else //value
					{
						ValuePrint(levelTabulatiion, leftNode, rightNode, ref registerLeft, ref registerRight);
					}
				}

				if (registerLeft == "") PrintPop(levelTabulatiion, ref registerLeft);
				if (registerRight == "") PrintPop(levelTabulatiion, ref registerRight);

				if (op != "/" && op != "%")
				{
					ASM.WriteASMCode(levelTabulatiion + ASMregisters.GetOperation(op) + "\t" + registerLeft + ", " + registerRight);
					ASM.WriteASMCode(levelTabulatiion + "push\t" + registerLeft);
				}
				else
				{
					string registerAdditive = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA, "ecx");
					ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerAdditive + ", " + registerLeft);
					if(op == "%") ASM.WriteASMCode(levelTabulatiion + "mov\tedx, 0");
					ASM.WriteASMCode(levelTabulatiion + "div\t" + registerAdditive);
					if(op == "%")
					{
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerLeft + ", edx");
					}
					ASMregisters.SetStateRegister(ASMregisters.Register.DATA, registerAdditive, true);
					ASM.WriteASMCode(levelTabulatiion + "push\t" + registerLeft);
				}
			}

			ASMregisters.SetStateRegister(ASMregisters.Register.DATA, registerLeft, true);
			ASMregisters.SetStateRegister(ASMregisters.Register.DATA, registerRight, true);
		}

		private void PrintExprAST(ASTNode node, string levelTabulatiion)
		{
			if(ASMregisters.HasFillRegisters(ASMregisters.Register.DATA)) ASM.WriteASMCode(levelTabulatiion + "push\t" + ASMregisters.GetFirstFillRegister(ASMregisters.Register.DATA));
			node.PrintASM(levelTabulatiion);
		}

		private void PrintPop(string levelTabulatiion, ref string register)
		{
			register = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
			ASM.WriteASMCode(levelTabulatiion + "pop\t" + register);
		}

		private void ValuePrint(string levelTabulatiion, ASTNode leftNode, ASTNode rightNode, ref string registerLeft, ref string registerRight)
		{
			registerLeft = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
			IEject valueLeft = (leftNode as IEject);
			ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerLeft + ", " + valueLeft.GetValue());
			if (rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
			{
				PrintExprAST(rightNode, levelTabulatiion);
				PrintPop(levelTabulatiion, ref registerLeft);
				PrintPop(levelTabulatiion, ref registerRight);
			}
			else
			{
				registerRight = IdentificatorOrValuePrint(levelTabulatiion, rightNode);
			}
		}

		private void IdentificatorMainPrint(string levelTabulatiion, ASTNode rightNode, IdentificatorAST leftNodeIdentificator, ref string registerLeft, ref string registerRight)
		{
			registerLeft = IdentificatorPrint(levelTabulatiion, leftNodeIdentificator);

			if (rightNode is BinaryExprAST || rightNode is ParenthesisExprAST)
			{
				PrintExprAST(rightNode, levelTabulatiion);
				PrintPop(levelTabulatiion, ref registerLeft);
				PrintPop(levelTabulatiion, ref registerRight);
			}
			else
			{
				registerRight = IdentificatorOrValuePrint(levelTabulatiion, rightNode);
			}
		}

		private string IdentificatorPrint(string levelTabulatiion, IdentificatorAST sideNodeIdentificatorAST)
		{
			string reigster = "";
			IdentificatorAST identificatorSide = (IdentificatorAST)SymTable.symTabls.FindNode(sideNodeIdentificatorAST.GetName());
			if (sideNodeIdentificatorAST.IsArray)
			{
				sideNodeIdentificatorAST.PrintArrayIterationASM(levelTabulatiion, isPushResult: true);
			}
			else
			{
				reigster = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
				int offsetInStack = sideNodeIdentificatorAST.GetOffseIfTtArray();
				int startInStack = identificatorSide.GetAddresInStack() + offsetInStack;
				ASM.WriteASMCode(levelTabulatiion + "mov\t" + reigster + ", " + ASMregisters.GetNameType(identificatorSide.GetTypeId()) + " [ebp-" + startInStack + "]");
			}
			return reigster;
		}

		private string IdentificatorOrValuePrint(string levelTabulatiion, ASTNode node)
		{
			string register = "";
			if (node is IdentificatorAST sideNodeIdentificatorAST)
			{
				register = IdentificatorPrint(levelTabulatiion, sideNodeIdentificatorAST);
			}
			else
			{
				register = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
				IEject value = (node as IEject);
				ASM.WriteASMCode(levelTabulatiion + "mov\t" + register + ", " + value.GetValue());
			}
			return register;
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
