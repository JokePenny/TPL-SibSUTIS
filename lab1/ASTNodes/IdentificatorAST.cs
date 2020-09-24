using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class IdentificatorAST : ASTNode, IStorage, ISemantics
	{
        private string type = "";
        private readonly ASTNode storage;
		private readonly BracketsAST brackets;
        private readonly string nameID;

		//for ASM codegen
		private int startInStack;
		private string takeRegister = "";

		//for array
		private int lengthArray;
		private bool isArray;

		public bool IsArray => isArray;

		public IdentificatorAST(string type, string nameID, Point point, bool isArray)
        {
            this.nameID = nameID;
            this.type = type;
            this.point = point;
			this.isArray = isArray;
		}

        public IdentificatorAST(string type, string nameID, ASTNode storage, Point point, bool isArray)
        {
            this.nameID = nameID;
            this.type = type;
            this.storage = storage;
            this.point = point;
			this.isArray = isArray;
		}

		public IdentificatorAST(string type, string nameID, ASTNode storage, BracketsAST brackets, Point point, bool isArray)
		{
			this.nameID = nameID;
			this.type = type;
			this.storage = storage;
			this.brackets = brackets;
			this.point = point;
			this.isArray = isArray;
		}

		public IdentificatorAST(string nameID, Point point, BracketsAST brackets = null, bool isArray = false)
        {
            this.nameID = nameID;
            this.point = point;
			this.brackets = brackets;
			this.isArray = isArray;
		}

        public string GetTypeId()
        {
            return type;
        }

		public int GetAddresInStack(bool stackRoot = false)
		{
			if(stackRoot)
			{
				IdentificatorAST identificatorFindInTable = (IdentificatorAST)SymTable.symTabls.FindNode(GetName());
				if (identificatorFindInTable != null)
					return identificatorFindInTable.GetAddresInStack();
			}
			return startInStack;
		}

		public string GetName()
        {
            return nameID;
        }

		public int GetOffseIfTtArray()
		{
			return (isArray && brackets != null ? brackets.GetSizeArrayInt() : 0) * ASMregisters.GetSizeStep(type);
		}

		public string GetCodegenASM()
		{
			return ASMregisters.GetNameType(type) + " [ebp-" + GetAddresInStack(true) + "]";
		}

        public override void Print(string level)
        {
            if (type != "")
            {
                Console.WriteLine(level + "[TYPE] " + type);
                level = level + "\t";
            }
            Console.WriteLine(level + "[ID] " + nameID);

			if(brackets != null) brackets.Print(level);

            if (storage != null)
            {
                Console.WriteLine(level + "[STORAGE] =");
                storage.Print(level + "\t");
            }
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (symTable.ContainsKey(nameID))
            {
                if (type != "") ConsoleHelper.WriteError(nameID + " - Variable is redeclared");
                type = (symTable[nameID] as IdentificatorAST).GetTypeId();
            }
            else symTable.Add(nameID, this);

            if (storage is IStorage)
                (storage as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            if(storage is ISemantics && !(storage is BracketsAST))
            {
                string typeStorage = (storage as ISemantics).GetTypeMember();
                if (typeStorage != type) ConsoleHelper.WriteError("<" + point.y + "," + point.x + ">: Wrong type storage in '" + type + "' " + nameID + " = '" + typeStorage + "'");
            }
            return type;
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			if (storage == null && brackets == null) return;

			if (startInStack == 0)
			{
				IdentificatorAST identificatorFindInTable = (IdentificatorAST)SymTable.symTabls.FindNode(GetName());
				int offsetInStack = GetOffseIfTtArray();

				if(identificatorFindInTable != null && identificatorFindInTable != this)
				{
					startInStack = identificatorFindInTable.GetAddresInStack() + offsetInStack;
				}
				else
				{
					startInStack = ASMregisters.stepByte;
					ASMregisters.stepByte += ASMregisters.GetSizeStep(type);
				}
			}

			if (isArray)
			{
				if (storage is NewAST newASTArray)
				{
					lengthArray = newASTArray.GetSizeArray();

					int locateStack = startInStack;
					ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + locateStack + "], 0");
					for (int i = 1; i < lengthArray; i++)
					{
						locateStack = startInStack + (ASMregisters.GetSizeStep(type) * i);
						ASMregisters.stepByte += ASMregisters.GetSizeStep(type);
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + locateStack + "], 0");
					}
				}
				else if (storage is BinaryExprAST || storage is ParenthesisExprAST)
				{
					storage.PrintASM(levelTabulatiion);
					PrintArrayIterationASM(levelTabulatiion);
				}
				else
				{
					if (isNewLine)
					{
						string elementStorage;
						if (storage is StringAST stringAST)
						{
							isArray = true;
							stringAST.PrintSringArray(levelTabulatiion, startInStack);
						}
						else if (storage is IEject ejectedStorage)
						{
							elementStorage = ejectedStorage.GetValue();
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + elementStorage);
						}
						else
						{
							storage.PrintASM(levelTabulatiion);
							PrintArrayIterationASM(levelTabulatiion);
						}
					}
					else
					{
						PrintArrayIterationASM(levelTabulatiion, isPushResult: true);
					}
				}
			}
			else
			{
				if (storage is NewAST)
				{
					ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], 0");
				}
				else if (storage is BinaryExprAST || storage is ParenthesisExprAST)
				{
					storage.PrintASM(levelTabulatiion);
					string register = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
					ASM.WriteASMCode(levelTabulatiion + "pop\t" + register);
					ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + register);
					ASMregisters.SetStateRegister(ASMregisters.Register.DATA, register, true);
				}
				else
				{
					if (isNewLine)
					{
						string elementStorage;
						if(storage is StringAST stringAST)
						{
							isArray = true;
							stringAST.PrintSringArray(levelTabulatiion, startInStack);
						}
						else if (storage is IEject ejectedStorage)
						{
							elementStorage = ejectedStorage.GetValue();
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + elementStorage);
						}
						else
						{
							storage.PrintASM(levelTabulatiion);
							elementStorage = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
							ASM.WriteASMCode(levelTabulatiion + "pop\t" + elementStorage);
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + elementStorage);
							ASMregisters.SetStateRegister(ASMregisters.Register.DATA, elementStorage, true);
						}
					}
					else
					{
						takeRegister = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + takeRegister + ", " + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "]");
						ASM.WriteASMCode(levelTabulatiion + "push\t" + takeRegister);
						ASMregisters.SetStateRegister(ASMregisters.Register.DATA, takeRegister, true);
					}
				}
			}
		}

		/// <summary>
		/// Вывод массива
		/// </summary>
		public void PrintArrayASM(string levelTabulatiion)
		{
			string markerJumpPrevBody = ASMregisters.GetNewMarkerJumpPrevBody();
			string markerJumpAfterBody = ASMregisters.GetNewMarkerJumpAfterBody();
			ASMregisters.ClearMarkerPrevBody();

			string registerIteration = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
			ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerIteration + ", 0");

			ASM.WriteASMCode(levelTabulatiion + markerJumpPrevBody + ":");
			ASM.WriteASMCode(levelTabulatiion + "\tcmp\t" + registerIteration + ", " + (lengthArray - 1));
			ASM.WriteASMCode(levelTabulatiion + "\t" + ASMregisters.GetTypeConditionJump(">", true) + "\t" + markerJumpAfterBody);
			PrintArrayIterationASM(levelTabulatiion + "\t", registerIteration, isShowResult: true);
			ASM.WriteASMCode(levelTabulatiion + "\tadd\t" + registerIteration + ", 1");
			
			ASM.WriteASMCode(levelTabulatiion + "\tjmp\t" + markerJumpPrevBody);
			ASM.WriteASMCode(levelTabulatiion + markerJumpAfterBody + ":");
			ASM.WriteASMCode(levelTabulatiion + "push\t" + ASMregisters.NewString);
			ASM.WriteASMCode(levelTabulatiion + "call\t[printf]");

			ASMregisters.SetStateRegister(ASMregisters.Register.DATA, registerIteration, true);
		}

		/// <summary>
		/// Вывод переменной
		/// </summary>
		public void PrintIdentificatorASM(string levelTabulatiion)
		{
			string register = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
			PrintValueASM(levelTabulatiion, register);
			ASMregisters.SetStateRegister(ASMregisters.Register.DATA, register, true);
			ASM.WriteASMCode(levelTabulatiion + "push\t" + ASMregisters.NewString);
			ASM.WriteASMCode(levelTabulatiion + "call\t[printf]");
		}

		/// <summary>
		/// Итерация по массиву
		/// </summary>
		public void PrintArrayIterationASM
			(
			string levelTabulatiion, 
			string customSource = "", 
			bool isPushResult = false, 
			bool isPopInArray = false, 
			bool isShowResult = false
			)
		{
			string registerForOffset = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
			string registerBuffer = ASMregisters.GetFreeRegister(ASMregisters.Register.DATA);
			string specialRegister = ASMregisters.GetFreeRegister(ASMregisters.Register.SPECIAL);
			string source = isShowResult ? customSource : brackets.GetSizeArrayString();
			int sizeType = isShowResult ? 4 : ASMregisters.GetSizeStep(brackets.Type);

			ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerForOffset + ", 0");
			ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerBuffer + ", " + source);
			ASM.WriteASMCode(levelTabulatiion + "imul\t" + registerBuffer + ", " + sizeType);

			ASM.WriteASMCode(levelTabulatiion + "sub\t" + registerForOffset + ", " + registerBuffer);
			// ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerForOffset + ", " + registerBuffer);

			ASM.WriteASMCode(levelTabulatiion + "mov\t" + specialRegister + ", " + registerForOffset);

			if (isPopInArray)
			{
				ASM.WriteASMCode(levelTabulatiion + "pop\t" + registerForOffset);
				ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + GetAddresInStack(true) + " + " + specialRegister + "], " + registerForOffset);
			}
			
			if(isPushResult)
			{
				ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerForOffset + ", " + ASMregisters.GetNameType(type) + " [ebp-" + GetAddresInStack(true) + " + " + specialRegister + "]");
				ASM.WriteASMCode(levelTabulatiion + "push\t" + registerForOffset);
			}

			if(isShowResult)
			{
				PrintValueASM(levelTabulatiion, registerForOffset, " + " + specialRegister);
			}

			ASMregisters.SetStateRegister(ASMregisters.Register.DATA, registerBuffer, true);
			ASMregisters.SetStateRegister(ASMregisters.Register.DATA, registerForOffset, true);
			ASMregisters.SetStateRegister(ASMregisters.Register.SPECIAL, specialRegister, true);
		}

		private void PrintValueASM(string levelTabulatiion, string register, string offset = "")
		{
			ASM.WriteASMCode(levelTabulatiion + "mov\t" + register + ", " + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + offset + "]");
			ASM.WriteASMCode(levelTabulatiion + "push\t" + register);
			ASM.WriteASMCode(levelTabulatiion + "push\t" + ASMregisters.ShowString);
			ASM.WriteASMCode(levelTabulatiion + "call\t[printf]");
			ASM.WriteASMCode(levelTabulatiion + "push\t" + ASMregisters.SpaceString);
			ASM.WriteASMCode(levelTabulatiion + "call\t[printf]");
		}
	}
}
